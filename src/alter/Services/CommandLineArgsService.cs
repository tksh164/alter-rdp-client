using System;
using AlterApp.Services.Interfaces;

namespace AlterApp.Services
{
    internal class CommandLineArgsService
    {
        public CommandLineArgsService()
        {
            IsValidCommandLineArgs = false;
            RemoteComputer = null;
            RemotePort = null;
            UserName = null;
            ConnectionTitle = null;
            AutoConnect = false;
            ParseCommandLineArgs();
        }

        public bool IsValidCommandLineArgs { get; private set; }

        public string? RemoteComputer { get; private set; }

        public string? RemotePort { get; private set; }

        public string? UserName { get; private set; }

        public string? ConnectionTitle { get; private set; }

        public bool AutoConnect { get; private set; }

        private void ParseCommandLineArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2) return;  // No command line arguments.

            // ExeName -host x.x.x.x -port xxxx -username xxxx -title "xxxx" -autoconnect
            bool isValidAllArgs = true;

            for (int i = 1; i < args.Length; i++)
            {
                string lowerArgs = args[i].ToLower();

                // -host
                if (string.CompareOrdinal("-host", lowerArgs) == 0)
                {
                    int indexOfValue = i + 1;
                    if (indexOfValue >= args.Length)
                    {
                        isValidAllArgs = false;  // Argument value is missing.
                        break;
                    }
                    i++;

                    string argValue = args[indexOfValue];
                    if (ValidateRemoteComputerArg(RemoteComputer, argValue))
                    {
                        RemoteComputer = argValue;
                    }
                    else
                    {
                        isValidAllArgs = false;  // Argument is invalid.
                    }
                }

                // -port
                else if (string.CompareOrdinal("-port", lowerArgs) == 0)
                {
                    int indexOfValue = i + 1;
                    if (indexOfValue >= args.Length)
                    {
                        isValidAllArgs = false;  // Argument value is missing.
                        break;
                    }
                    i++;

                    string argValue = args[indexOfValue];
                    if (ValidateRemotePortArg(RemotePort, argValue))
                    {
                        RemotePort = argValue;
                    }
                    else
                    {
                        isValidAllArgs = false;  // Argument is invalid.
                    }
                }

                // -username
                else if (string.CompareOrdinal("-username", lowerArgs) == 0)
                {
                    int indexOfValue = i + 1;
                    if (indexOfValue >= args.Length)
                    {
                        isValidAllArgs = false;  // Argument value is missing.
                        break;
                    }
                    i++;

                    string argValue = args[indexOfValue];
                    if (ValidateUserNameArg(UserName, argValue))
                    {
                        UserName = argValue;
                    }
                    else
                    {
                        isValidAllArgs = false;  // Argument is invalid.
                    }
                }

                // -title
                else if (string.CompareOrdinal("-title", lowerArgs) == 0)
                {
                    int indexOfValue = i + 1;
                    if (indexOfValue >= args.Length)
                    {
                        isValidAllArgs = false;  // Argument value is missing.
                        break;
                    }
                    i++;

                    string argValue = args[indexOfValue];
                    if (ValidateConnectionTitleArg(ConnectionTitle, argValue))
                    {
                        ConnectionTitle = argValue;
                    }
                    else
                    {
                        isValidAllArgs = false;  // Argument is invalid.
                    }
                }

                // -autoconnect
                else if (string.CompareOrdinal("-autoconnect", lowerArgs) == 0)
                {
                    if (ValidateAutoConnectArg(AutoConnect))
                    {
                        AutoConnect = true;
                    }
                    else
                    {
                        isValidAllArgs = false;  // Argument is invalid.
                    }
                }

                // Unexpected argument.
                else
                {
                    isValidAllArgs = false;
                }
            }

            IsValidCommandLineArgs = isValidAllArgs;
        }

        private static bool ValidateRemoteComputerArg(string? currentArgValue, string newArgValue)
        {
            if (currentArgValue != null) return false;  // The argument value is already set.
            if (!ValidateRemoteComputerArgValue(newArgValue)) return false;  // The argument value is invalid.
            return true;
        }

        private static bool ValidateRemoteComputerArgValue(string argValue)
        {
            return argValue.IndexOf(' ') < 0;  // Whitespaces are not allowed in the computer name.
        }

        private static bool ValidateRemotePortArg(string? currentArgValue, string newArgValue)
        {
            if (currentArgValue != null) return false;  // The argument value is already set.
            if (!ValidateRemotePortArgValue(newArgValue)) return false;  // The argument value is invalid.
            return true;
        }

        private static bool ValidateRemotePortArgValue(string argValue)
        {
            bool parseResult = int.TryParse(argValue, out int port);
            return parseResult && port >= 1 && port <= 65535;  // The remote port is a number between 1 and 65535.
        }

        private static bool ValidateUserNameArg(string? currentArgValue, string newArgValue)
        {
            if (currentArgValue != null) return false;  // The argument value is already set.
            if (!ValidateUserNameArgValue(newArgValue)) return false;  // The argument value is invalid.
            return true;
        }

        private static bool ValidateUserNameArgValue(string argValue)
        {
            return true;
        }

        private static bool ValidateConnectionTitleArg(string? currentArgValue, string newArgValue)
        {
            if (currentArgValue != null) return false;  // The argument value is already set.
            if (!ValidateConnectionTitleArgValue(newArgValue)) return false;  // The argument value is invalid.
            return true;
        }

        private static bool ValidateConnectionTitleArgValue(string argValue)
        {
            return true;
        }

        private static bool ValidateAutoConnectArg(bool currentArgValue)
        {
            if (currentArgValue == true) return false;  // The argument value is already set.
            return true;
        }
    }
}
