using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.WebSockets;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;

namespace TestConsole.Programs;

public class IQOption
{
    private bool _isSendAuthMessage = false;
    private string _ssid = "68fcec9b9ac7dd7ee4052acca3463c7l";
    private bool _isSendGetCandlesMessage = false;
    private bool _isReceivedFront = false;
    private int _requestIdCounter = 0;

    private List<string> _receivedMessages = new List<string>();

    public async Task Run()
    {
        await CandleScraper();
    }
    public async Task CandleScraper()
    {
        var url = new Uri("wss://ws.km.iqoption.com/echo/websocket");
        using var client = new ClientWebSocket(); ;
        await client.ConnectAsync(url, CancellationToken.None);
        while (client.State == WebSocketState.Open)
        {
            _receivedMessages.Add(await ListenForMessage(client));
            ProcessReceivedMessages();
            await IfNeedToSendAuthMessage(client);
            await IfNeedToSendGetCandlesMessage(client);
        }
    }

    private void ProcessReceivedMessages()
    {
        try
        {
            var lastMessageInJson = _receivedMessages.LastOrDefault();
            if (String.IsNullOrEmpty(lastMessageInJson))
                return;
            var message = JsonConvert.DeserializeObject<MessageReceiveBase<object>>(lastMessageInJson);

            switch (message.name)
            {
                case "front":
                    _isReceivedFront = true;
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex}");
        }
    }

    private async Task IfNeedToSendGetCandlesMessage(ClientWebSocket client)
    {
        if (_isSendAuthMessage && _isReceivedFront && !_isSendGetCandlesMessage)
        {
            await SendGetCandlesMessage(client);
            _isSendGetCandlesMessage = true;
        }
    }

    private async Task SendGetCandlesMessage(ClientWebSocket client)
    {
        _requestIdCounter++;
        var request = new GetCandlesMessageRequest()
        {
            request_id = _requestIdCounter.ToString(),
            msg = new GetCandlesMessageRequest.Msg()
            {
                body = new GetCandlesMessageRequest.Msg.MsBody()
                {
                    active_id = 74,
                    from_id = 35156307,
                    to_id = 35156506,
                    only_closed = true,
                    size = 5,
                    split_normalization = true,
                }
            }
        };
        await SendMessage(request, client);
    }

    private async Task IfNeedToSendAuthMessage(ClientWebSocket client)
    {
        if (!_isSendAuthMessage)
        {
            await SendAuthMessage(client);
            _isSendAuthMessage = true;
        }
    }

    private async Task SendAuthMessage(ClientWebSocket client)
    {
        _requestIdCounter++;
        var request = new AuthMessageRequest()
        {
            request_id = _requestIdCounter.ToString(),
            msg = new AuthMessageRequest.Msg()
            {
                ssid = _ssid,
                protocol = 3,
            }
        };
        await SendMessage(request, client);
    }

    private async Task SendMessage<T>(T request, ClientWebSocket client)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(request);
        var buffer = System.Text.Encoding.UTF8.GetBytes(json);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Sending: {json}");
        Console.ForegroundColor = ConsoleColor.White;
        await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private static async Task<string> ListenForMessage(ClientWebSocket client)
    {
        var buffer = new byte[60000];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Text)
        {
            var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received: {message}");
            return message;
        }
        return string.Empty;
    }
}

internal class GetCandlesMessageRequest : MessageBase
{
    public override string name { get; set; } = "sendMessage";
    public override string request_id { get; set; }
    public override int local_time { get; set; } = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public Msg msg { get; set; }
    public class Msg
    {
        public string name { get; set; } = "get-candles";
        public string version { get; set; } = "2.0";
        public MsBody body { get; set; }

        public class MsBody
        {
            public int active_id { get; set; }
            public int size { get; set; }
            public int from_id { get; set; }
            public int to_id { get; set; }
            public bool split_normalization { get; set; }
            public bool only_closed { get; set; }
        }

    }
}

public class MessageBase
{
    public virtual string name { get; set; }
    public virtual string request_id { get; set; }
    public virtual int local_time { get; set; }
}

internal class AuthMessageRequest : MessageBase
{
    public override string name { get; set; } = "authenticate";
    public override string request_id { get; set; }
    public override int local_time { get; set; } = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public Msg msg { get; set; }

    public class Msg
    {
        public string ssid { get; set; }
        public int protocol { get; set; }
        public string session_id { get; set; }
        public string client_session_id { get; set; }
    }


}

public class MessageReceiveBase<T>
{
    public string name { get; set;  }
    public T msg { get; set;  }
}