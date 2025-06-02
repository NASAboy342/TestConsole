using System.Net.Http;
using System.Net.WebSockets;
using System.Text;

namespace TestConsole.Programs
{
    public class WebSocketClient
    {
        public async Task ConnectToServer(string serverUri)
        {
            try
            {
                using (var clientWebSocket = new ClientWebSocket())
                {
                    await clientWebSocket.ConnectAsync(new Uri(serverUri),CancellationToken.None);
                    var connectionTimeSpend = TimeSpan.FromSeconds(0);
                    while (clientWebSocket.State == WebSocketState.Connecting && connectionTimeSpend <= TimeSpan.FromMinutes(1))
                    {
                        Console.WriteLine("Connecting to the server...");
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        connectionTimeSpend += TimeSpan.FromSeconds(1);
                    }
                    if (clientWebSocket.State == WebSocketState.Open)
                    {
                        Console.WriteLine("Connected to the server.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to connect to the server. {clientWebSocket.State}");
                        return;
                    }

                    // Send a message to the server
                    var message = "Hello from the client!";
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Receive messages from the server
                    var buffer = new byte[1024];
                    while (clientWebSocket.State == WebSocketState.Open)
                    {
                        var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            Console.WriteLine($"Received from server: {receivedMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to server: {ex}");
            }
            
        }
    }
}