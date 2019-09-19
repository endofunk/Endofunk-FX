// Web.cs
//
// MIT License
// Copyright (c) 2019 endofunk
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Net;
using Endofunk.FX;
using static Endofunk.FX.Prelude;

namespace Endofunk.FX.Net {
  public static class Web {
    private static Func<string, Result<string>> DownloadUrl(bool debug = false, string filePath = "")
      => EscapeURL
      .Compose(ConnectToURL)
      .Compose(ValidateHTTPResponse)
      .Compose(PrintHttpWebResponseProperties(debug))
      .Compose(GetDataFromInputStream(filePath));
      
    public static Result<A> DownloadAndDeserialize<A>(this Result<string> url, Func<string, Result<A>> constructor, bool debug = false, string filePath = "") {
      return url.Bind(DownloadUrl(debug, filePath).Compose(constructor));
    }

    public static Func<string, Result<string>> EscapeURL => url => Try(() => Uri.EscapeUriString(url));
    public static Func<string, Result<WebRequest>> ConnectToURL => url => Try(() => WebRequest.Create(url));

    public static Func<WebRequest, Result<HttpWebResponse>> ValidateHTTPResponse => request => Try(() => {
      var response = request.GetResponse() as HttpWebResponse;
      if (response.StatusCode == HttpStatusCode.OK) return response;
      throw new WebException(response.GetResponseStream().ReadToEndOfStream());
    });

    private static void SaveJson(string filePath, string contents) {
      try { 
        File.WriteAllText(filePath, contents);
      } catch (Exception e) {
        Console.WriteLine($"SaveJson: {LogDefault(e)}");
      }
    }

    public static Func<HttpWebResponse, Result<string>> GetDataFromInputStream(string filePath) => response => Try(() => {
      var result = response.GetResponseStream().ReadToEndOfStream();
      response.Close();
      if (!filePath.IsEmpty()) SaveJson(filePath, result);      
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