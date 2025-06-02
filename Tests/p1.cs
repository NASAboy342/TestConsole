using Newtonsoft.Json;
using System.ComponentModel;
using System.Globalization;

namespace TestConsole.Tests
{
    public class p1
    {
        public static void Run()
        {
            var fakeGameReportData = new List<GetGameProviderGameReportSummaryFromDb>
            {
                new GetGameProviderGameReportSummaryFromDb
                {
                    Date = DateTime.Parse("2024-02-01"),
                    BrandName = "Brand A",
                    GpId = 1,
                    Currency = "USD",
                    AU = 150,
                    WinLost = -5000.75m,
                    Turnover = 25000.50m,
                    BetCount = 1200,
                    GameId = 101,
                    WebId = 1
                },
                new GetGameProviderGameReportSummaryFromDb
                {
                    Date = DateTime.Parse("2024-02-02"),
                    BrandName = "Brand A",
                    GpId = 1,
                    Currency = "USD",
                    AU = 160,
                    WinLost = -4500.00m,
                    Turnover = 26000.00m,
                    BetCount = 1300,
                    GameId = 102,
                    WebId = 1
                },
                new GetGameProviderGameReportSummaryFromDb
                {
                    Date = DateTime.Parse("2024-02-01"),
                    BrandName = "Brand B",
                    GpId = 2,
                    Currency = "EUR",
                    AU = 100,
                    WinLost = -3000.00m,
                    Turnover = 18000.75m,
                    BetCount = 900,
                    GameId = 201,
                    WebId = 2
                },
                new GetGameProviderGameReportSummaryFromDb
                {
                    Date = DateTime.Parse("2024-02-02"),
                    BrandName = "Brand B",
                    GpId = 2,
                    Currency = "EUR",
                    AU = 110,
                    WinLost = -2800.50m,
                    Turnover = 19000.25m,
                    BetCount = 950,
                    GameId = 202,
                    WebId = 2
                },
                new GetGameProviderGameReportSummaryFromDb
                {
                    Date = DateTime.Parse("2024-02-10"),
                    BrandName = "Brand B",
                    GpId = 2,
                    Currency = "EUR",
                    AU = 0,
                    WinLost = -2800.50m,
                    Turnover = 19000.25m,
                    BetCount = 950,
                    GameId = 202,
                    WebId = 2
                },
                new GetGameProviderGameReportSummaryFromDb
                {
                    Date = DateTime.Parse("2024-02-05"),
                    BrandName = "Brand B",
                    GpId = 2,
                    Currency = "EUR",
                    AU = 0,
                    WinLost = 0,
                    Turnover = 19000.25m,
                    BetCount = 950,
                    GameId = 202,
                    WebId = 2
                },
                new GetGameProviderGameReportSummaryFromDb
                {
                    Date = DateTime.Parse("2024-02-03"),
                    BrandName = "Brand B",
                    GpId = 2,
                    Currency = "EUR",
                    AU = 0,
                    WinLost = 0,
                    Turnover = 19000.25m,
                    BetCount = 950,
                    GameId = 202,
                    WebId = 2
                },
                new GetGameProviderGameReportSummaryFromDb
                {
                    Date = DateTime.Parse("2024-02-03"),
                    BrandName = "Brand A",
                    GpId = 4,
                    Currency = "EUR",
                    AU = 0,
                    WinLost = 0,
                    Turnover = 19000.25m,
                    BetCount = 950,
                    GameId = 202,
                    WebId = 2
                }
            };
            var request = new GetProviderGameRequest { ProviderId = 3, DateType = "Daily" };
            var summaryDatas = new SummaryDataForReportModel();
            var turnOverGroup = new List<GameReportModel>();
            var winLostGroup = new List<GameReportModel>();
            var activeUserGroup = new List<GameReportModel>();
            var betCountGroup = new List<GameReportModel>();

            var programInstance = new p1();

            turnOverGroup = programInstance.GetSummaryReportForResponse(fakeGameReportData, request, "TurnOver");
            winLostGroup = programInstance.GetSummaryReportForResponse(fakeGameReportData, request, "WinLost");
            activeUserGroup = programInstance.GetSummaryReportForResponse(fakeGameReportData, request, "ActiveUser");
            betCountGroup = programInstance.GetSummaryReportForResponse(fakeGameReportData, request, "BetCount");

            var result = new
            {
                TurnOver = turnOverGroup,
                WinLost = winLostGroup,
                ActiveUser = activeUserGroup,
                BetCount = betCountGroup
            };
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }

