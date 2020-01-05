using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SMon.Provider
{
    public class LinuxProvider : IProvider
    {
        private ServiceSettings settings;

        public LinuxProvider(ServiceSettings settings)
        {
            this.settings = settings;
        }


        public void Install(string[] args)
        {
            try
            {
                InstallService(settings.NetDLLPath, args, true);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Permission denied.(Hint: use sudo)");
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Start()
        {
            ControlService(settings.ServiceName, "start");
        }

        public void Stop()
        {
            ControlService(settings.ServiceName, "stop");
        }

        public void Uninstall()
        {
            try
            {
                InstallService(settings.NetDLLPath, null, false);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Permission denied.(Hint: use sudo)");
            }
        }

        static int InstallService(string netDllPath, string[] args, bool doInstall)
        {
            var dllFileName = Path.GetFileName(netDllPath);
            var osName = Environment.OSVersion.ToString();

            FileInfo fi = null;

            try
            {
                fi = new FileInfo(netDllPath);
            }
            catch { }

            if (doInstall == true && fi != null && fi.Exists == false)
            {
                //WriteLog("NOT FOUND: " + fi.FullName);
                return 1;
            }

            var serviceName = "dotnet-" + Path.GetFileNameWithoutExtension(dllFileName).ToLower();

            var exeName = Process.GetCurrentProcess().MainModule.FileName;

            var workingDir = Path.GetDirectoryName(fi.FullName);

            string serviceFilePath = $"/etc/systemd/system/{serviceName}.service";

            if (doInstall == true)
            {
                var execStart = "";
                if (exeName.EndsWith("dotnet") == true)
                    execStart = $"{exeName} {fi.FullName}";
                else
                    execStart = exeName;
                var exeArgs = string.Concat(args ?? new[] { "" });

                var fullText = $@"
[Unit]
Description={dllFileName} running on {osName}
[Service]
WorkingDirectory={workingDir}
ExecStart={execStart} {exeArgs}
KillSignal=SIGINT
SyslogIdentifier={serviceName}
[Install]
WantedBy=multi-user.target
";
                Console.WriteLine(fullText);

                File.WriteAllText(serviceFilePath, fullText);
                //WriteLog(serviceFilePath + " Created");
                ControlService(serviceName, "enable");
                ControlService(serviceName, "start");
            }
            else
            {
                if (File.Exists(serviceFilePath) == true)
                {
                    ControlService(serviceName, "stop");
                    File.Delete(serviceFilePath);
                    //WriteLog(serviceFilePath + " Deleted");
                }
            }

            return 0;
        }

        static int ControlService(string serviceName, string mode)
        {
            string servicePath = $"/etc/systemd/system/{serviceName}.service";

            if (File.Exists(servicePath) == false)
            {
                //WriteLog($"No service: {serviceName} to {mode}");
                return 1;
            }

            var psi = new ProcessStartInfo();
            psi.FileName = "systemctl";
            psi.Arguments = $"{mode} {serviceName}";
            var child = Process.Start(psi);
            child.WaitForExit();
            return child.ExitCode;
        }
    }
}
