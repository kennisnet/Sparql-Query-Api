using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Trezorix.Sparql.Api.Core.Queries;

namespace Trezorix.Sparql.Api.Core.Repositories
{
	public class FileQueryRepository: IQueryRepository
	{
		private string _repositoryPath;
		private IList<Query> _queries; 

		public FileQueryRepository(string repositoryPath)
		{
			_repositoryPath = repositoryPath;
			if (!_repositoryPath.EndsWith("\\"))
			{
				_repositoryPath += "\\";
			}
			_queries = new List<Query>();

		}

		private void LoadAllFromPath()
		{
			var files = Directory.EnumerateFiles(_repositoryPath);
			foreach (var file in files)
			{
				if (file.ToLower().EndsWith(".json"))
				{
					_queries.Add(LoadFromFile(file));
				}
			}
		}

		public IEnumerable<Query> All()
		{
			//if (_queries.Count == 0)
			{
				LoadAllFromPath();
			}
			return _queries;
		}

		public void Delete(Query query)
		{
			string filename = FilenameFromQueryName(query.Id);

			if (_queries.Any(q => q.Id == query.Id))
			{
				_queries.Remove(query);
			}

			File.Delete(filename);
		}

		public Query Get(string name) {
			return this.GetByAlias(name);
		}

		public Query GetByAlias(string alias)
		{
			var query = _queries.FirstOrDefault(q => q.Id == alias);

			//if (query == null)
			{
				string filename = FilenameFromQueryName(alias);

				if (!File.Exists(filename))
				{
					return null;
				}

				query = LoadFromFile(filename);
			}

			return query;
		}

    public Query GetById(string id) {
      return All().SingleOrDefault(a => a.Id == id);
    }
    
    private Query LoadFromFile(string filename)
		{
			string json = File.ReadAllText(filename);

			var item = JsonConvert.DeserializeObject<dynamic>(json);
			var query =
				new Query()
				{
					Id = item.Id,
					Alias = item.Id,
					AllowAnonymous = item.AllowAnonymous,
					Description = item.Description,
					Endpoint = item.Endpoint,
					Group = item.Group,
					Label = item.Label,
					SparqlQuery = item.SparqlQuery,
				};

			query.ApiKeys = (item.ApiKeys != null) ? item.ApiKeys.ToObject<List<string>>() : new List<string>();
			query.Notes = (item.Notes != null) ? item.Notes.ToObject<List<Note>>() : new List<Note>();
			query.Parameters = (item.Parameters != null) ? item.Parameters.ToObject<List<QueryParameter>>() : new List<QueryParameter>(); ;

			return query;
		}

    public Query Add(Query query) {
      return this.Update(query);
    }

    public Query Update(Query query) {
      //query.Id = query.ApiKey.AsObjectId().ToString();
      dynamic item =
        new {
          query.Id,
          query.AllowAnonymous,
          query.ApiKeys,
          query.Description,
          query.Endpoint,
          query.Group,
          query.Label,
          query.Notes,
          query.Parameters,
          query.SparqlQuery,
        };

      string json = JsonConvert.SerializeObject(item, Formatting.Indented);
      File.WriteAllText(FilenameFromQueryName(query.Id), json);
      return query;
    }
    
    public void Save(string name, Query query)
		{
			string json = JsonConvert.SerializeObject(query, Formatting.Indented);
			File.WriteAllText(FilenameFromQueryName(name), json);
		}
		
		private string FilenameFromQueryName(string name)
		{
			string filename = _repositoryPath + name + ".json";
			return filename;
		}

		public void FixAll()
		{
			LoadAllFromPath();
			foreach (var query in _queries)
			{
				File.WriteAllText( _repositoryPath + query.Id + ".txt", query.SparqlQuery);
			}
		}


	}
}