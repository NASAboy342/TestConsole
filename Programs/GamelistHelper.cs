using System;
using TestConsole.model;
using System.Linq;
using Newtonsoft.Json;
using TestConsole.Enums;
using System.Threading.Tasks;
using TestConsole.Helper;

namespace TestConsole.Programs;

public class GamelistHelper
{
    internal async Task Run()
    {
        var gmsSessionToken = "c12e42a7c9414";
        var uatUrl = "http://gms-api-uat.remotes.local";
        var gMSHelperUat = new GMSHelper(gmsSessionToken, uatUrl);
        Console.WriteLine("Getting all game info from GMS...");
        var allGameOnDemo = await gMSHelperUat.GetAllGameAsync(1025);
        Console.WriteLine($"Finished getting all game info from GMS. Total games: {allGameOnDemo.Data.Games.Count}");
        // var providerInfo = await gMSHelper.GetProviderInfoAsync(1058);


        // var gmsSessionTokenProd = "e8fd848417564";
        // var prodUrl = "http://gms-api.remotes.local";
        // var gMSHelperProd = new GMSHelper(gmsSessionTokenProd, prodUrl);
        // Console.WriteLine("Getting all game info from GMS...");
        // var allGameOnProd = await gMSHelperProd.GetAllGameAsync(1025);
        // Console.WriteLine($"Finished getting all game info from GMS. Total games: {allGameOnProd.Data.Games.Count}");

        var availableGameCodes = new List<string> { "fbbl","fbbjl","cml","tgcsl","ubal","fbrol","bs_pokl","bs_bal","cbjl","chel","nc_bal","bal","frol","frofl","rodzl","aogjbrol","cspljpt","3brgl","bfbl","abwl","7eml","abl","dtl" };
        

        foreach(var game in allGameOnDemo.Data.Games.OrderBy(g => g.GameId))
        {
            Console.WriteLine($"update game {game.GameId} {game.GameCode} in GMS...");
            if (game != null)
            {
                if (availableGameCodes.Contains(game.GameCode))
                {
                    game.IsEnabled = true;
                    game.IsRetired = false;
                    game.IsUnderMaintain = false;
                    game.DisableReason = string.Empty;
                }
                else
                {
                    game.IsEnabled = false;
                    game.IsRetired = false;
                    game.IsUnderMaintain = false;
                    game.Remark += "| This game is not available on Demo from provider.";
                }
            }
            await gMSHelperUat.UpdateGameToGMSByGame(game);
            Console.WriteLine($"Finished adding game {game.GameId} {game.GameCode} in GMS.");
            Console.WriteLine("--------------------------------------------------");
        }
        Console.WriteLine("Finished adding game currencies in GMS.");
    }

    

    private SboGamelist GetAndConvertGamelistFromProvider()
    {
        var qtechGamelist = GetQtechGamelist();
        var sboGamelist = qtechGamelist.Items.Select(i =>
            new Game
            {
                Id = qtechGamelist.Items.IndexOf(i) + 1,
                Name = i.Name,
                ChineseName = "",
                GameCode = i.Id,
                GameCode1 = "",
                Url = i.Images.FirstOrDefault(img => img.Type == "logo-square")?.Url ?? "",
                Category = GetSBOGameCategory(i.Category),
                NewGameType = GetSBONewGameType(i.Category),
                ProviderSupportedCurrencies = i.Currencies?.Select(c => c.Id ?? "").ToList() ?? new List<string>(),
                SubProvider = i.Provider?.Name ?? ""
            }).ToList();
        var sboGamelistWrapper = new SboGamelist { Games = sboGamelist };
        return sboGamelistWrapper;
    }

    private async Task DownLoadImgs(List<Game> gameList)
    {
        // download images from the list of urls into this folder "/Users/pinsopheaktra/Downloads/QtechGameIcons"
        var folderPath = "/Users/pinsopheaktra/Downloads/QtechGameIcons";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        using (var client = new System.Net.Http.HttpClient())
        {
            foreach (var game in gameList)
            {
                try
                {
                    if (string.IsNullOrEmpty(game.Url))
                        continue;
                    
                    // Get the filename from the URL
                    var uri = new Uri(game.Url);
                    var filename = System.IO.Path.GetFileName(uri.AbsolutePath);
                    if (string.IsNullOrEmpty(filename))
                        filename = Guid.NewGuid().ToString();
                    
                    var filePath = System.IO.Path.Combine(folderPath, game.FileName + ".png");
                    
                    // Skip if file already exists
                    if (System.IO.File.Exists(filePath))
                        continue;
                    
                    // Download the image
                    var imageBytes = await client.GetByteArrayAsync(game.Url);
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    
                    Console.WriteLine($"Downloaded: {filename}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download {game}: {ex.Message}");
                }
            }
        }
    }

    private EnumSBONewGameType GetSBONewGameType(string category)
    {
        if (string.IsNullOrEmpty(category))
            return EnumSBONewGameType.Unknown;
        if (category.Contains("Slot", StringComparison.OrdinalIgnoreCase))
            return EnumSBONewGameType.Slots;
        if (category.Contains("TABLEGAME", StringComparison.OrdinalIgnoreCase))
            return EnumSBONewGameType.TableGames;
        return EnumSBONewGameType.OthersGames;
    }

    private EnumSBOGameCategory GetSBOGameCategory(string category)
    {
        if (string.IsNullOrEmpty(category))
            return EnumSBOGameCategory.Unknown;
        if (category.Contains("slot", StringComparison.OrdinalIgnoreCase))
            return EnumSBOGameCategory.Slots;
        if (category.Contains("TABLEGAME", StringComparison.OrdinalIgnoreCase))
            return EnumSBOGameCategory.TableGames;
        return EnumSBOGameCategory.OthersGames;
    }

    private QtechGameResponse GetQtechGamelist()
    {
        var json = System.IO.File.ReadAllText("/Users/pinsopheaktra/Downloads/qtechGamelist.json");
        var qtechGamelist = Newtonsoft.Json.JsonConvert.DeserializeObject<QtechGameResponse>(json);
        return qtechGamelist;
    }

    
}
