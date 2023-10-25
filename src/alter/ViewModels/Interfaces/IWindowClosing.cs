namespace AlterApp.ViewModels.Interfaces
{
    internal interface IWindowClosing
    {
        /// <summary>
        /// Implements logic to execute when the window is closing.
        /// </summary>
        /// <returns>true if the window closing should be canceled; otherwise, false.</returns>
        public bool OnClosing();
    }
}
