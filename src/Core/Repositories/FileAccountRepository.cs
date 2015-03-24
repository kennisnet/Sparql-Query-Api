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
	  private bool _loaded;
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
		  if (_loaded) return;
      var files = Directory.EnumerateFiles(_repositoryPath);
			foreach (var file in files)
			{
				if (file.ToLower().EndsWith(".json"))
				{
					_accounts.Add(LoadFromFile(file));
				}
			}
		  _loaded = true;
		}

		public void Delete(Account account)
		{
			string filename = FilenameFromAccountName(account.ApiKey.ToString());

			if (_accounts.Any(q => q.ApiKey == account.ApiKey))
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
			var account = _accounts.FirstOrDefault(q => q.ApiKey.ToString() == id);

			//if (account == null)
			{
				string filename = FilenameFromAccountName(id);
				account = LoadFromFile(filename);
			}

			return account;
		}

	  public Account GetById(string id) 
    {
	    return All().SingleOrDefault(a => a.Id == id);
	  }

		public Account GetByApiKey(string apiKey) {
			throw new NotImplementedException();
		}

		public IEnumerable<Account> GetByApiKeys(IEnumerable<string> apiKeys) {
			throw new NotImplementedException();
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
				var item = JsonConvert.DeserializeObject<dynamic>(json);
        var account = new Account();
        account.ApiKey = item.ApiKey;
			  account.UserName = item.UserName;
			  account.FullName = item.FullName;
			  account.Password = item.Password;
        account.Roles = (item.Roles != null) ? ((Newtonsoft.Json.Linq.JArray) item.Roles).Select(s => s.ToString()) : null;

        account.Id = account.ApiKey.AsObjectId().ToString();

        return account;
			}

			return null;
		}

    public Account Add(Account account) 
    {
      return this.Update(account);
    }

    public Account Update(Account account) {
      account.Id = account.ApiKey.AsObjectId().ToString();
      dynamic item =
        new { account.Id, account.ApiKey, account.UserName, account.FullName, account.Password, account.Roles };
      
      string json = JsonConvert.SerializeObject(item, Formatting.Indented);
      File.WriteAllText(FilenameFromAccountName(account.ApiKey.ToString()), json);
      return account;
    }

		private string FilenameFromAccountName(string name)
		{
			string filename = _repositoryPath + name + ".json";
			return filename;
		}

	}
}