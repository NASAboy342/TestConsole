using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TestConsole.model;

public class GMSAddGameRequest : GMSBaseRequest
{
    public GMSAddGameRequest()
    {
    }

    public GMSAddGameRequest(GameFromGMS game, bool isForSA = false)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game));
        }

        GameProviderId = game.GameProviderId;
        GameId = game.GameId;
        GameType = game.GameType;
        NewGameType = game.NewGameType;
        Rank = game.Rank;
        GameCode = game.GameCode;
        GameCode1 = game.GameCode1;
        GameCode2 = game.GameCode2;
        GameCode3 = game.GameCode3;
        GameCode4 = game.GameCode4;
        GameCode5 = game.GameCode5;
        Device = game.Device;
        Platform = game.Platform;
        Provider = game.Provider;
        Rtp = game.Rtp.ToString(CultureInfo.InvariantCulture);
        Rows = game.Rows;
        Reels = game.Reels;
        Lines = game.Lines;
        GameLanguages = game.GameLanguages?.Select(gl => new AddGameLanguageInfo
        {
            Language = gl.Language,
            GameName = gl.GameName,
            GameIconFilePath = gl.GameIconFilePath,
            Image = string.Empty,
            IsGameIconUseUrl = gl.IsGameIconOnServer
        }).ToList() ?? new List<AddGameLanguageInfo>();
        SupportedCurrencies = game.SupportedCurrencies ?? new List<string>();
        BlockedCountries = game.BlockedCountries ?? new List<string>();
        IsUnderMaintain = game.IsUnderMaintain;
        IsEnabled = game.IsEnabled;
        IsRetired = game.IsRetired;
        IsJackpot = game.IsJackpot;
        IsNewGame = game.IsNewGame;
        IsProvideCommission = game.IsProvideCommission;
        HasHedgeBet = game.HasHedgeBet;
        HasBuyFreeSpin = game.HasBuyFreeSpin;
        IsForSA = isForSA;
        Remark = game.Remark;
        DisableReason = game.DisableReason;
    }

    public string ModifiedBy { get; set; }
    public int GameProviderId { get; set; }
    public int GameId { get; set; }
    public int GameType { get; set; }
    public int NewGameType { get; set; }
    public int Rank { get; set; }
    public string GameCode { get; set; }
    public string GameCode1 { get; set; }
    public string GameCode2 { get; set; }
    public string GameCode3 { get; set; }
    public string GameCode4 { get; set; }
    public string GameCode5 { get; set; }
    public string Device { get; set; }
    public string Platform { get; set; }
    public string Provider { get; set; }
    public string Rtp { get; set; }
    public int Rows { get; set; }
    public int Reels { get; set; }
    public int Lines { get; set; }
    public List<AddGameLanguageInfo> GameLanguages { get; set; }
    public List<string> SupportedCurrencies { get; set; }
    public List<string> BlockedCountries { get; set; }
    public bool IsUnderMaintain { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsRetired { get; set; }
    public bool IsJackpot { get; set; }
    public bool IsNewGame { get; set; }
    public bool IsProvideCommission { get; set; }
    public bool HasHedgeBet { get; set; }
    public bool HasBuyFreeSpin { get; set; }
    public bool IsForSA { get; set; }
    public string Remark { get; set; }
    public string DisableReason { get; set; }
}

public class AddGameLanguageInfo
{
    public string Language { get; set; }
    public string GameName { get; set; }
    public string GameIconFilePath { get; set; }
    public string Image { get; set; }
    public bool IsGameIconUseUrl { get; set; }
}
