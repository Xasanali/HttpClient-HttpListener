using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using HttpMultipartParser;
using System.Text;

namespace ServerSide
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start Project...");
            Console.WriteLine("For work as Server Side choose #1");
            Console.WriteLine("For work as Client Side choose #2");
            Console.WriteLine("For work as Chat choose #3");
            string selection = Console.ReadLine();

            switch (selection)
            {
                case "1":
                    {
                        ServerSide serverSide = new ServerSide();
                        serverSide.StartAsServer().GetAwaiter().GetResult();
                        break;
                    };
                case "2":
                    {
                        ClientSide clientSide = new ClientSide();
                        clientSide.StartAsClient().GetAwaiter().GetResult();
                        break;
                    };
                case "3":
                    {
                        // To Do Chat Functional
                        return;
                    };
                default:
                    {
                        ClientSide clientSide = new ClientSide();
                        clientSide.StartAsClient().GetAwaiter().GetResult();
                        break;
                    };
            }
        }


    }
}
