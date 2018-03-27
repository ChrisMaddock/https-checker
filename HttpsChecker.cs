using System;
using System.Net;

namespace https_checker
{
    internal static class HttpsChecker
    {
        public static string FindUnredirectedHttpsFor(string inputUri)
        {
            var uriBuilder = new UriBuilder(new Uri(inputUri))
            {
                Scheme = Uri.UriSchemeHttps,
                Port = -1 // default port for scheme
            };

            //Build current Uri under https
            var inputHttpsUri = uriBuilder.Uri;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(inputHttpsUri);
            HttpWebResponse response = null;
            Uri responseUri;
            HttpStatusCode? responseCode;

            try
            {
                response = (HttpWebResponse) request.GetResponse();
                responseUri = response.ResponseUri;
                responseCode = response.StatusCode;
            }
            catch (WebException we)
            {
                var wRespStatusCode = ((HttpWebResponse) we.Response)?.StatusCode.ToString() ?? "UNKNOWN";
                Console.WriteLine($"{wRespStatusCode} recieved from {inputHttpsUri}");
                return null;
            }
            finally
            {
                response?.Close();
            }

            if (responseUri != inputHttpsUri)
            {
                Console.WriteLine($"Skipped due to redirect: {inputHttpsUri}");
                return null;
            }

            if (responseCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"{responseCode} recieved from: {inputHttpsUri}");
                return null;
            }

            Console.WriteLine($"Successful URI found: {inputHttpsUri}");
            return inputHttpsUri.AbsoluteUri;
        }
    }
}