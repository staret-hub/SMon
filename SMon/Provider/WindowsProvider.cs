using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SMon.Provider
{
    /// <summary>
    /// Service Provider for Windows.
    /// Sc.exe is used internally and must be run with administrator privileges.
    /// 
    /// **The ability to log through event logs and the ability to select service execution accounts are not yet implemented and will be implemented soon.**
    /// </summary>
    public class WindowsProvider : IProvider
    {
        private ServiceSettings settings;

        /// <summary>
        /// Prefixed to the service name, meaning dotnet execution service.
        /// </summary>
        public static readonly string ServiceNamePrefix = "dotnet-";

        //private static readonly string ScFilepath = $@"{Environment.SystemDirectory}\sc.exe";
        private static readonly string ScFilepath = "sc";
        private static readonly string NetFilepath = "net";

        private string ServiceName => $"{ServiceNamePrefix}{settings.ServiceName}";

        public WindowsProvider(ServiceSettings settings)
        {
            this.settings = settings;
        }

        public void Install(string[] args)
        {
            var path = Assembly.GetEntryAssembly().Location;
            // if .dll
            if (path.EndsWith(".exe") == false)
                path = $"dotnet {path}";

            var filename = ScFilepath;
            var arguments = $@"create {ServiceName} binpath=""{path} {string.Join(' ', args)}""";
            Process.Start(filename, arguments).WaitForExit();
        }

        public void Uninstall()
        {
            var filename = ScFilepath;
            var arguments = $@"delete {ServiceName}";
            Process.Start(filename, arguments).WaitForExit();
        }

        public void Restart()
        {
            Process.Start(NetFilepath, $"stop {ServiceName}").WaitForExit();
            Start();
        }

        public void Start()
        {
            var filename = ScFilepath;
            var arguments = $@"start {ServiceName}";
            Process.Start(filename, arguments).WaitForExit();
        }

        public void Stop()
        {
            var filename = ScFilepath;
            var arguments = $@"stop {ServiceName}";
            Process.Start(filename, arguments).WaitForExit();
        }
    }
}
