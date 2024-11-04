using System.Net.WebSockets;
using System.Text;
using Client.Windows;

namespace Windows;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        loginButton.Click += Button1_Click;
    }

    private void Button1_Click(object? sender, EventArgs e) {
        Connect();
    }

    private void Connect() {
        var username = "admin";
        var password = "admin";

        Console.WriteLine("Connecting to the server...");

        Console.WriteLine("Logging in...");
        var apiClient = new ApiClient();
        apiClient.LoginAsync(username, password).Wait();

        Console.WriteLine("Connecting...");
        var connection = apiClient.ConnectAsync().Result;
        Console.WriteLine("Connected!");

        connection.SendAsync(Encoding.UTF8.GetBytes("Hello!"), WebSocketMessageType.Text, true, CancellationToken.None).Wait();

        var buffer = new byte[1024];
        var result = connection.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

        Console.WriteLine(message);

        try {
            connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).Wait();
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
