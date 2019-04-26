using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TcpClientRunner client = new TcpClientRunner())
            {
                client.Run();
            }
        }
    }
}
