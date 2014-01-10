using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Trezorix.Sparql.Api.Core.Sparql
{
	public class Parameterized‎SparqlQuery
	{
		private readonly string _query;
		private readonly string _namespaces;
		public Dictionary<string, string> Params { get; set; }

		public Parameterized‎SparqlQuery(string namespaces, string text)
		{
			_query = text;
			_namespaces = namespaces;
			Params = ParseParamsFromQueryText(_query);
		}

		public string Query
		{
			get
			{
				var text = _query;
				foreach (var param in Params)
				{
					text = text.Replace("$$" + param.Key, param.Value);
				}
				return _namespaces + text;
			}
		}

		private Dictionary<string, string> ParseParamsFromQueryText(string text)
		{
			var paramList = new Dictionary<string, string>();

			var regex = new Regex(@"\$\$\w+", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
			var match = regex.Match(text);

			while (match.Success)
			{
				var param = match.Value;
				paramList.Add(param.Substring(2), null);
				//text = text.Replace(param, model[param.Substring(2)].Value);
				match = match.NextMatch();
			}

			return paramList;
		}

		public void InjectParameterValues(dynamic data)
		{
			var paramList = Params.ToList();

			foreach (var param in paramList)
			{
				if (data[param.Key] != null)
				{
					if (data[param.Key] is string)
					{
						Params[param.Key] = data[param.Key];
					}
					else if (data[param.Key] is JValue)
					{
						Params[param.Key] = data[param.Key].Value;
					}
				}
			} 

		}
	}
}