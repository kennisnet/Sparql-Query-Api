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

		public Query Get(string name)
		{
			var query = _queries.FirstOrDefault(q => q.Id == name);

			//if (query == null)
			{
				string filename = FilenameFromQueryName(name);

				if (!File.Exists(filename))
				{
					return null;
				}

				query = LoadFromFile(filename);
			}


			return query;
		}

		private Query LoadFromFile(string filename)
		{
			string json = File.ReadAllText(filename);
			var query = JsonConvert.DeserializeObject<Query>(json);
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