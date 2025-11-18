using System.Text.Json.Serialization;

namespace TestConsole.model;

public class MiranaGamelist
{
    [JsonPropertyName("seamlessGameProviderGames")]
    public List<SeamlessGameProviderGame> SeamlessGameProviderGames { get; set; } = new();
}

public class SeamlessGameProviderGame
{
    [JsonPropertyName("gameProviderId")]
    public int GameProviderId { get; set; }

    [JsonPropertyName("gameID")]
    public int GameID { get; set; }

    [JsonPropertyName("gameType")]
    public int GameType { get; set; }

    [JsonPropertyName("newGameType")]
    public int NewGameType { get; set; }

    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("device")]
    public string Device { get; set; } = string.Empty;

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = string.Empty;

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("rtp")]
    public double Rtp { get; set; }

    [JsonPropertyName("rows")]
    public int Rows { get; set; }

    [JsonPropertyName("reels")]
    public int Reels { get; set; }

    [JsonPropertyName("lines")]
    public int Lines { get; set; }

    [JsonPropertyName("gameInfos")]
    public List<GameInfo> GameInfos { get; set; } = new();

    [JsonPropertyName("supportedCurrencies")]
    public List<string> SupportedCurrencies { get; set; } = new();

    [JsonPropertyName("blockCountries")]
    public List<string> BlockCountries { get; set; } = new();

    [JsonPropertyName("isMaintain")]
    public bool IsMaintain { get; set; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; }

    [JsonPropertyName("isProvideCommission")]
    public bool IsProvideCommission { get; set; }

    [JsonPropertyName("hasHedgeBet")]
    public bool HasHedgeBet { get; set; }

    [JsonPropertyName("providerStatus")]
    public string ProviderStatus { get; set; } = string.Empty;

    [JsonPropertyName("isProviderOnline")]
    public bool IsProviderOnline { get; set; }
}

public class GameInfo
{
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("gameName")]
    public string GameName { get; set; } = string.Empty;

    [JsonPropertyName("gameIconUrl")]
    public string GameIconUrl { get; set; } = string.Empty;
}
