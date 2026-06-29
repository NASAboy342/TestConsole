using System;
using Newtonsoft.Json;
using TestConsole.model;

namespace TestConsole.Helper;

public class GMSHelper
{
    private readonly string _apiUrl = "http://gms-api-uat.remotes.local";
    private readonly string _secretKey = "StagingSecretKey";
    private readonly string _sessionToken;
    public GMSHelper(string sessionToken, string apiUrl = "http://gms-api-uat.remotes.local")
    {
        _apiUrl = apiUrl;
        _sessionToken = sessionToken;
    }


    public async Task<GetAllGameResponse> GetAllGameAsync(int providerId, bool isForSA = false)
    {
        var apiHelper = new HttpHelper();
        var request = new GMSGetAllGameRequest
        {
            ProviderId = providerId,
            IsForSA = isForSA,
            SessionToken = _sessionToken
        };

        var response = await apiHelper.PostAsync<GMSGetAllGameRequest, GetAllGameResponse>($"{_apiUrl}/api/Game/GetAll", request);
        return response;
    }

    public async Task UpdateGameCurrencyToGMS(Game? qTechGame, ProviderCurrencyInfo currencyInfo, GameFromGMS? game)
    {
        var gMSUpdateGameRequest = new GMSUpdateGameRequest
        {
            IsForSA = false,
            BlockedCountries = game.BlockedCountries,
            Device = game.Device,
            DisableReason = game.DisableReason,
            GameCode = game.GameCode,
            GameCode1 = game.GameCode1,
            GameCode2 = game.GameCode2,
            GameCode3 = game.GameCode3,
            GameCode4 = game.GameCode4,
            GameCode5 = game.GameCode5,
            GameId = game.GameId,
            GameLanguages = game.GameLanguages.Select(gl => new GameLanguageInfo
            {
                GameIconFilePath = gl.GameIconFilePath,
                IsGameIconOnServer = gl.IsGameIconOnServer,
                Language = gl.Language,
                GameName = gl.GameName
            }).ToList(),
            GameProviderId = game.GameProviderId,
            GameType = game.GameType,
            HasBuyFreeSpin = game.HasBuyFreeSpin,
            HasHedgeBet = game.HasHedgeBet,
            IsEnabled = game.IsEnabled,
            IsJackpot = game.IsJackpot,
            IsNewGame = game.IsNewGame,
            IsProvideCommission = game.IsProvideCommission,
            IsRetired = game.IsRetired,
            IsUnderMaintain = game.IsUnderMaintain,
            Lines = game.Lines,
            ModifiedBy = "TC_Pinsopheaktra",
            NewGameType = game.NewGameType,
            Platform = game.Platform,
            Provider = game.Provider,
            Rank = game.Rank,
            Reels = game.Reels,
            Remark = game.Remark,
            Rows = game.Rows,
            Rtp = game.Rtp,
            SessionToken = _sessionToken,
            SupportedCurrencies = qTechGame.ProviderSupportedCurrencies.Select(c => currencyInfo.ProviderCurrencyInfos.FirstOrDefault(pc => pc.ProviderCurrency.Equals(c))?.CustomerCurrency ?? "").Where(c => !string.IsNullOrEmpty(c)).ToList(),
        };
        var httpHelper = new HttpHelper();
        var response = await httpHelper.PostAsync<GMSUpdateGameRequest, GMSBaseResponse>($"{_apiUrl}/api/Game/Update", gMSUpdateGameRequest);
        if(response.ErrorCode != 0)
        {
            throw new Exception($"Failed to update game {game.GameId} : {response.ErrorMessage}");
        }
        Console.WriteLine($"Successfully to update game {JsonConvert.SerializeObject(response)}");

    }

