using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace Project1.Basics
{
    static class WebClientExamples
    {
        public static void Run()
        {
            Demo1();
        }

        static void Demo1()
        {
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential("apitesting656@gmail.com", "webdir123R");
            client.DownloadFile("https://doc-0o-60-docs.googleusercontent.com/docs/securesc/5d6ibgb24c7f96ehl23nodtja6hqm4l0/t1bll4mm6cbtbu7k3ivelcdr3hhltj8t/1556186400000/07971844216046138297/07971844216046138297/0B4IIvrnGe211c3RhcnRlcl9maWxl?e=download&nonce=q4gkqkia566g2&user=07971844216046138297&hash=5m371tevnrmr060fo3ib2j9kvn87rr6v", "drive.html");
        }
    }
}
