namespace Trezorix.Sparql.Api.Application.FileRepositories
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;

  using Newtonsoft.Json;

  using Trezorix.Sparql.Api.Core.Queries;
  using Trezorix.Sparql.Api.Core.Repositories;

  public class FileQueryRepository: IQueryRepository
	{
		private string _repositoryPath;
		private IList<Query> _queries; 

		public FileQueryRepository(string repositoryPath)
		{
			this._repositoryPath = repositoryPath;
			if (!this._repositoryPath.EndsWith("\\"))
			{
				this._repositoryPath += "\\";
			}
			this._queries = new List<Query>();
		}

		private void LoadAllFromPath()
		{
			var files = Directory.EnumerateFiles(this._repositoryPath);
			foreach (var file in files)
			{
				if (file.ToLower().EndsWith(".json"))
				{
					this._queries.Add(this.LoadFromFile(file));
				}
			}
		}

		public IEnumerable<Query> All()
		{
			//if (_queries.Count == 0)
			{
				this.LoadAllFromPath();
			}
			return this._queries;
		}

		public void Delete(Query query)
		{
			string filename = this.FilenameFromQueryName(query.Id);

			if (this._queries.Any(q => q.Id == query.Id))
			{
				this._queries.Remove(query);
			}

			File.Delete(filename);
		}

		public Query Get(string name) {
			return this.GetByAlias(name);
		}

		public Query GetByAlias(string alias)
		{
			var query = this._queries.FirstOrDefault(q => q.Id == alias);

			//if (query == null)
			{
				string filename = this.FilenameFromQueryName(alias);

				if (!File.Exists(filename))
				{
					return null;
				}

				query = this.LoadFromFile(filename);
			}

			return query;
		}

    public Query GetById(string id) {
      return this.All().SingleOrDefault(a => a.Id == id);
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

      this.SaveToFile(query.Id, item);
      return query;
    }

    public Query Save(Query query) 
    {
      return this.Update(query);
    }

    private void SaveToFile(string name, Query query)
		{
			string json = JsonConvert.SerializeObject(query, Formatting.Indented);
			File.WriteAllText(this.FilenameFromQueryName(name), json);
		}
		
		private string FilenameFromQueryName(string name)
		{
			string filename = this._repositoryPath + name + ".json";
			return filename;
		}

		public void FixAll()
		{
			this.LoadAllFromPath();
			foreach (var query in this._queries)
			{
				File.WriteAllText( this._repositoryPath + query.Id + ".txt", query.SparqlQuery);
			}
		}


	}
}