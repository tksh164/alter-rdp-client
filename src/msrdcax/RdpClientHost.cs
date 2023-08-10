using System;
using System.Windows.Forms.Integration;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MsRdcAx.AxMsTscLib;

namespace MsRdcAx
{
    public class RdpClientHost : WindowsFormsHost, IDisposable
    {
        private AxMsRdpClient? _axMsRdpClient = null;

        public RdpClientHost() : base()
        {
        }

        public new void Dispose()
        {
            _axMsRdpClient?.Dispose();
            base.Dispose();
        }

        public string RemoteComputer { get; set; } = string.Empty;

        public int RemotePort { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int DesktopWidth { get; set; } = 0;

        public int DesktopHeight { get; set; } = 0;

        public bool IsLoginCompleted { get; private set; } = false;

        public RdpClientConnectionStatus ConnectionStatus
        {
            get
            {
                return _axMsRdpClient == null ? RdpClientConnectionStatus.NotConnected : ConvertEnumValue.To<RdpClientConnectionStatus>(_axMsRdpClient.Connected);
            }
        }

        public RdpClientDisconnectReason LastDisconnectReason { get; private set; } = new();

        public void Connect()
        {
            InitializeRdpClientActiveXControl();
            SetupRdpClientAxEventHandlers();
            SetupRdpClientAxSettings();
            LastDisconnectReason = new();
            _axMsRdpClient!.Connect();
        }

        private void InitializeRdpClientActiveXControl()
        {
            // Create and initialize the RDP client ActiveX control.
            _axMsRdpClient = new AxMsRdpClient();
            _axMsRdpClient.BeginInit();
            _axMsRdpClient.EndInit();

            // Set the RDP client ActiveX control as a child element of WindowsFormsHost.
            this.BeginInit();
            this.Child = _axMsRdpClient;
            this.EndInit();

            // Create the RDP client ActiveX control's visible controls.
            _axMsRdpClient.CreateControl();
        }

        private void SetupRdpClientAxEventHandlers()
        {
            if (_axMsRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            // Connecting
            _axMsRdpClient.OnConnecting += AxRdpClient_OnConnecting;
            _axMsRdpClient.OnAuthenticationWarningDisplayed += AxRdpClient_OnAuthenticationWarningDisplayed;
            _axMsRdpClient.OnAuthenticationWarningDismissed += AxRdpClient_OnAuthenticationWarningDismissed;
            _axMsRdpClient.OnUserNameAcquired += AxRdpClient_OnUserNameAcquired;
            _axMsRdpClient.OnConnected += AxRdpClient_OnConnected;
            _axMsRdpClient.OnLoginComplete += AxRdpClient_OnLoginComplete;
            _axMsRdpClient.OnLogonError += AxRdpClient_OnLogonError;
            _axMsRdpClient.OnReceivedTSPublicKey += AxRdpClient_OnReceivedTSPublicKey;

            // Disconnecting
            _axMsRdpClient.OnDisconnected += AxRdpClient_OnDisconnected;
            _axMsRdpClient.OnConfirmClose += AxRdpClient_OnConfirmClose;

            // Networking
            _axMsRdpClient.OnNetworkStatusChanged += AxRdpClient_OnNetworkStatusChanged;
            _axMsRdpClient.OnAutoReconnecting += AxRdpClient_OnAutoReconnecting;
            _axMsRdpClient.OnAutoReconnecting2 += AxRdpClient_OnAutoReconnecting2;
            _axMsRdpClient.OnAutoReconnected += AxRdpClient_OnAutoReconnected;

            // Screen resizing
            _axMsRdpClient.Resize += AxRdpClient_Resize;
            _axMsRdpClient.OnRemoteDesktopSizeChange += AxRdpClient_OnRemoteDesktopSizeChange;
            _axMsRdpClient.OnEnterFullScreenMode += AxRdpClient_OnEnterFullScreenMode;
            _axMsRdpClient.OnLeaveFullScreenMode += AxRdpClient_OnLeaveFullScreenMode;
            _axMsRdpClient.OnRequestGoFullScreen += AxRdpClient_OnRequestGoFullScreen;
            _axMsRdpClient.OnRequestLeaveFullScreen += AxRdpClient_OnRequestLeaveFullScreen;

            // UI
            _axMsRdpClient.OnFocusReleased += AxRdpClient_OnFocusReleased;
            _axMsRdpClient.OnMouseInputModeChanged += AxRdpClient_OnMouseInputModeChanged;
            _axMsRdpClient.OnConnectionBarPullDown += AxRdpClient_OnConnectionBarPullDown;
            _axMsRdpClient.OnDevicesButtonPressed += AxRdpClient_OnDevicesButtonPressed;
            _axMsRdpClient.OnRequestContainerMinimize += AxRdpClient_OnRequestContainerMinimize;
            _axMsRdpClient.OnIdleTimeoutNotification += AxRdpClient_OnIdleTimeoutNotification;

            // Data communication
            _axMsRdpClient.OnChannelReceivedData += AxRdpClient_OnChannelReceivedData;
            _axMsRdpClient.OnServiceMessageReceived += AxRdpClient_OnServiceMessageReceived;

            // Error
            _axMsRdpClient.OnWarning += AxRdpClient_OnWarning;
            _axMsRdpClient.OnFatalError += AxRdpClient_OnFatalError;

            // RemoteApp
            // OnRemoteProgramDisplayed
            // OnRemoteProgramResult
            // OnRemoteWindowDisplayed
        }

        private void SetupRdpClientAxSettings()
        {
            if (_axMsRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            _axMsRdpClient.Server = RemoteComputer;
            _axMsRdpClient.AdvancedSettings2.RDPPort = RemotePort;
            _axMsRdpClient.UserName = UserName;
            _axMsRdpClient.AdvancedSettings9.EnableCredSspSupport = true;

            var displayScaleFactor = GetDisplayScaleFactor(_axMsRdpClient.DeviceDpi);
            _axMsRdpClient.DesktopWidth = (int)(DesktopWidth * displayScaleFactor);
            _axMsRdpClient.DesktopHeight = (int)(DesktopHeight * displayScaleFactor);
        }

        public void Disconnect()
        {
            _axMsRdpClient?.Disconnect();
        }

        public event EventHandler? OnConnecting;

        private void AxRdpClient_OnConnecting(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnConnecting");
            OnConnecting?.Invoke(this, e);
        }

        public event EventHandler? OnAuthenticationWarningDisplayed;

        private void AxRdpClient_OnAuthenticationWarningDisplayed(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnAuthenticationWarningDisplayed");
            OnAuthenticationWarningDisplayed?.Invoke(this, e);
        }

        public event EventHandler? OnAuthenticationWarningDismissed;

        private void AxRdpClient_OnAuthenticationWarningDismissed(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnAuthenticationWarningDismissed");
            OnAuthenticationWarningDismissed?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnUserNameAcquiredEventHandler? OnUserNameAcquired;

        private void AxRdpClient_OnUserNameAcquired(object? sender, IMsTscAxEvents_OnUserNameAcquiredEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnUserNameAcquired");
            OnUserNameAcquired?.Invoke(this, e);
        }

        public event EventHandler? OnConnected;

        private void AxRdpClient_OnConnected(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnConnected");
            OnConnected?.Invoke(this, e);
        }

        public event EventHandler? OnLoginComplete;

        private void AxRdpClient_OnLoginComplete(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnLoginComplete");
            IsLoginCompleted = true;
            UpdateSessionDisplaySettingsWithRetry();
            OnLoginComplete?.Invoke(this, e);
        }

        private void UpdateSessionDisplaySettingsWithRetry()
        {
            if (_axMsRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            try
            {
                UpdateSessionDisplaySettings();
            }
            catch (COMException ex)
            {
                Debug.WriteLine("COMException: {0}", ex.HResult);

                // Some times UpdateSessionDisplaySettings() throws a COM exception with E_UNEXPECTED.
                const int E_UNEXPECTED = -2147418113;  // 0x8000ffff
                if (ex.HResult == E_UNEXPECTED)
                {
                    // Retry after the waiting.
                    Task.Run(() =>
                    {
                        Thread.Sleep(2000);
                        UpdateSessionDisplaySettingsWithRetry();
                    });
                    return;
                }

                throw;
            }
        }

        private void UpdateSessionDisplaySettings()
        {
            if (_axMsRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            var desktopWidth = (uint)_axMsRdpClient.Width;
            var desktopHeight = (uint)_axMsRdpClient.Height;
            var desktopScaleFactor = (uint)(GetDisplayScaleFactor(_axMsRdpClient.DeviceDpi) * 100.0);
            var physicalWidth = ConvertToPhysicalUnitSize(desktopWidth, desktopScaleFactor);
            var physicalHeight = ConvertToPhysicalUnitSize(desktopHeight, desktopScaleFactor);
            Debug.WriteLine("desktopWidth: {0}", desktopWidth);
            Debug.WriteLine("desktopHeight: {0}", desktopHeight);
            Debug.WriteLine("desktopScaleFactor: {0}", desktopScaleFactor);
            Debug.WriteLine("physicalWidth: {0}", physicalWidth);
            Debug.WriteLine("physicalHeight: {0}", physicalHeight);

            const uint deviceScaleFactor = 100;
            const uint orientation = 0;
            _axMsRdpClient.UpdateSessionDisplaySettings(desktopWidth, desktopHeight, physicalWidth, physicalHeight, orientation, desktopScaleFactor, deviceScaleFactor);
        }

        private static double GetDisplayScaleFactor(int deviceDpi)
        {
            const double nonScaledDpi = 96.0;  // DPI for 100%
            return deviceDpi / nonScaledDpi;
        }

        private static uint ConvertToPhysicalUnitSize(uint desktopSize, uint desktopScaleFactor)
        {
            const double oneInchInMillimeter = 25.4;
            return (uint)(desktopSize / desktopScaleFactor * oneInchInMillimeter);
        }

        public event IMsTscAxEvents_OnLogonErrorEventHandler? OnLogonError;

        private void AxRdpClient_OnLogonError(object? sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnLogonError");
            OnLogonError?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnReceivedTSPublicKeyEventHandler? OnReceivedTSPublicKey;

        private void AxRdpClient_OnReceivedTSPublicKey(object? sender, IMsTscAxEvents_OnReceivedTSPublicKeyEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnReceivedTSPublicKey");
            OnReceivedTSPublicKey?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnDisconnectedEventHandler? OnDisconnected;

        private void AxRdpClient_OnDisconnected(object? sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnDisconnected");
            if (_axMsRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            IsLoginCompleted = false;
            LastDisconnectReason = new RdpClientDisconnectReason(e.discReason, _axMsRdpClient.ExtendedDisconnectReason);
            // TODO: Get reason text
            OnDisconnected?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnConfirmCloseEventHandler? OnConfirmClose;

        private void AxRdpClient_OnConfirmClose(object? sender, IMsTscAxEvents_OnConfirmCloseEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnConfirmClose");
            OnConfirmClose?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnNetworkStatusChangedEventHandler? OnNetworkStatusChanged;

        private void AxRdpClient_OnNetworkStatusChanged(object? sender, IMsTscAxEvents_OnNetworkStatusChangedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnNetworkStatusChanged");
            OnNetworkStatusChanged?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnAutoReconnectingEventHandler? OnAutoReconnecting;

        private void AxRdpClient_OnAutoReconnecting(object? sender, IMsTscAxEvents_OnAutoReconnectingEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnAutoReconnecting");
            OnAutoReconnecting?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnAutoReconnecting2EventHandler? OnAutoReconnecting2;

        private void AxRdpClient_OnAutoReconnecting2(object? sender, IMsTscAxEvents_OnAutoReconnecting2Event e)
        {
            Debug.WriteLine("AxRdpClient_OnAutoReconnecting2");
            OnAutoReconnecting2?.Invoke(this, e);
        }

        public event EventHandler? OnAutoReconnected;

        private void AxRdpClient_OnAutoReconnected(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnAutoReconnected");
            OnAutoReconnected?.Invoke(this, e);
        }

        private void AxRdpClient_Resize(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_Resize");
            if (_axMsRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            // NOTE: UpdateSessionDisplaySettings only works after complete login.
            if (IsLoginCompleted)
            {
                const int delayToUpdateInMsec = 300;
                Task.Run(() => UpdateSessionDisplaySettingsOnResize(_axMsRdpClient.ClientSize.Width, _axMsRdpClient.ClientSize.Height, delayToUpdateInMsec));
            }
        }

        private void UpdateSessionDisplaySettingsOnResize(int clientSizeWidthAtFired, int clientSizeHeightAtFired, int delayToUpdateInMilliseconds)
        {
            if (_axMsRdpClient == null) throw new InvalidOperationException("The RDP client ActiveX control is not instantiated.");

            // Delay the session display settings update because continuously fired resize event during the window's edge dragging.
            Thread.Sleep(delayToUpdateInMilliseconds);

            // If the current client sizes are the same as the client sizes of this thread started, it means dragging of the window's edge is stopped.
            // Only call the UpdateSessionDisplaySettings when dragging of the window's edge is stopped.
            if (_axMsRdpClient.ClientSize.Width == clientSizeWidthAtFired && _axMsRdpClient.ClientSize.Height == clientSizeHeightAtFired)
            {
                Debug.WriteLine("Do resizing.");
                UpdateSessionDisplaySettingsWithRetry();
            }
            else
            {
                Debug.WriteLine("Skip resizing: {0}!={1}, {2}!={3}", _axMsRdpClient.ClientSize.Width, clientSizeWidthAtFired, _axMsRdpClient.ClientSize.Height, clientSizeHeightAtFired);
            }
        }

        public event IMsTscAxEvents_OnRemoteDesktopSizeChangeEventHandler? OnRemoteDesktopSizeChange;

        private void AxRdpClient_OnRemoteDesktopSizeChange(object? sender, IMsTscAxEvents_OnRemoteDesktopSizeChangeEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnRemoteDesktopSizeChange");
            OnRemoteDesktopSizeChange?.Invoke(this, e);
        }

        public event EventHandler? OnEnterFullScreenMode;

        private void AxRdpClient_OnEnterFullScreenMode(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnEnterFullScreenMode");
            OnEnterFullScreenMode?.Invoke(this, e);
        }

        public event EventHandler? OnLeaveFullScreenMode;

        private void AxRdpClient_OnLeaveFullScreenMode(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnLeaveFullScreenMode");
            OnLeaveFullScreenMode?.Invoke(this, e);
        }

        public event EventHandler? OnRequestGoFullScreen;

        private void AxRdpClient_OnRequestGoFullScreen(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnRequestGoFullScreen");
            OnRequestGoFullScreen?.Invoke(this, e);
        }

        public event EventHandler? OnRequestLeaveFullScreen;

        private void AxRdpClient_OnRequestLeaveFullScreen(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnRequestLeaveFullScreen");
            OnRequestLeaveFullScreen?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnFocusReleasedEventHandler? OnFocusReleased;

        private void AxRdpClient_OnFocusReleased(object? sender, IMsTscAxEvents_OnFocusReleasedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnFocusReleased");
            OnFocusReleased?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnMouseInputModeChangedEventHandler? OnMouseInputModeChanged;

        private void AxRdpClient_OnMouseInputModeChanged(object? sender, IMsTscAxEvents_OnMouseInputModeChangedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnMouseInputModeChanged");
            OnMouseInputModeChanged?.Invoke(this, e);
        }

        public event EventHandler? OnConnectionBarPullDown;

        private void AxRdpClient_OnConnectionBarPullDown(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnConnectionBarPullDown");
            OnConnectionBarPullDown?.Invoke(this, e);
        }

        public event EventHandler? OnDevicesButtonPressed;

        private void AxRdpClient_OnDevicesButtonPressed(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnDevicesButtonPressed");
            OnDevicesButtonPressed?.Invoke(this, e);
        }

        public event EventHandler? OnRequestContainerMinimize;

        private void AxRdpClient_OnRequestContainerMinimize(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnRequestContainerMinimize");
            OnRequestContainerMinimize?.Invoke(this, e);
        }

        public event EventHandler? OnIdleTimeoutNotification;

        private void AxRdpClient_OnIdleTimeoutNotification(object? sender, EventArgs e)
        {
            Debug.WriteLine("AxRdpClient_OnIdleTimeoutNotification");
            OnIdleTimeoutNotification?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnChannelReceivedDataEventHandler? OnChannelReceivedData;

        private void AxRdpClient_OnChannelReceivedData(object? sender, IMsTscAxEvents_OnChannelReceivedDataEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnChannelReceivedData");
            OnChannelReceivedData?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnServiceMessageReceivedEventHandler? OnServiceMessageReceived;

        private void AxRdpClient_OnServiceMessageReceived(object? sender, IMsTscAxEvents_OnServiceMessageReceivedEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnServiceMessageReceived");
            OnServiceMessageReceived?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnWarningEventHandler? OnWarning;

        private void AxRdpClient_OnWarning(object? sender, IMsTscAxEvents_OnWarningEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnWarning");
            OnWarning?.Invoke(this, e);
        }

        public event IMsTscAxEvents_OnFatalErrorEventHandler? OnFatalError;

        private void AxRdpClient_OnFatalError(object? sender, IMsTscAxEvents_OnFatalErrorEvent e)
        {
            Debug.WriteLine("AxRdpClient_OnFatalError");
            IsLoginCompleted = false;
            OnFatalError?.Invoke(this, e);
        }
    }
}
