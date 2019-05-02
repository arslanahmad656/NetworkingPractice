using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new HttpServer("http://localhost:40000/myserver", "folder");
            server.StartAsync();

            Console.ReadKey();
        }
    }
}
