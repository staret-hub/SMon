using SMon.Provider;
using System;
using System.Threading;

namespace SMon.TestService
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Started TestService Main!");

            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine(DateTime.Now);
            }
        }
    }

    class SMon
    {
        static void Main(string[] args)
        {
            var settings = new ServiceSettings
            {
                ServiceName = "SMon.TestService",
                Description = "SMon Test Service"
            };

            SMonHost.Run(args => Program.Main(args), args, settings);
        }
    }
}