        public List<GameReportModel> GetSummaryReportForResponse(List<GetGameProviderGameReportSummaryFromDb> fromDbDatas,
                GetProviderGameRequest request, string dataToGet)
        {
            var gameReportGroupList = new List<GameReportModel>();
            var dataMapping = new Dictionary<string, Func<GetGameProviderGameReportSummaryFromDb, decimal>>
                {
                    { "TurnOver", totalReportData => totalReportData.Turnover },
                    { "WinLost", totalReportData => totalReportData.WinLost },
                    { "ActiveUser", totalReportData => totalReportData.AU },
                    { "BetCount", totalReportData => totalReportData.BetCount }
                };

            var allGameName = new GetGamesResponse
            {
                Games = new List<Game>
                        {
                            new Game { ProviderId = 1, GameId = 101, GameName = "Game A", SupportedCurrencies = new List<string> { "USD", "EUR" } },
                            new Game { ProviderId = 1, GameId = 102, GameName = "Game B", SupportedCurrencies = new List<string> { "USD", "GBP" } },
                            new Game { ProviderId = 2, GameId = 201, GameName = "Game C", SupportedCurrencies = new List<string> { "EUR" } }
                        }
            };
            var allGameProvider = new GetGameProvidersResponse
            {
                GameProviders = new List<GetGameProviderOption>
                        {
                            new GetGameProviderOption { ProviderId = 1, ProviderName = "Provider X" },
                            new GetGameProviderOption { ProviderId = 2, ProviderName = "Provider Y" },
                            new GetGameProviderOption { ProviderId = 3, ProviderName = "Provider Z" },
                            new GetGameProviderOption { ProviderId = 4, ProviderName = "Provider XY" },
                        }
            };
            gameReportGroupList = fromDbDatas.GroupBy(fromDbData =>
                    GetDataKeyForReport(request, fromDbData, allGameProvider, allGameName))
                .Select(groupDatas => new GameReportModel
                {
                    Key = groupDatas.Key,
                    Dates = groupDatas.Select(groupData => new
                    {
                        DateAsString = GetDateTimeForDisplay(groupData.Date, request.DateType),
                        Value = dataMapping[dataToGet](groupData),
                        groupData.Date
                    })
                        .OrderBy(groupData => groupData.Date)
                        .GroupBy(groupData => groupData.DateAsString)
                        .Select(groupDate => new Dictionary<string, decimal>
                        {
                                { groupDate.Key, groupDate.Sum(g => g.Value) }
                        }).ToList()
                }).OrderByDescending(gameReport => gameReport.Dates
                    .Sum(d => d.Values.Sum())).ToList();
            var filledDataForResponse = FillingEmptyDataForResponse(gameReportGroupList);
            return filledDataForResponse;
        }

        private List<GameReportModel> FillingEmptyDataForResponse(List<GameReportModel> gameReportGroupList)
        {
            var allWeeks = gameReportGroupList
                .SelectMany(group => group.Dates.SelectMany(date => date.Keys))
                .Distinct()
                .ToList();
            gameReportGroupList = gameReportGroupList
                .Select(group => new GameReportModel
                {
                    Key = group.Key,
                    Dates = allWeeks
                        .Select(week => group.Dates.FirstOrDefault(date => date.ContainsKey(week))
                                        ?? new Dictionary<string, decimal> { { week, 0.0m } })
                        .ToList()
                })
                .ToList();
            return gameReportGroupList;
        }

        private string GetDataKeyForReport(GetProviderGameRequest request, GetGameProviderGameReportSummaryFromDb data, GetGameProvidersResponse gameProvider, GetGamesResponse allGames)
        {
            var filterTypeAsEnum = Enum.TryParse<FilteringGameReportType>(request.FilteringBy, true, out var filteringGameReportType) ? filteringGameReportType : FilteringGameReportType.Default;
            var provider = gameProvider.GameProviders.FirstOrDefault(item => item.ProviderId == data.GpId);
            switch (filterTypeAsEnum)
            {
                case FilteringGameReportType.Customer:
                    return data.BrandName;

                case FilteringGameReportType.Game:

                    var game = allGames.Games.FirstOrDefault(item => item.GameId == data.GameId);
                    return game.GameName;

                case FilteringGameReportType.Provider:
                    return provider?.ProviderName ?? string.Empty;
            }
            return provider?.ProviderName ?? string.Empty;
        }

        private string GetDateTimeForDisplay(DateTime date, string dateType)
        {
            var dateTypeAsEnum = FilterDateTypeEnum.Parse<FilterDateTypeEnum>(dateType, true);
            var calendar = new GregorianCalendar();
            var isFilterByDaily = dateTypeAsEnum.Equals(FilterDateTypeEnum.Daily);
            var isFilterByWeekly = dateTypeAsEnum.Equals(FilterDateTypeEnum.Weekly);
            var isFilterByMonthly = dateTypeAsEnum.Equals(FilterDateTypeEnum.Monthly);
            var filterDateType = "";
            if (isFilterByWeekly)
            {
                filterDateType = $"Week {calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString()} {date:yyyy}";
            }
            else if (isFilterByMonthly)
            {
                filterDateType = date.ToString("MMM yyyy");
            }
            else if (isFilterByDaily)
            {
                filterDateType = date.ToString("yyyy-MM-dd");
            }
            return filterDateType;
        }

