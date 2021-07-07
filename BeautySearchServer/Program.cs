using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BeautySearchServer
{
    class HttpServer
    {
        private const String URL = "http://localhost:8087/";
        private static HttpListener listener;

        private static String lastRequestedUriSite = "";

        public static void Main(string[] args)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(URL);
            listener.Start();

            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }

        public static async Task HandleIncomingConnections()
        {
            while (true)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse res = ctx.Response;

                Console.WriteLine("NEW REQ");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Mobile Safari/537.36");
                    string requestedUri = Uri.UnescapeDataString(req.RawUrl).Substring(1);
                    if(!requestedUri.StartsWith("http://") && !requestedUri.StartsWith("https://"))
                    {
                        requestedUri = lastRequestedUriSite + (requestedUri.StartsWith("/") ? "" : "/") + requestedUri;
                        //res.Close();
                        //continue;
                    }
                    Uri uri = new Uri(requestedUri);
                    lastRequestedUriSite = uri.Scheme + "://" + uri.Authority;
                    Console.WriteLine(lastRequestedUriSite);
                    using (HttpResponseMessage response = await client.GetAsync(uri))
                    {
                        using (HttpContent content = response.Content)
                        {
                            string result = await content.ReadAsStringAsync();

                            byte[] data = Encoding.UTF8.GetBytes("<!DOCTYPE html><html><body>Response</body></html>");
                            res.ContentType = "text/html";
                            res.ContentEncoding = Encoding.UTF8;
                            res.ContentLength64 = data.LongLength;

                            res.Headers.Add("Access-Control-Allow-Origin", "*");
                            res.Headers.Add("Content-Security-Policy", "child-src *");

                            await res.OutputStream.WriteAsync(data, 0, data.Length);
                            res.Close();
                        }
                    }
                }
            }
        }
    }
}