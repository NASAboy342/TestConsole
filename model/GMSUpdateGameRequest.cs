using System;
using System.Collections.Generic;

namespace TestConsole.model;

public class GMSUpdateGameRequest : GMSBaseRequest
{
    public string ModifiedBy { get; set; }
    public int GameProviderId { get; set; }
    public int GameId { get; set; }
    public int Rank { get; set; }
    public int GameType { get; set; }
    public int NewGameType { get; set; }
    public string Device { get; set; }
    public string Platform { get; set; }
    public double Rtp { get; set; }
    public int Rows { get; set; }
    public int Reels { get; set; }
    public int Lines { get; set; }
    public string GameCode { get; set; }
    public string GameCode1 { get; set; }
    public string GameCode2 { get; set; }
    public string GameCode3 { get; set; }
    public string GameCode4 { get; set; }
    public string GameCode5 { get; set; }
    public string Provider { get; set; }
    public List<GameLanguageInfo> GameLanguages { get; set; }
    public List<string> SupportedCurrencies { get; set; }
    public List<string> BlockedCountries { get; set; }
    public bool IsUnderMaintain { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsRetired { get; set; }
    public bool IsJackpot { get; set; }
    public bool IsNewGame { get; set; }
    public string Remark { get; set; }
    public bool IsProvideCommission { get; set; }
    public bool HasHedgeBet { get; set; }
    public bool HasBuyFreeSpin { get; set; }
    public string DisableReason { get; set; }
    public bool IsForSA { get; set; }
}

public class GameLanguageInfo
{
    public string Language { get; set; }
    public string GameName { get; set; }
    public string GameIconFilePath { get; set; }
    public bool IsGameIconOnServer { get; set; }
}
