using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trezorix.Sparql.Api.Admin.Models.Statistics
{
	public class LogDownloadListModel
	{
		public int TotalCount { get; set; }
		public int Start { get; set; }
		public int End { get; set; }
		public List<LogDownLoadModel> Items { get; set; }
	}

	public class LogDownLoadModel
	{
		public string Label { get; set; }
		public string Start { get; set; }
		public string End { get; set; }
	}
}