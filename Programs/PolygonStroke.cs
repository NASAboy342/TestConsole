using System;
using System.Net.WebSockets;
using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;

namespace TestConsole.Programs;

public class PolygonStroke
{
    private readonly Dictionary<EnumPolygonAction, string> _polygonRequestAction = new()
    {
        { EnumPolygonAction.Auth, "auth" },
        { EnumPolygonAction.Subscribe, "subscribe" },
        { EnumPolygonAction.Unsubscribe, "unsubscribe" }
    };
    private readonly string _polygonApiKey = "9QHpaU8QILopgobBs9xPEHqv6ggEWYAO";
    private readonly ClientWebSocket _clientWebSocket = new ClientWebSocket();
    internal async Task Run()
    {
        await OpenConnection();
        Console.WriteLine(await ListenToServer());

        await SendAuthRequest();
        Console.WriteLine(await ListenToServer());


        await SendDataSubscription();
        while (_clientWebSocket.State == WebSocketState.Open)
        {
            await Task.Delay(1000);
            var serverMessage = await ListenToServer();
            Console.WriteLine(serverMessage);
            Console.WriteLine("==============================");
            
        }

        await CloseConnection();

        Console.WriteLine("WebSocket connection closed.");
    }

    private async Task SendDataSubscription()
    {
        var subscribeRequest = new PolygonSubscribeRequest
        {
            Action = _polygonRequestAction[EnumPolygonAction.Subscribe],
            Params = "A.I:XAUUSD"
        };

        await SendRequest(subscribeRequest);
    }

    private async Task CloseConnection()
    {
        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }

    private async Task OpenConnection()
    {
        var polygonWsUrl = "wss://socket.polygon.io/indices";
        await _clientWebSocket.ConnectAsync(new Uri(polygonWsUrl), CancellationToken.None);
        if (_clientWebSocket.State != WebSocketState.Open)
        {
            Console.WriteLine($"WebSocket connection failed.{await ListenToServer()}");
            throw new Exception("WebSocket connection failed.");
        }
    }

    private async Task SendAuthRequest()
    {
        var authRequest = new PolygonAuthRequest
        {
            Action = _polygonRequestAction[EnumPolygonAction.Auth],
            Params = _polygonApiKey
        };
        Console.WriteLine($"Sending auth request...{JsonConvert.SerializeObject(authRequest)}");
        await SendRequest(authRequest);
    }

    private async Task SendRequest<T>(T request) where T : class
    {
        var requestJson = JsonConvert.SerializeObject(request);
        var requestBytes = System.Text.Encoding.UTF8.GetBytes(requestJson);
        var requestSegment = new ArraySegment<byte>(requestBytes);
        await _clientWebSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task<string> ListenToServer()
    {
        var buffer = new byte[1024];
        var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        return System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
    }
}

public class PolygonResponseBase
{
    public string Ev { get; set; } = string.Empty;
}

public class PolygonAuthResponse : PolygonResponseBase
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
}

public enum EnumPolygonAction
{
    Auth,
    Subscribe,
    Unsubscribe
}

public class PolygonRequestBase
{
    [JsonProperty("action")]
    public string Action { get; set; } = string.Empty;
}

public class PolygonAuthRequest : PolygonRequestBase
{
    [JsonProperty("params")]
    public string Params { get; set; } = string.Empty;
}

public class PolygonSubscribeRequest : PolygonRequestBase
{
    [JsonProperty("params")]
    public string Params { get; set; } = string.Empty;
}
