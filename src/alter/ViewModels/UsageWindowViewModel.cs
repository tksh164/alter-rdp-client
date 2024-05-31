using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlterApp.Views.Interfaces;

namespace AlterApp.ViewModels
{
    internal partial class UsageWindowViewModel : ObservableObject
    {
        public UsageWindowViewModel()
        {
        }

        [RelayCommand()]
        private void CloseWindow(IClosable window)
        {
            window?.Close();
        }
    }
}
