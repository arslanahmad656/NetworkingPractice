using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Project1.Basics
{
    static class WebClientExamples
    {
        public static void Run()
        {
            //Demo1();
            //DownloadFtpFileUsingWebClient();
            //DownloadFtpFileUsingWebRequest();
            //DownloadFilesConcurrentlyWithHttpClient();
            //DemoHttpClient();
            UploadUsingHttpClient();
        }

        static void Demo1()
        {
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential("apitesting656@gmail.com", "webdir123R");
            client.DownloadFile("https://doc-0o-60-docs.googleusercontent.com/docs/securesc/5d6ibgb24c7f96ehl23nodtja6hqm4l0/t1bll4mm6cbtbu7k3ivelcdr3hhltj8t/1556186400000/07971844216046138297/07971844216046138297/0B4IIvrnGe211c3RhcnRlcl9maWxl?e=download&nonce=q4gkqkia566g2&user=07971844216046138297&hash=5m371tevnrmr060fo3ib2j9kvn87rr6v", "drive.html");
        }

        static void DownloadFtpFileUsingWebClient()
        {
            var webclient = new WebClient
            {
                Proxy = null,
                Credentials = new NetworkCredential("tfs18", "v9YI02UiEg")
            };

            webclient.DownloadFile("ftp://tfs18@192.168.0.195/GroupIDDev/10.0.7034.5/NewInstallers.sln.log", "file.zip");

            Console.WriteLine("Download completed");
        }

        static void DownloadFtpFileUsingWebRequest()
        {
            var request = WebRequest.Create("ftp://tfs18@192.168.0.195/GroupIDDev/10.0.7034.5/NewInstallers.sln.log");
            request.Proxy = null;
            request.Credentials = new NetworkCredential("tfs18", "v9YI02UiEg");

            //Console.WriteLine($"Request content type: {request.ContentType}");
            Console.WriteLine("Request headers:");
            foreach (var item in request.Headers)
            {
                Console.WriteLine($"{item.GetType().FullName}: {item.ToString()}");
            }

            var response = request.GetResponse();

            //Console.WriteLine($"Response content type: {response.ContentType}");
            Console.WriteLine("Request headers:");
            foreach (var item in response.Headers)
            {
                Console.WriteLine($"{item.GetType().FullName}: {item.ToString()}");
            }

            using (var responseStream = response.GetResponseStream())
            using (var fileStream = File.Create("download.txt"))
            {
                responseStream.CopyTo(fileStream);
                fileStream.Flush();
            }

            Console.WriteLine("File donwloaded");
            Process.Start("download.txt");
        }

        static async void DemoHttpClient()
        {
            var client = new HttpClient(new HttpClientHandler
            {
                Proxy = null
            });

            Console.WriteLine("Request headers:");
            Console.WriteLine();
            Console.WriteLine("Accept:");
            foreach (var item in client.DefaultRequestHeaders.Accept)
            {
                Console.WriteLine($"\tMedia type: {item.MediaType}");
                Console.WriteLine($"\tParameters:");
                foreach (var item2 in item.Parameters)
                {
                    Console.WriteLine($"\t\t{item2.Name}: {item2.Value}");
                }
            }
            
            HttpResponseMessage response1 = await client.GetAsync("https://www.google.com.pk/");
            HttpResponseMessage response2 = await client.GetAsync("https://www.google.com.pk/");
        }

        static void DownloadFilesConcurrentlyWithHttpClient()
        {
            var httpClient = new HttpClient(new HttpClientHandler
            {
                Proxy = null
            });

            var t1 = httpClient.GetStreamAsync("https://www.google.com.pk/");
            var t2 = httpClient.GetStreamAsync("https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone?view=netframework-4.8");

            t1.ContinueWith(stream => OnDownloadFile(stream.Result, "google.html"));
            t2.ContinueWith(stream => OnDownloadFile(stream.Result, "msdn.html"));

            Task.WaitAll(new[] { t1, t2 });

            void OnDownloadFile(Stream stream, string fileName)
            {
                stream.CopyTo(File.Create(fileName));
                Console.WriteLine($"File {fileName} downloaded.");
            }
        }

        static async void UploadUsingHttpClient()
        {
            var client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false
            });

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://www.albahari.com/EchoPost.aspx")
            {
                Content = new StringContent("Test posting from a c# papplication")
            };

            var response = client.SendAsync(request).Result;
            Console.WriteLine($"Response code: {response.StatusCode} ({response.ReasonPhrase})");
            Console.WriteLine($"Response content: {await response.Content.ReadAsStringAsync()}");
        }
    }
}
