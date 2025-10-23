using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using NPOI.SS.Formula;
using TestConsole.Helper;

namespace TestConsole.Programs;

public class IQOption
{
    private bool _isSendAuthMessage = false;
    private string _ssid = "68fcec9b9ac7dd7ee4052acca3463c7l";
    private bool _isSendGetCandlesMessage = false;
    private bool _isReceivedFront = false;
    private int _requestIdCounter = 0;
    private HttpHelper _httpHelper = new HttpHelper();
    private bool _isAlreadySubscribeCandlesData = false;
    private readonly object _lockObject = new object();
    private readonly object _lockSaveLiveCandles = new object();

    private List<Candle> _liveCandlesThatNeedToSave = new List<Candle>();

    private List<RawMessageReceiveBase> _receivedMessages = new List<RawMessageReceiveBase>();

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
            Console.WriteLine("Unprocessed Messages Count: " + _receivedMessages.Count);
            await ListenForMessage(client);
            ProcessReceivedMessages();
            await IfNeedToSendAuthMessage(client);
            // await IfNeedToSendGetCandlesMessage(client);
            await IfNeedToSubscribeCandlesData(client);
        }
    }

    private async Task IfNeedToSubscribeCandlesData(ClientWebSocket client)
    {
        if (_isSendAuthMessage && _isReceivedFront && !_isAlreadySubscribeCandlesData)
        {
            await SendSubscribeCandlesDataMessage(client);
            _isAlreadySubscribeCandlesData = true;
        }
    }

    private async Task SendSubscribeCandlesDataMessage(ClientWebSocket client)
    {
        _requestIdCounter++;
        var request = new SubscribeCandlesMessageRequest()
        {
            request_id = $"s_{_requestIdCounter}" ,
            msg = new SubscribeCandlesMessageRequest.Msg()
            {
                Params = new SubscribeCandlesMessageRequest.Msg.ParamsClass()
                {
                    routingFilters = new SubscribeCandlesMessageRequest.Msg.ParamsClass.RoutingFiltersClass()
                    {
                        active_id = 1,
                        size = 5,
                    }
                }
            }
        };
        await SendMessage(request, client);
    }

    private void ProcessReceivedMessages()
    {
        try
        {
            lock (_lockObject)
            {
                var lastMessageInJson = _receivedMessages.FirstOrDefault(m => !m.IsProcced)?.RawMessage;

                if (String.IsNullOrEmpty(lastMessageInJson))
                    return;
                var message = JsonConvert.DeserializeObject<MessageReceiveBase<object>>(lastMessageInJson);

                switch (message.name)
                {
                    case "front":
                        _isReceivedFront = true;
                        break;
                    case "candles":
                        SaveCandles(message);
                        break;
                    case "candle-generated":
                        SaveLiveCandles(message);
                        break;
                    default:
                        break;
                }
                var messageToMark = _receivedMessages.FirstOrDefault(m => m.RawMessage == lastMessageInJson);
                if (messageToMark != null)
                {
                    messageToMark.IsProcced = true;
                }
                _receivedMessages.RemoveAll(m => m.IsProcced);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex}");
        }
    }

    private async Task SaveLiveCandles(MessageReceiveBase<object> message)
    {
        var request = new SaveCandlesRequest();
        lock (_lockSaveLiveCandles)
        {
            var candleInJson = message.msg.ToString();
            var candle = JsonConvert.DeserializeObject<LiveCandle>(candleInJson);
            var liveCandle = new SaveCandlesRequest()
            {
                Candles = new List<Candle>()
            {
                new Candle()
                {
                    Id = candle.id,
                    From = candle.from,
                    To = candle.to,
                    Open = candle.open,
                    Close = candle.close,
                    Min = candle.min,
                    Max = candle.max,
                    Volume = candle.volume,
                }
            }
            };
            if (_liveCandlesThatNeedToSave.Count < 20)
            {
                _liveCandlesThatNeedToSave.AddRange(liveCandle.Candles);
                return;
            }
            request.Candles = liveCandle.Candles.Concat(_liveCandlesThatNeedToSave).ToList();
            _liveCandlesThatNeedToSave.Clear();
        }
        var response = await _httpHelper.PostAsync("https://apini.ppiinn.net/api/iqoption/add-candles", JsonConvert.SerializeObject(request));
        Console.WriteLine($"Save live candle response: {JsonConvert.SerializeObject(response)}");
    }

    private async Task SaveCandles(MessageReceiveBase<object> message)
    {
        var candlesInJson = message.msg.ToString();
        var candles = JsonConvert.DeserializeObject<CandleMessage>(candlesInJson);
        var request = new SaveCandlesRequest()
        {
            Candles = candles.candles
        };
        var response = await _httpHelper.PostAsync("https://apini.ppiinn.net/api/iqoption/add-candles", JsonConvert.SerializeObject(request));
        Console.WriteLine($"Save candles response: {JsonConvert.SerializeObject(response)}");
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
                    active_id = 1,
                    from_id = 35751731,
                    to_id = 35752131,
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
        var json = JsonConvert.SerializeObject(request);
        var buffer = System.Text.Encoding.UTF8.GetBytes(json);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Sending: {json}");
        Console.ForegroundColor = ConsoleColor.White;
        await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task<string> ListenForMessage(ClientWebSocket client)
    {
        var buffer = new byte[60000];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Text)
        {
            var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received: {message}");
            lock (_lockObject)
            {
                _receivedMessages.Add(new RawMessageReceiveBase()
                {
                    RawMessage = message,
                    IsProcced = false,
                });
            }
            
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
    public string name { get; set; }
    public T msg { get; set; }
}

public class RawMessageReceiveBase
{
    public bool IsProcced { get; set; } = false;
    public string RawMessage { get; set; }
}

public class CandleMessage
{
    public List<Candle> candles { get; set; }
}

public class Candle
{
    public long Id { get; set; }
    public long From { get; set; }
    public long To { get; set; }
    public double Open { get; set; }
    public double Close { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public int Volume { get; set; }
}

public class SaveCandlesRequest
{
    public List<Candle> Candles { get; set; }
}

public class SubscribeCandlesMessageRequest
{
    public string name { get; set; } = "subscribeMessage";
    public string request_id { get; set; }

    public int local_time { get; set; } = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public Msg msg { get; set; }
    public class Msg
    {
        public string name { get; set; } = "candle-generated";

        [JsonProperty("params")]
        public ParamsClass Params { get; set; }

        public class ParamsClass
        {
            public RoutingFiltersClass routingFilters { get; set; }

            public class RoutingFiltersClass
            {
                public int active_id { get; set; }
                public int size { get; set; }
            }
        }
    }
}

public class LiveCandle
{
    public int active_id { get; set; }
    public int size { get; set; }
    public long at { get; set; }
    public long from { get; set; }
    public long to { get; set; }
    public long id { get; set; }
    public double open { get; set; }
    public double close { get; set; }
    public double min { get; set; }
    public double max { get; set; }
    public double ask { get; set; }
    public double bid { get; set; }
    public int volume { get; set; }
    public string phase { get; set; }
}