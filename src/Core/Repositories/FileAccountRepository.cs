using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Trezorix.Sparql.Api.Core.Accounts;

namespace Trezorix.Sparql.Api.Core.Repositories
{
	public class FileAccountRepository: IAccountRepository
	{
		private string _repositoryPath;
		private IList<Account> _accounts; 

		public FileAccountRepository(string repositoryPath)
		{
			_repositoryPath = repositoryPath;
			if (!_repositoryPath.EndsWith("\\"))
			{
				_repositoryPath += "\\";
			}
			_accounts = new List<Account>();

		}

		private void LoadAllFromPath()
		{
			var files = Directory.EnumerateFiles(_repositoryPath);
			foreach (var file in files)
			{
				if (file.ToLower().EndsWith(".json"))
				{
					_accounts.Add(LoadFromFile(file));
				}
			}
		}

		public void Delete(Account account)
		{
			string filename = FilenameFromAccountName(account.Id);

			if (_accounts.Any(q => q.Id == account.Id))
			{
				_accounts.Remove(account);
			}

			File.Delete(filename);
			//return account;
		}

		public IEnumerable<Account> All()
		{
			//if (_accounts.Count == 0)
			{
				LoadAllFromPath();
			}
			return _accounts;
		}

		public Account Get(string id)
		{
			var account = _accounts.FirstOrDefault(q => q.Id == id);

			//if (account == null)
			{
				string filename = FilenameFromAccountName(id);
				account = LoadFromFile(filename);
			}

			return account;
		}

		public Account GetByUserName(string userName)
		{
			return All().SingleOrDefault(a => a.UserName == userName);
		}

		private Account LoadFromFile(string filename)
		{
			if (File.Exists(filename))
			{
				string json = File.ReadAllText(filename);
				var account = JsonConvert.DeserializeObject<Account>(json);
				return account;
			}

			return null;
		}

		public void Save(string name, Account account)
		{
			string json = JsonConvert.SerializeObject(account, Formatting.Indented);
			File.WriteAllText(FilenameFromAccountName(name), json);
		}

		private string FilenameFromAccountName(string name)
		{
			string filename = _repositoryPath + name + ".json";
			return filename;
		}

	}
}