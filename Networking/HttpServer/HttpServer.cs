using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace HttpServer
{
    class HttpServer
    {
        private string prefix;
        private string scheme;
        private string host;
        private int port;
        private string baseFolder;

        private HttpListener listener;

        public HttpServer(string url, string baseFolder)
        {
            if (!url.EndsWith("/"))
            {
                url = url.Insert(url.Length, "/");
            }
            var uri = new Uri(url);
            if (!Regex.IsMatch(uri.Scheme, @"(?i)https?"))
            {
                throw new Exception("Invalid URI. Only http/https is supported");
            }

            prefix = url;
            this.scheme = uri.Scheme;
            this.host = uri.Host;
            this.port = uri.Port;

            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }

            this.baseFolder = baseFolder;

            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(url);
            }
            catch (Exception ex)
            {

                throw new Exception("Error initializing server", ex);
            }
        }

        public async void StartAsync()
        {
            listener.Start();
            Console.WriteLine($"Server started at {prefix}");

#pragma warning disable CS4014
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var context = await listener.GetContextAsync();
                        Task.Run(() => ProcessRequest(context));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Cannot process requests anymore...", ex);
                    }
                }
            });
#pragma warning restore
        }

        public void Stop()
        {
            listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async void ProcessRequest(HttpListenerContext context)
        {
            Console.WriteLine("Received a request.");
            var message = "<h1>Request has been processed and response generated.</h1>";
            context.Response.Cookies.Add(new Cookie("test", "testcookie"));
            Console.WriteLine("Cookies received from request: " + context.Request.Cookies.Count);
            context.Response.ContentLength64 = message.Length;
            context.Response.Redirect("https://www.bing.com");
            using (var writer = new StreamWriter(context.Response.OutputStream))
            {
                writer.AutoFlush = true;
                await writer.WriteAsync(message);
            }
            context.Response.StatusCode = (int)HttpStatusCode.OK;
        }
    }
}
