using System.Text.Json.Serialization;

namespace TestConsole.model;

public class MarsGetGameListResponse
{
    [JsonPropertyName("seamlessGameProviderGames")]
    public List<MarsSeamlessGame> SeamlessGameProviderGames { get; set; } = new();
}

public class MarsSeamlessGame
{
    [JsonPropertyName("providerStatus")]
    public string ProviderStatus { get; set; } = string.Empty;

    [JsonPropertyName("gameProviderId")]
    public int GameProviderId { get; set; }

    [JsonPropertyName("gameId")]
    public int GameId { get; set; }

    [JsonPropertyName("gameType")]
    public string GameType { get; set; } = string.Empty;

    [JsonPropertyName("newGameType")]
    public string NewGameType { get; set; } = string.Empty;

    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("gameCode")]
    public string GameCode { get; set; } = string.Empty;

    [JsonPropertyName("gameCode1")]
    public string? GameCode1 { get; set; }

    [JsonPropertyName("gameCode2")]
    public string? GameCode2 { get; set; }

    [JsonPropertyName("gameCode3")]
    public string? GameCode3 { get; set; }

    [JsonPropertyName("gameCode4")]
    public string? GameCode4 { get; set; }

    [JsonPropertyName("gameCode5")]
    public string? GameCode5 { get; set; }

    [JsonPropertyName("device")]
    public string Device { get; set; } = string.Empty;

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = string.Empty;

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; }

    [JsonPropertyName("isUM")]
    public bool IsUM { get; set; }

    [JsonPropertyName("isMaintain")]
    public bool IsMaintain { get; set; }

    [JsonPropertyName("isRetired")]
    public bool IsRetired { get; set; }

    [JsonPropertyName("remark")]
    public string Remark { get; set; } = string.Empty;

    [JsonPropertyName("gameInfos")]
    public List<GameInfo> GameInfos { get; set; } = new();

    [JsonPropertyName("supportedCurrencies")]
    public List<string> SupportedCurrencies { get; set; } = new();

    [JsonPropertyName("blockCountries")]
    public List<string> BlockCountries { get; set; } = new();

    [JsonPropertyName("rtp")]
    public double Rtp { get; set; }

    [JsonPropertyName("rows")]
    public int Rows { get; set; }

    [JsonPropertyName("reels")]
    public int Reels { get; set; }

    [JsonPropertyName("lines")]
    public int Lines { get; set; }

    [JsonPropertyName("isGotoGameDirectly")]
    public bool IsGotoGameDirectly { get; set; }

    [JsonPropertyName("isJackpot")]
    public bool IsJackpot { get; set; }

    [JsonPropertyName("isNewGame")]
    public bool IsNewGame { get; set; }

    [JsonPropertyName("isPromoteNow")]
    public bool IsPromoteNow { get; set; }

    [JsonPropertyName("promoteStartDate")]
    public DateTime? PromoteStartDate { get; set; }

    [JsonPropertyName("promoteEndDate")]
    public DateTime? PromoteEndDate { get; set; }

    [JsonPropertyName("gameProviderName")]
    public string GameProviderName { get; set; } = string.Empty;

    [JsonPropertyName("isProvideCommission")]
    public bool IsProvideCommission { get; set; }

    [JsonPropertyName("hasHedgeBet")]
    public bool HasHedgeBet { get; set; }

    [JsonPropertyName("subProviderId")]
    public int SubProviderId { get; set; }
}
