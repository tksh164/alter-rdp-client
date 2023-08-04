﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using MsRdcAx.AxMsTscLib;

namespace MsRdcAx
{
    public class RdpClientHost : WindowsFormsHost, IDisposable
    {
        private AxRdpClient? _axRdpClient = null;

        public RdpClientHost() : base()
        {
        }

        public new void Dispose()
        {
            _axRdpClient?.Dispose();
            base.Dispose();
        }

        public string RemoteServer { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public int DesktopWidth { get; set; } = 0;

        public int DesktopHeight { get; set; } = 0;

        public bool IsLoginCompleted { get; private set; } = false;

        public RdpClientConnectionStatus ConnectionStatus
        {
            get
            {
                return _axRdpClient == null ? RdpClientConnectionStatus.NotConnected : ConvertEnumValue.To<RdpClientConnectionStatus>(_axRdpClient.Connected);
            }
        }

        public RdpClientDisconnectReason LastDisconnectReason { get; private set; } = new();

        public void Connect()
        {
            InitializeRdpClientActiveXControl();
            InitializeRdpClientAxEventHandlers();
            InitializeRdpClientAxSettings();
            LastDisconnectReason = new();
            _axRdpClient!.Connect();
        }

        private void InitializeRdpClientActiveXControl()
        {
            _axRdpClient = new AxRdpClient();
            _axRdpClient.BeginInit();
            _axRdpClient.EndInit();

            this.BeginInit();
            this.Child = _axRdpClient;
            this.EndInit();

            _axRdpClient.Visible = false;
            _axRdpClient.CreateControl();
        }

        private void InitializeRdpClientAxEventHandlers()
        {
            if (_axRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            _axRdpClient.OnConnecting += AxRdpClient_OnConnecting;
            _axRdpClient.OnAuthenticationWarningDisplayed += AxRdpClient_OnAuthenticationWarningDisplayed;
            _axRdpClient.OnAuthenticationWarningDismissed += AxRdpClient_OnAuthenticationWarningDismissed;
            _axRdpClient.OnUserNameAcquired += AxRdpClient_OnUserNameAcquired;
            _axRdpClient.OnConnected += AxRdpClient_OnConnected;
            _axRdpClient.OnLoginComplete += AxRdpClient_OnLoginComplete;
            _axRdpClient.OnLogonError += AxRdpClient_OnLogonError;
            _axRdpClient.OnReceivedTSPublicKey += AxRdpClient_OnReceivedTSPublicKey;

            _axRdpClient.OnDisconnected += AxRdpClient_OnDisconnected;
            _axRdpClient.OnConfirmClose += AxRdpClient_OnConfirmClose;

            _axRdpClient.Resize += AxRdpClient_Resize;

            _axRdpClient.OnNetworkStatusChanged += AxRdpClient_OnNetworkStatusChanged;
            _axRdpClient.OnAutoReconnecting += AxRdpClient_OnAutoReconnecting;
            _axRdpClient.OnAutoReconnecting2 += AxRdpClient_OnAutoReconnecting2;
            _axRdpClient.OnAutoReconnected += AxRdpClient_OnAutoReconnected;

            _axRdpClient.OnFocusReleased += AxRdpClient_OnFocusReleased;
            _axRdpClient.OnMouseInputModeChanged += AxRdpClient_OnMouseInputModeChanged;

            _axRdpClient.OnRemoteDesktopSizeChange += AxRdpClient_OnRemoteDesktopSizeChange;
            _axRdpClient.OnEnterFullScreenMode += AxRdpClient_OnEnterFullScreenMode;
            _axRdpClient.OnLeaveFullScreenMode += AxRdpClient_OnLeaveFullScreenMode;
            _axRdpClient.OnRequestGoFullScreen += AxRdpClient_OnRequestGoFullScreen;
            _axRdpClient.OnRequestLeaveFullScreen += AxRdpClient_OnRequestLeaveFullScreen;

            _axRdpClient.OnConnectionBarPullDown += AxRdpClient_OnConnectionBarPullDown;
            _axRdpClient.OnDevicesButtonPressed += AxRdpClient_OnDevicesButtonPressed;
            _axRdpClient.OnRequestContainerMinimize += AxRdpClient_OnRequestContainerMinimize;

            _axRdpClient.OnChannelReceivedData += AxRdpClient_OnChannelReceivedData;
            _axRdpClient.OnIdleTimeoutNotification += AxRdpClient_OnIdleTimeoutNotification;
            _axRdpClient.OnServiceMessageReceived += AxRdpClient_OnServiceMessageReceived;

            _axRdpClient.OnWarning += AxRdpClient_OnWarning;
            _axRdpClient.OnFatalError += AxRdpClient_OnFatalError;

            // OnRemoteProgramDisplayed  // For RemoteApp
            // OnRemoteProgramResult     // For RemoteApp
            // OnRemoteWindowDisplayed   // For RemoteApp
        }

        private void InitializeRdpClientAxSettings()
        {
            if (_axRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            _axRdpClient!.Server = RemoteServer;
            _axRdpClient.UserName = UserName;
            _axRdpClient.DesktopWidth = DesktopWidth;
            _axRdpClient.DesktopHeight = DesktopHeight;

            _axRdpClient.AdvancedSettings9.EnableCredSspSupport = true;
        }

        public void Disconnect()
        {
            _axRdpClient?.Disconnect();
        }

        private void UpdateSessionDisplaySettings()
        {
            if (_axRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            var updateSessionDisplaySettingsInternal = () =>
            {
                const double NonscaledDpi = 96.0;  // DPI for 100%
                uint desktopScaleFactor = (uint)((((double)_axRdpClient.DeviceDpi) / NonscaledDpi) * 100.0);
                Debug.WriteLine("desktopScaleFactor: {0}", desktopScaleFactor);

                const double OneInchInMillimeter = 25.4;
                uint desktopWidth = (uint)_axRdpClient.Width;
                uint desktopHeight = (uint)_axRdpClient.Height;
                uint physicalWidth = (uint)(((double)desktopWidth / desktopScaleFactor) * OneInchInMillimeter);
                uint physicalHeight = (uint)(((double)desktopHeight / desktopScaleFactor) * OneInchInMillimeter);
                Debug.WriteLine("physicalWidth: {0}", physicalWidth);
                Debug.WriteLine("physicalHeight: {0}", physicalHeight);

                const uint DeviceScaleFactor = 100;
                const uint Orientation = 0;
                _axRdpClient.UpdateSessionDisplaySettings(desktopWidth, desktopHeight, physicalWidth, physicalHeight, Orientation, desktopScaleFactor, DeviceScaleFactor);
            };

            try
            {
                updateSessionDisplaySettingsInternal();
            }
            catch (COMException ex)
            {
                Debug.WriteLine("COMException: {0}", ex.HResult);

                // Retry after waiting.
                Task.Run(() => {
                    Thread.Sleep(2000);
                    updateSessionDisplaySettingsInternal();
                });
            }
        }

        public event EventHandler? OnConnecting;

        private void AxRdpClient_OnConnecting(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnConnecting");

            // Do hidden the WindowsFormsHost element while the RDP client establishing a connection
            // for showing the connecting status message that at under the WindowsFormsHost element.
            // If did hidden the WindowsFormsHost element at initial time, the credential prompt
            // window does not showing up the center of the main window.
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () => {
                this.Visibility = Visibility.Hidden;
            });

            OnConnecting?.Invoke(sender, e);
        }

        public event EventHandler? OnAuthenticationWarningDisplayed;

        private void AxRdpClient_OnAuthenticationWarningDisplayed(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnAuthenticationWarningDisplayed");
            OnAuthenticationWarningDisplayed?.Invoke(sender, e);
        }

        public event EventHandler? OnAuthenticationWarningDismissed;

        private void AxRdpClient_OnAuthenticationWarningDismissed(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnAuthenticationWarningDismissed");
            OnAuthenticationWarningDismissed?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnUserNameAcquiredEventHandler? OnUserNameAcquired;

        private void AxRdpClient_OnUserNameAcquired(object? sender, IMsTscAxEvents_OnUserNameAcquiredEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnUserNameAcquired");
            OnUserNameAcquired?.Invoke(sender, e);
        }

        public event EventHandler? OnConnected;

        private void AxRdpClient_OnConnected(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnConnected");
            if (_axRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            // Do visible the WindowsFormsHost element that did hidden in the OnConnecting event handler.
            this.Visibility = Visibility.Visible;
            _axRdpClient.Visible = true;

            OnConnected?.Invoke(sender, e);
        }

        public event EventHandler? OnLoginComplete;

        private void AxRdpClient_OnLoginComplete(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnLoginComplete");
            IsLoginCompleted = true;
            UpdateSessionDisplaySettings();
            OnLoginComplete?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnLogonErrorEventHandler? OnLogonError;

        private void AxRdpClient_OnLogonError(object? sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnLogonError");
            OnLogonError?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnReceivedTSPublicKeyEventHandler? OnReceivedTSPublicKey;

        private void AxRdpClient_OnReceivedTSPublicKey(object? sender, IMsTscAxEvents_OnReceivedTSPublicKeyEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnReceivedTSPublicKey");
            OnReceivedTSPublicKey?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnDisconnectedEventHandler? OnDisconnected;

        private void AxRdpClient_OnDisconnected(object? sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnDisconnected");
            if (_axRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            LastDisconnectReason = new RdpClientDisconnectReason(e.discReason, _axRdpClient.ExtendedDisconnectReason);
            IsLoginCompleted = false;
            OnDisconnected?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnConfirmCloseEventHandler? OnConfirmClose;

        private void AxRdpClient_OnConfirmClose(object? sender, IMsTscAxEvents_OnConfirmCloseEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnConfirmClose");
            OnConfirmClose?.Invoke(sender, e);
        }

        private void AxRdpClient_Resize(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_Resize");
            if (_axRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            // UpdateSessionDisplaySettings works after complete login only.
            if (IsLoginCompleted)
            {
                var onResizeState = (DelayMilliseconds: 300, ClientSizeWidthAtFired: _axRdpClient.ClientSize.Width, ClientSizeHeightAtFired: _axRdpClient.ClientSize.Height);
                Task.Factory.StartNew((object? onResizeState) =>
                {
                    ArgumentNullException.ThrowIfNull(nameof(onResizeState));

                    var (delayMilliseconds, clientSizeWidthAtFired, clientSizeHeightAtFired) = ((int, int, int))onResizeState;
                    Thread.Sleep(delayMilliseconds);
                    if (clientSizeWidthAtFired == _axRdpClient.ClientSize.Width && clientSizeHeightAtFired == _axRdpClient.ClientSize.Height)
                    {
                        Debug.WriteLine("Resize completed.");
                        UpdateSessionDisplaySettings();
                    }
                }, onResizeState);
            }
        }

        public event IMsTscAxEvents_OnNetworkStatusChangedEventHandler? OnNetworkStatusChanged;

        private void AxRdpClient_OnNetworkStatusChanged(object? sender, IMsTscAxEvents_OnNetworkStatusChangedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnNetworkStatusChanged");
            OnNetworkStatusChanged?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnAutoReconnectingEventHandler? OnAutoReconnecting;

        private void AxRdpClient_OnAutoReconnecting(object? sender, IMsTscAxEvents_OnAutoReconnectingEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnAutoReconnecting");
            OnAutoReconnecting?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnAutoReconnecting2EventHandler? OnAutoReconnecting2;

        private void AxRdpClient_OnAutoReconnecting2(object? sender, IMsTscAxEvents_OnAutoReconnecting2Event e)
        {
            Debug.WriteLine("AxRdpClient_OnAutoReconnecting2");
            OnAutoReconnecting2?.Invoke(sender, e);
        }

        public event EventHandler? OnAutoReconnected;

        private void AxRdpClient_OnAutoReconnected(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnAutoReconnected");
            OnAutoReconnected?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnFocusReleasedEventHandler? OnFocusReleased;

        private void AxRdpClient_OnFocusReleased(object? sender, IMsTscAxEvents_OnFocusReleasedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnFocusReleased");
            OnFocusReleased?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnMouseInputModeChangedEventHandler? OnMouseInputModeChanged;

        private void AxRdpClient_OnMouseInputModeChanged(object? sender, IMsTscAxEvents_OnMouseInputModeChangedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnMouseInputModeChanged");
            OnMouseInputModeChanged?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnRemoteDesktopSizeChangeEventHandler? OnRemoteDesktopSizeChange;

        private void AxRdpClient_OnRemoteDesktopSizeChange(object? sender, IMsTscAxEvents_OnRemoteDesktopSizeChangeEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnRemoteDesktopSizeChange");
            OnRemoteDesktopSizeChange?.Invoke(sender, e);
        }

        public event EventHandler? OnEnterFullScreenMode;

        private void AxRdpClient_OnEnterFullScreenMode(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnEnterFullScreenMode");
            OnEnterFullScreenMode?.Invoke(sender, e);
        }

        public event EventHandler? OnLeaveFullScreenMode;

        private void AxRdpClient_OnLeaveFullScreenMode(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnLeaveFullScreenMode");
            OnLeaveFullScreenMode?.Invoke(sender, e);
        }

        public event EventHandler? OnRequestGoFullScreen;

        private void AxRdpClient_OnRequestGoFullScreen(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnRequestGoFullScreen");
            OnRequestGoFullScreen?.Invoke(sender, e);
        }

        public event EventHandler? OnRequestLeaveFullScreen;

        private void AxRdpClient_OnRequestLeaveFullScreen(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnRequestLeaveFullScreen");
            OnRequestLeaveFullScreen?.Invoke(sender, e);
        }

        public event EventHandler? OnConnectionBarPullDown;

        private void AxRdpClient_OnConnectionBarPullDown(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnConnectionBarPullDown");
            OnConnectionBarPullDown?.Invoke(sender, e);
        }

        public event EventHandler? OnDevicesButtonPressed;

        private void AxRdpClient_OnDevicesButtonPressed(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnDevicesButtonPressed");
            OnDevicesButtonPressed?.Invoke(sender, e);
        }

        public event EventHandler? OnRequestContainerMinimize;

        private void AxRdpClient_OnRequestContainerMinimize(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnRequestContainerMinimize");
            OnRequestContainerMinimize?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnChannelReceivedDataEventHandler? OnChannelReceivedData;

        private void AxRdpClient_OnChannelReceivedData(object? sender, IMsTscAxEvents_OnChannelReceivedDataEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnChannelReceivedData");
            OnChannelReceivedData?.Invoke(sender, e);
        }

        public event EventHandler? OnIdleTimeoutNotification;

        private void AxRdpClient_OnIdleTimeoutNotification(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnIdleTimeoutNotification");
            OnIdleTimeoutNotification?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnServiceMessageReceivedEventHandler? OnServiceMessageReceived;

        private void AxRdpClient_OnServiceMessageReceived(object? sender, IMsTscAxEvents_OnServiceMessageReceivedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnServiceMessageReceived");
            OnServiceMessageReceived?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnWarningEventHandler? OnWarning;

        private void AxRdpClient_OnWarning(object? sender, IMsTscAxEvents_OnWarningEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnWarning");
            OnWarning?.Invoke(sender, e);
        }

        public event IMsTscAxEvents_OnFatalErrorEventHandler? OnFatalError;

        private void AxRdpClient_OnFatalError(object? sender, IMsTscAxEvents_OnFatalErrorEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnFatalError");
            IsLoginCompleted = false;
            OnFatalError?.Invoke(sender, e);
        }
    }
}