using System;

namespace AlterApp.Services
{
    internal class CommandLineArgsService
    {
        public CommandLineArgsService()
        {
            HasInvalidArgs = false;
            RemoteComputer = null;
            RemotePort = null;
            UserName = null;
            ConnectionTitle = null;
            ShouldAutomaticallyStartConnection = false;
            ParseCommandLineArgs();
        }

        public bool HasInvalidArgs { get; private set; }

        public string? RemoteComputer { get; private set; }

        public string? RemotePort { get; private set; }

        public string? UserName { get; private set; }

        public string? ConnectionTitle { get; private set; }

        public bool ShouldAutomaticallyStartConnection { get; private set; }

        private void ParseCommandLineArgs()
        {
            string[] args = Environment.GetCommandLineArgs();

            // No command line arguments.
            if (args.Length < 2) return;

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith("-host:", StringComparison.OrdinalIgnoreCase))
                {
                    RemoteComputer = GetArgValue(args[i]);
                }
                else if (args[i].StartsWith("-port:", StringComparison.OrdinalIgnoreCase))
                {
                    RemotePort = GetArgValue(args[i]);
                }
                else if (args[i].StartsWith("-username:", StringComparison.OrdinalIgnoreCase))
                {
                    UserName = GetArgValue(args[i]);
                }
                else if (args[i].StartsWith("-title:", StringComparison.OrdinalIgnoreCase))
                {
                    ConnectionTitle = GetArgValue(args[i]);
                }
                else if (args[i].Equals("-autoconnect", StringComparison.OrdinalIgnoreCase))
                {
                    ShouldAutomaticallyStartConnection = true;
                }
                else
                {
                    HasInvalidArgs = true;
                }
            }
        }

        private static string GetArgValue(string argNameValuePair)
        {
            return argNameValuePair[(argNameValuePair.IndexOf(':') + 1)..];
        }
    }
}
