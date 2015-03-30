namespace Trezorix.Sparql.Api.Application.FileRepositories
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;

  using Newtonsoft.Json;

  using Trezorix.Sparql.Api.Core.Accounts;
  using Trezorix.Sparql.Api.Core.Repositories;

  public class FileAccountRepository: IAccountRepository 
  {
	  private bool _loaded;
    private string _repositoryPath;
		private IList<Account> _accounts; 

		public FileAccountRepository(string repositoryPath)
		{
			this._repositoryPath = repositoryPath;
			if (!this._repositoryPath.EndsWith("\\"))
			{
				this._repositoryPath += "\\";
			}
			this._accounts = new List<Account>();

		}

		private void LoadAllFromPath() 
    {
		  if (this._loaded) return;
      var files = Directory.EnumerateFiles(this._repositoryPath);
			foreach (var file in files)
			{
				if (file.ToLower().EndsWith(".json"))
				{
					this._accounts.Add(this.LoadFromFile(file));
				}
			}
		  this._loaded = true;
		}	  

	  public void Delete(Account account)
		{
			string filename = this.FilenameFromAccountName(account.ApiKey.ToString());

			if (this._accounts.Any(q => q.ApiKey == account.ApiKey))
			{
				this._accounts.Remove(account);
			}

			File.Delete(filename);
			//return account;
		}

		public IEnumerable<Account> All()
		{
			//if (_accounts.Count == 0)
			{
				this.LoadAllFromPath();
			}
			return this._accounts;
		}

		public Account Get(string id)
		{
			var account = this._accounts.FirstOrDefault(q => q.ApiKey.ToString() == id);

			//if (account == null)
			{
				string filename = this.FilenameFromAccountName(id);
				account = this.LoadFromFile(filename);
			}

			return account;
		}

	  public Account GetById(string id) 
    {
	    return this.All().SingleOrDefault(a => a.Id == id);
	  }

		public Account GetByApiKey(string apiKey) {
			throw new NotImplementedException();
		}

		public IEnumerable<Account> GetByApiKeys(IEnumerable<string> apiKeys) {
			throw new NotImplementedException();
		}

		public Account GetByUserName(string userName)
		{
			return this.All().SingleOrDefault(a => a.UserName == userName);
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

        account.Id = account.ApiKey;

        return account;
			}

			return null;
		}

    public Account Save(Account account) {
      var accountResult = account.Id == null ? this.Add(account) : this.Update(account);
      return accountResult;
    }

	  private Account Add(Account account) 
    {
      return this.Update(account);
    }

    private Account Update(Account account) {


      account.Id = account.ApiKey;
      dynamic item = new { account.Id, account.ApiKey, account.UserName, account.FullName, account.Password, account.Roles };
      
      string json = JsonConvert.SerializeObject(item, Formatting.Indented);
      File.WriteAllText(this.FilenameFromAccountName(account.ApiKey), json);
      return account;
    }

		private string FilenameFromAccountName(string name)
		{
			string filename = this._repositoryPath + name + ".json";
			return filename;
		}

	}
}