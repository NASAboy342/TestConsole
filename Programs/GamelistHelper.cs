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
        var gmsSessionToken = "e8fd848417564";
        var prodUrl = "http://gms-api.remotes.local";
        var gMSHelper = new GMSHelper(gmsSessionToken, prodUrl);
        var allGameInfoFromGMS = await gMSHelper.GetAllGameAsync(1102);
        foreach(var game in allGameInfoFromGMS.Data.Games.OrderBy(g => g.GameId))
        {
            Console.WriteLine($"Updating game {game.GameId} in GMS...");
            game.IsEnabled = false;
            game.DisableReason = "Not support in Asia region.";
            game.Remark = "Not support in Asia region.";
            await gMSHelper.UpdateGameToGMSByGame(game);
            Console.WriteLine($"Finished updating game {game.GameId} in GMS.");
        }
        Console.WriteLine("Finished updating game currencies in GMS.");
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
