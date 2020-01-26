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
```

You must change the `<StartupObject>` starting entry point in the `.csproj` project file
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <StartupObject>SMon.TestService.SMon</StartupObject>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\SMon\SMon.csproj" />
  </ItemGroup>

</Project>
```

Samples
```sh
$ ./SMon.TestService
$ sudo ./SMon.TestService install
$ sudo ./SMon.TestService uninstall
$ sudo ./SMon.TestService start
$ sudo ./SMon.TestService stop
```
