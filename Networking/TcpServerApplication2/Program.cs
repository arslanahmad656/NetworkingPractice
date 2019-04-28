using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServerApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            //var tasks = new[]
            //{
            //    Task.Run(() => new TcpApp.TcpServerRunner().Run()),
            //    Task.Run(() => new TcpApp.TcpClientRunner().Run())
            //};

            //Task.WaitAll(tasks);

            new AnotherClient().Run();
        }
    }
}
