using SMon.Provider;
using System;
using System.Threading;

namespace SMon.TestService
{
    class Program
    {
        public static void Main(string[] args)
        {
            var settings = new ServiceSettings
            {
                ServiceName = "SMon.TestService",
                //StartupMain = "SMon.TestService.Program",
                Description = "SMon Test Service",
                //NetDLLPath = Type.GetType(settings.StartupMain).Assembly.Location,
                //OSName = Environment.OSVersion.ToString()
            };

            SMonHost.Run(args =>
            {
                Console.WriteLine("Started TestService Main!");

                while (true)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(DateTime.Now);
                }
            }, args, settings);
        }
    }

    class SMon
    {

    }
}
