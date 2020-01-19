SMon
--------------------
[![latest version](https://img.shields.io/nuget/v/SMon)](https://www.nuget.org/packages/SMon) [![preview version](https://img.shields.io/nuget/v/SMon)](https://www.nuget.org/packages/SMon/absoluteLatest) [![downloads](https://img.shields.io/nuget/dt/SMon)](https://www.nuget.org/packages/SMon)

SMon makes it easy to install the application as a service.

### Installation

SMon is available on [NuGet](https://www.nuget.org/packages/SMon). Install the provider package corresponding to your .NET Core Project.

```sh
dotnet add package SMon
```

### Usage
Sample code
```cs
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

```

Samples on Linux
```sh
$ ./SMon.TestService
$ sudo ./SMon.TestService install
$ sudo ./SMon.TestService uninstall
$ sudo ./SMon.TestService start
$ sudo ./SMon.TestService stop
```
