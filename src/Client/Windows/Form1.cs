using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Win32;
using Shared;
using Shared.Models;

namespace Client.Windows;

public partial class Form1 : Form {
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);

    private ApiClient apiClient;
    private ClientWebSocketConnection connection;
    private bool handRaised = false;
    private bool screenOn = true;

    public Form1()
    {
        InitializeComponent();
        loginButton.Click += loginButton_Click;
        logoutButton.Click += logoutButton_Click;
        raiseHandButton.Click += raiseHandButton_Click;
        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

        apiClient = new ApiClient();
    }

    private void loginButton_Click(object? sender, EventArgs e) {
        Connect();
    }

    private void logoutButton_Click(object? sender, EventArgs e) {
        Disconnect();
    }

    private void raiseHandButton_Click(object? sender, EventArgs e) {
        ToggleHandRaised();
    }

    private void SystemEvents_SessionSwitch(object? sender, EventArgs e) {
        if (!screenOn)
            SetScreenStatus(false);
    }

    private void Connect() {
        var username = usernameTextBox.Text;
        var password = passwordTextBox.Text;
        var token = apiClient.LoginAsync(username, password).Result;

        if (token == string.Empty) {
            Console.WriteLine("Login failed");
            return;
        }

        Console.WriteLine("Connecting...");
        connection = apiClient.ConnectAsync().Result;
        Console.WriteLine("Connected!");

        connection.OnMessageReceived += OnMessageReceived;
        _ = connection.StartListeningAsync();

        connection.SendMessageAsync(Json.Serialize(new AuthMessage {
            AccessToken = token
        })).Wait();

        SwitchPanel();
    }

    private void OnMessageReceived(string message) {
        var messageObject = Json.Deserialize<Shared.Models.Message>(message);

        if (messageObject is ScreenStatusMessage screenStatus) {
            SetScreenStatus(screenStatus.TurnedOn);
        }
    }

    private void SwitchPanel() {
        loginPanel.Visible = !loginPanel.Visible;
        dashboardPanel.Visible = !dashboardPanel.Visible;
    }

    private void Disconnect() {
        _ = apiClient.LogoutAsync();
        _ = connection.Terminate();
        SwitchPanel();
    }

    private void ToggleHandRaised() {
        handRaised = !handRaised;
        var message = new HandStatusMessage {
            Raised = handRaised
        };

        connection.SendMessageAsync(Json.Serialize(message)).Wait();
        raiseHandButton.Text = handRaised ? "Lower Hand" : "Raise Hand";
    }

    private const int HWND_BROADCAST = 0xffff;
    private const int WM_SYSCOMMAND = 0x0112;
    private const int SC_MONITORPOWER = 0xf170;
    private const int MONITOR_ON = -1;
    private const int MONITOR_OFF = 2;
    private const int MOUSEEVENTF_MOVE = 0x0001;
    private void SetScreenStatus(bool turnedOn) {
        lock (this) {
            SendMessage(GetForegroundWindow(), WM_SYSCOMMAND, SC_MONITORPOWER, turnedOn ? MONITOR_ON : MONITOR_OFF);
            if (turnedOn) {
                mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);
                Thread.Sleep(50);
                mouse_event(MOUSEEVENTF_MOVE, 0, -1, 0, UIntPtr.Zero);
            }
        }
    }
}
