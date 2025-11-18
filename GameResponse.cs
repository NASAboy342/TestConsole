using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TestConsole;

public class GameResponse
{
    public int Code { get; set; }
    public string Msg { get; set; }
    public List<GameData> Data { get; set; }
}

public class GameData
{
    public string Provider { get; set; }
    [JsonProperty("game_id")]
    public int GameId { get; set; }
    public string GameName { get; set; }
    public string GameNameCn { get; set; }
    public string ReleaseDate { get; set; }
    public string Rtp { get; set; }
    [JsonProperty("game_icon")]
    public string GameIcon { get; set; }
    public string ContentType { get; set; }
    public string GameType { get; set; }
    public string Content { get; set; }
}
