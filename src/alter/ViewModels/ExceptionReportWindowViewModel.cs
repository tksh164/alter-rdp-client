﻿using System;
using System.Threading;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlterApp.Services.Interfaces;
using AlterApp.Models;

namespace AlterApp.ViewModels
{
    internal partial class ExceptionReportWindowViewModel : ObservableObject
    {
        private readonly IExceptionReportWindowViewModelService _viewModelService;

        public ExceptionReportWindowViewModel(IExceptionReportWindowViewModelService viewModelService)
        {
            _viewModelService = viewModelService;
        }

        public string WindowTitle
        {
            get => _viewModelService.GetWindowTitle();
        }

        public string AppName
        {
            get => AppConstants.AppName;
        }

        private const string _copyButtonCaptionForBeforeClick = "Copy the exception report to the clipboard";

        private string _copyButtonContentText = _copyButtonCaptionForBeforeClick;
        public string CopyButtonContentText
        {
            get => _copyButtonContentText;
            private set => SetProperty(ref _copyButtonContentText, value);
        }

        [ObservableProperty]
        private string _reportContentText = string.Empty;

        [RelayCommand()]
        private void OpenProjectWebsite()
        {
            _viewModelService.OpenIssueReportUri();
        }

        [RelayCommand()]
        private void CopyReportContentTextToClipboard()
        {
            Clipboard.SetText(ReportContentText);

            const string copyButtonCaptionForAfterClick = "Copied!";
            CopyButtonContentText = copyButtonCaptionForAfterClick;

            // Create a timer to return back the button's caption then start the timer with specified due time.
            var captionRestoreTimer = new Timer((state) => {
                CopyButtonContentText = _copyButtonCaptionForBeforeClick;
                var timer = state as Timer;  // "state" is a newly created Timer object (The timer itself).
                timer?.Dispose();
            });
            const int restMilliseconds = 5000;
            captionRestoreTimer.Change(restMilliseconds, Timeout.Infinite);
        }

        [RelayCommand()]
        private void ExitApplication()
        {
            _viewModelService.TerminateApplicationWithErrorExitCode();
        }

        public void OnWindowClosed(object? sender, EventArgs e)
        {
            ExitApplication();
        }
    }
}
