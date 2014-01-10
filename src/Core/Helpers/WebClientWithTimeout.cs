using System;
using System.Net;

namespace Trezorix.Sparql.Api.Core.Helpers {

  internal class WebClientWithTimeout : WebClient {
    // Timeout in milliseconds

    /// <summary>
    ///   Sets default timeout
    /// </summary>
    public WebClientWithTimeout() {
      Timeout = 90000;
    }

    /// <summary>
    ///   Sets custom timeout
    /// </summary>
    /// <param name="timeout">Timeout in milliseconds</param>
    public WebClientWithTimeout(int timeout) {
      Timeout = timeout;
    }

    public int Timeout { get; set; }

    /// <summary>
    ///   Overriding base method to set timeout
    /// </summary>
    /// <param name="address">Server Url</param>
    /// <returns>A WebRequest with a timeout assigned</returns>
    protected override WebRequest GetWebRequest(Uri address) {
      WebRequest wr = base.GetWebRequest(address);
      wr.Timeout = Timeout;
      return wr;
    }
  }

}
