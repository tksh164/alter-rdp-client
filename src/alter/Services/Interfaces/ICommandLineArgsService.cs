namespace AlterApp.Services.Interfaces
{
    internal interface ICommandLineArgsService
    {
        public bool ShouldShowUsage { get; }

        public string? RemoteComputer { get; }

        public string? RemotePort { get; }

        public string? UserName { get; }

        public string? ConnectionTitle { get; }

        public bool ShouldStartConnect { get; }
    }
}