    public async Task<GMSGetAllAgentsResponse> GetAllAgents(int providerId, bool isGetForAllProviders = false, bool isReloadCache = false, int page = 1, int itemsPerPage = 100)
    {
        var request = new GMSGetAllAgentsRequest
        {
            SecretKey = _secretKey,
            SessionToken = _sessionToken,
            ModifiedBy = "TC_Pinsopheaktra",
            IsGetForAllProviders = isGetForAllProviders,
            ProviderId = providerId,
            IsReloadCache = isReloadCache,
            PagenationProperty = new PagenationProperty
            {
                Page = page,
                ItemsPerPage = itemsPerPage
            }
        };

        var httpHelper = new HttpHelper();
        var response = await httpHelper.PostAsync<GMSGetAllAgentsRequest, GMSGetAllAgentsResponse>($"{_apiUrl}/api/GameProvider/GetProviderAgentInfo", request);
        return response;
    }

    public async Task<GMSUpdateAgentResponse> UpdateAgent(ProviderAgentInfo agentInfo)
    {
        var request = new GMSUpdateAgentRequest
        {
            SecretKey = _secretKey,
            SessionToken = _sessionToken,
            ModifiedBy = "TC_Pinsopheaktra",
            AgentInfos = new List<AgentInfoUpdate>
            {
                new AgentInfoUpdate
                {
                    ProviderId = agentInfo.ProviderId,
                    Currency = agentInfo.Currency,
                    ApiUrl = agentInfo.ApiUrl,
                    ApiUrl1 = agentInfo.ApiUrl1,
                    AgentId = agentInfo.AgentId,
                    AgentName = agentInfo.AgentName,
                    Cert = agentInfo.Cert,
                    Cert1 = agentInfo.Cert1,
                    WebId = agentInfo.WebId,
                    ModifiedBy = "TC_Pinsopheaktra"
                }
            }
        };

        var httpHelper = new HttpHelper();
        var response = await httpHelper.PostAsync<GMSUpdateAgentRequest, GMSUpdateAgentResponse>($"{_apiUrl}/api/GameProvider/UpdateProviderAgentInfo", request);
        if (response.ErrorCode != 0)
        {
            throw new Exception($"Failed to update agent {agentInfo.AgentId}: {response.ErrorMessage}");
        }
        Console.WriteLine($"Successfully updated agent {agentInfo.AgentId}");
        return response;
    }

    internal async Task GetProviderInfoAsync(int v)
    {
        throw new NotImplementedException();
    }

    internal async Task UpdateGamesSubProviderToGMS(Game? qTechGame, GameFromGMS game)
    {
        var gMSUpdateGameRequest = new GMSUpdateGameRequest
        {
            IsForSA = false,
            BlockedCountries = game.BlockedCountries,
            Device = game.Device,
            DisableReason = game.DisableReason,
            GameCode = game.GameCode,
            GameCode1 = game.GameCode1,
            GameCode2 = game.GameCode2,
            GameCode3 = game.GameCode3,
            GameCode4 = game.GameCode4,
            GameCode5 = game.GameCode5,
            GameId = game.GameId,
            GameLanguages = game.GameLanguages.Select(gl => new GameLanguageInfo
            {
                GameIconFilePath = gl.GameIconFilePath,
                IsGameIconOnServer = gl.IsGameIconOnServer,
                Language = gl.Language,
                GameName = gl.GameName
            }).ToList(),
            GameProviderId = game.GameProviderId,
            GameType = game.GameType,
            HasBuyFreeSpin = game.HasBuyFreeSpin,
            HasHedgeBet = game.HasHedgeBet,
            IsEnabled = game.IsEnabled,
            IsJackpot = game.IsJackpot,
            IsNewGame = game.IsNewGame,
            IsProvideCommission = game.IsProvideCommission,
            IsRetired = game.IsRetired,
            IsUnderMaintain = game.IsUnderMaintain,
            Lines = game.Lines,
            ModifiedBy = "TC_Pinsopheaktra",
            NewGameType = game.NewGameType,
            Platform = game.Platform,
            Provider = qTechGame.SubProvider,
            Rank = game.Rank,
            Reels = game.Reels,
            Remark = game.Remark,
            Rows = game.Rows,
            Rtp = game.Rtp,
            SessionToken = _sessionToken,
            SupportedCurrencies = game.SupportedCurrencies
        };
        var httpHelper = new HttpHelper();
        var response = await httpHelper.PostAsync<GMSUpdateGameRequest, GMSBaseResponse>($"{_apiUrl}/api/Game/Update", gMSUpdateGameRequest);
        if(response.ErrorCode != 0)
        {
            throw new Exception($"Failed to update game {game.GameId} : {response.ErrorMessage}");
        }
        Console.WriteLine($"Successfully to update game {JsonConvert.SerializeObject(response)}");
    }

