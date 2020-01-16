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

    public class ServiceEntrance
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var settings = new ServiceSettings();
                settings.ServiceName = "SMon.TestService";
                settings.StartupMain = "SMon.TestService.Program";
                settings.Description = "SMon Test Service";
                settings.NetDLLPath = Type.GetType(settings.StartupMain).Assembly.Location;
                settings.OSName = Environment.OSVersion.ToString();

                IProvider provider;
                if (settings.OSName.Contains("Windows") == true)
                {   // ex: Microsoft Windows NT 6.2.9200.0
                    provider = new WindowsProvider();
                }
                else
                {   // ex: Unix 4.4.0.18362
                    provider = new LinuxProvider(settings);
                }

                switch (args[0])
                {
                    case "install":
                        provider.Install(args.Length > 1 ? args[1..^0] : null);
                        return;
                    case "uninstall":
                        provider.Uninstall();
                        return;
                    case "start":
                        provider.Start();
                        return;
                    case "stop":
                        provider.Stop();
                        return;
                    case "restart":
                        provider.Restart();
                        return;
                }
            }

            Program.Main(args);
        }
    }
}
