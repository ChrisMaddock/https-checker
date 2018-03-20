using System;
using System.Net;

namespace https_checker
{
    internal static class HttpsChecker
    {
        public static string FindHttpsFor(string inputUri)
        {
            var uriBuilder = new UriBuilder(new Uri(inputUri))
            {
                Scheme = Uri.UriSchemeHttps,
                Port = -1 // default port for scheme
            };

            var inputHttpsUri = uriBuilder.Uri;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(inputHttpsUri);
            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException we)
            {
                var wRespStatusCode = ((HttpWebResponse)we.Response)?.StatusCode.ToString() ?? "UNKNOWN";
                Console.WriteLine($"{wRespStatusCode} recieved from {inputHttpsUri}");
                return null;
            }

            if (response.ResponseUri != inputHttpsUri)
            {
                Console.WriteLine($"A different Uri responded for {inputHttpsUri}.");
                return null;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"{response.StatusCode} recieved from {inputHttpsUri}.");
                return null;
            }

            Console.WriteLine($"{inputHttpsUri} was successful");
            return inputHttpsUri.AbsoluteUri;
        }
    }
}