    internal async Task UpdateGameToGMSByGame(GameFromGMS game, bool isForSA = false)
    {
        var gMSUpdateGameRequest = new GMSUpdateGameRequest
        {
            IsForSA = isForSA,
            BlockedCountries = game.BlockedCountries,
            Device = game.Device,
            DisableReason = game.DisableReason,
            GameCode = game.GameCode,
            GameCode1 = game.GameCode1,
            GameCode2 = game.GameCode2,
            GameCode3 = game.GameCode3,
            GameCode4 = game.GameCode4,
            GameCode5 = game.GameCode5,
            GameId = game.GameId,
            GameLanguages = game.GameLanguages.Select(gl => new GameLanguageInfo
            {
                GameIconFilePath = gl.GameIconFilePath,
                IsGameIconOnServer = gl.IsGameIconOnServer,
                Language = gl.Language,
                GameName = gl.GameName
            }).ToList(),
            GameProviderId = game.GameProviderId,
            GameType = game.GameType,
            HasBuyFreeSpin = game.HasBuyFreeSpin,
            HasHedgeBet = game.HasHedgeBet,
            IsEnabled = game.IsEnabled,
            IsJackpot = game.IsJackpot,
            IsNewGame = game.IsNewGame,
            IsProvideCommission = game.IsProvideCommission,
            IsRetired = game.IsRetired,
            IsUnderMaintain = game.IsUnderMaintain,
            Lines = game.Lines,
            ModifiedBy = "TC_Pinsopheaktra",
            NewGameType = game.NewGameType,
            Platform = game.Platform,
            Provider = game.Provider,
            Rank = game.Rank,
            Reels = game.Reels,
            Remark = game.Remark,
            Rows = game.Rows,
            Rtp = game.Rtp,
            SessionToken = _sessionToken,
            SupportedCurrencies = game.SupportedCurrencies
        };
        var httpHelper = new HttpHelper();
        var response = await httpHelper.PostAsync<GMSUpdateGameRequest, GMSBaseResponse>($"{_apiUrl}/api/Game/Update", gMSUpdateGameRequest);
        if(response.ErrorCode != 0)
        {
            throw new Exception($"Failed to update game {game.GameId} : {response.ErrorMessage}");
        }
        Console.WriteLine($"Successfully to update game {JsonConvert.SerializeObject(response)}");
    }

    public async Task AddGame(GMSAddGameRequest addGameRequest)
    {
        addGameRequest.SecretKey = _secretKey;
        addGameRequest.SessionToken = _sessionToken;
        addGameRequest.ModifiedBy = string.IsNullOrWhiteSpace(addGameRequest.ModifiedBy) ? "TC_Pinsopheaktra" : addGameRequest.ModifiedBy;

        var httpHelper = new HttpHelper();
        var response = await httpHelper.PostAsync<GMSAddGameRequest, GMSBaseResponse>($"{_apiUrl}/api/Game/Add", addGameRequest);
        if (response.ErrorCode != 0)
        {
            throw new Exception($"Failed to add game : {response.ErrorMessage}");
        }
        Console.WriteLine($"Successfully to add game {JsonConvert.SerializeObject(response)}");
    }
}
