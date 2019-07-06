using System;
using System.Net;
using Endofunk.FX;
using static Endofunk.FX.Prelude;

namespace Endofunk.FX.Net {
  public static class Web {
    private static Func<string, Result<string>> DownloadUrl(bool debug = false)
      => EscapeURL
      .Compose(ConnectToURL)
      .Compose(ValidateHTTPResponse)
      .Compose(PrintHttpWebResponseProperties(debug))
      .Compose(GetDataFromInputStream);
      
    public static Result<A> DownloadAndDeserialize<A>(this Result<string> url, Func<string, Result<A>> constructor, bool debug = false) {
      return url.Bind(DownloadUrl(debug).Compose(constructor));
    }

    public static Func<string, Result<string>> EscapeURL => url => Try(() => Uri.EscapeUriString(url));
    public static Func<string, Result<WebRequest>> ConnectToURL => url => Try(() => WebRequest.Create(url));

    public static Func<WebRequest, Result<HttpWebResponse>> ValidateHTTPResponse => request => Try(() => {
      var response = request.GetResponse() as HttpWebResponse;
      if (response.StatusCode == HttpStatusCode.OK) return response;
      throw new WebException(response.GetResponseStream().ReadToEndOfStream());
    });

    public static Func<HttpWebResponse, Result<string>> GetDataFromInputStream => response => Try(() => {
      var result = response.GetResponseStream().ReadToEndOfStream();
      response.Close();
      return result;
    });

    public static Func<HttpWebResponse, Result<HttpWebResponse>> PrintHttpWebResponseProperties(bool debug = false) => response => Try(() => {
      if (!debug) return response;
      var properties = $"Server: {response.Server}\n" +
        $"ProtocolVersion: {response.ProtocolVersion}\n" +
        $"ContentType: {response.ContentType}\n" +
        $"StatusCode: {(int)response.StatusCode}\n" +
        $"ContentEncoding: {response.ContentLength}\n" +
        $"Headers: {response.Headers}\n" +
        $"ResponseUri: {response.ResponseUri}\n" +
        $"StatusDescription: {response.StatusDescription}";
      Console.WriteLine("{0}\n{1}\n{0}\n", new string('-', 80), properties);
      return response;
    });
  }
}