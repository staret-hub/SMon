using SMon.Provider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SMon
{
    public static class SMonHost
    {
        private static readonly string ApplicationPath = Path.GetDirectoryName(typeof(SMonHost).Assembly.Location);
        private static readonly string ServiceSettingsFilepath = $@"{ApplicationPath}{Path.PathSeparator}Service.json";

        private static readonly string[] CommandArguments = { "install", "uninstall", "start", "stop", "restart" };

        public static void Run(Action<string[]> main, string[] args, ServiceSettings settings)
        {
            if (args.Length > 0 && CommandArguments.Any(x => x == args[0]) == true)
            {
                // Get the ServiceSettings instance from the service settings file.
                if (settings == null)
                {
                    try
                    {
                        var json = File.ReadAllText(ServiceSettingsFilepath);
                        settings = JsonSerializer.Deserialize<ServiceSettings>(json);
                    }
                    catch
                    {
                        throw new SMonException("The service settings file does not exist or is invalid.");
                    }
                }

                IProvider provider;
                //if (Settings.OSName.Contains("Windows") == true)
                //{   // ex: Microsoft Windows NT 6.2.9200.0
                //    provider = new WindowsProvider(Settings);
                //}
                //else
                //{   // ex: Unix 4.4.0.18362
                //    provider = new LinuxProvider(Settings);
                //}
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
                    provider = new WindowsProvider(settings);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) == true)
                    provider = new LinuxProvider(settings);
                else
                    throw new SMonException("OS platform not supported.");

                switch (args[0])
                {
                    case "install":
                        args[0] = "run";
                        provider.Install(args);
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

            //Console.CancelKeyPress += (s, e) =>
            //{
            //    e.Cancel = true;
            //};

            var asService = args.Length > 0 && args[0] == "run";
            if (asService == true)
                args = args[1..^0];
            if (asService == true && RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
            {
                var service = new SMonService(settings.ServiceName, () => main?.Invoke(args));
                ServiceBase.Run(service);
            }
            else if (asService == false || RuntimeInformation.IsOSPlatform(OSPlatform.Linux) == true)
            {
                main?.Invoke(args);
            }
        }
    }

    public class SMonException : Exception
    {
        public SMonException(string message) : base(message)
        {
        }

        public SMonException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    class SMonService : ServiceBase
    {
        private enum CtrlTypes : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

        private readonly Action worker;
        private Thread workerThread;

        public SMonService(string serviceName, Action worker)
        {
            this.ServiceName = serviceName;
            this.worker = worker;
        }

        protected override void OnStart(string[] args)
        {
            workerThread = new Thread(() =>
            {
                try
                {
                    worker?.Invoke();
                }
                catch
                {
                }
            })
            {
                IsBackground = false
            };
            workerThread.Start();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            Task.Run(() =>
            {

                Thread.Sleep(1000);
                Process.GetCurrentProcess().Kill();
            });

            base.OnStop();
        }
    }
}
