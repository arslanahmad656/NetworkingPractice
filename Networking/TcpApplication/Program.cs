using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TcpApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var t1 = Task.Run(() => ServerRunner.Run());
            Thread.Sleep(2000);
            var t2 = Task.Run(() => ClientRunner.Run("127.0.0.1", "Hello from server"));

            Task.WaitAll(new[] { t1, t2 });
            Console.ReadKey();
        }
    }
}