        public class GetProviderGameRequest
        {
            public int ProviderId { get; set; }
            public int Customer { get; set; }
            public string Currency { get; set; }
            public List<int> GameIds { get; set; }
            public string DateType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string FilteringBy { get; set; }

            public bool IsWithinMaxDateRange(int maxDays = 14)
            {
                var dateDifference = (EndDate - StartDate).TotalDays;
                return dateDifference <= maxDays;
            }

            public bool IsWithinMaxWeekRange(int maxWeeks = 14)
            {
                var totalWeeks = (EndDate - StartDate).TotalDays / 7;
                return totalWeeks <= maxWeeks;
            }

            public bool IsWithinMaxMonthRange(int maxMonths = 12)
            {
                var totalMonths = ((EndDate.Year - StartDate.Year) * 12) + EndDate.Month - StartDate.Month;
                return totalMonths <= maxMonths;
            }
        }

        public class GameViewModel
        {
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
            public decimal Rtp { get; set; }
            public int Rows { get; set; }
            public int Reels { get; set; }
            public int Lines { get; set; }
            public List<GameLanguageViewModel> GameLanguages { get; set; }
            public List<string> SupportedCurrencies { get; set; }
            public List<string> ProviderSupportedCurrencies { get; set; } = new List<string>();
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
            public string EnglishGameName => GetEnlishGameName();
            public string ChineseGameName => GetChineseGameName();

            private string GetChineseGameName()
            {
                var gameLanguage = GameLanguages.FirstOrDefault(g => g.Language.Equals("zh_cn", StringComparison.OrdinalIgnoreCase)) ?? new GameLanguageViewModel();
                return gameLanguage.GameName;
            }

            private string GetEnlishGameName()
            {
                var gameLanguage = GameLanguages.FirstOrDefault(g => g.Language.Equals("en", StringComparison.OrdinalIgnoreCase)) ?? new GameLanguageViewModel();
                return gameLanguage.GameName;
            }
        }

        public class GameLanguageViewModel
        {
            public GameLanguageViewModel()
            {
            }

            public GameLanguageViewModel(string language, string gameName, string gameIconFilePath, bool isGameIconOnServer)
            {
                Language = language;
                GameName = gameName;
                GameIconFilePath = gameIconFilePath;
                IsGameIconOnServer = isGameIconOnServer;
            }

            public string Language { get; set; } = "";
            public string GameName { get; set; } = "";
            public string GameIconFilePath { get; set; } = "";
            public bool IsGameIconOnServer { get; set; }
        }

        public class GetGameProviderGameReportSummaryFromDb
        {
            public DateTime Date { get; set; }
            public string BrandName { get; set; } = string.Empty;
            public int? GpId { get; set; }
            public string Currency { get; set; }
            public int AU { get; set; }
            public decimal WinLost { get; set; }
            public decimal Turnover { get; set; }
            public int BetCount { get; set; }
            public int? GameId { get; set; }
            public int? WebId { get; set; } = 0;
        }

        public class GameReportModel
        {
            public String Key { get; set; } = string.Empty;
            public List<Dictionary<string, decimal>> Dates { get; set; }
        }

        public class GetGameProvidersResponse
        {
            public List<GetGameProviderOption> GameProviders { get; set; } = new List<GetGameProviderOption>();
        }

        public class GetGameProviderOption
        {
            public int ProviderId { get; set; }
            public string ProviderName { get; set; } = string.Empty;
        }

        public class GetGamesResponse
        {
            public List<Game> Games { get; set; }
        }

        public class Game
        {
            public int ProviderId { get; set; }
            public int GameId { get; set; }
            public string GameName { get; set; } = "";
            public List<string> SupportedCurrencies { get; set; }
        }

        public class SummaryDataForReportModel
        {
            public Decimal TurnOver { get; set; }
            public Decimal WinLost { get; set; }
            public int ActiveUser { get; set; }
            public int BetCount { get; set; }
        }

        public enum FilteringGameReportType
        {
            [Description("provider")]
            Provider = 1,

            [Description("customer")]
            Customer = 2,

            [Description("game")]
            Game = 3,

            [Description("default")]
            Default = 4,
        }

        public enum FilterDateTypeEnum
        {
            [Description("daily")]
            Daily = 1,

            [Description("weekly")]
            Weekly = 2,

            [Description("monthly")]
            Monthly = 3,
        }
    }
}