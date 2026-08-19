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
        // var gmsSessionToken = "c12e42a7c9414";
        // var uatUrl = "http://gms-api-uat.remotes.local";
        // var gMSHelperUat = new GMSHelper(gmsSessionToken, uatUrl);
        // Console.WriteLine("Getting all game info from GMS...");
        // var allGameOnDemo = await gMSHelperUat.GetAllGameAsync(1018);
        // Console.WriteLine($"Finished getting all game info from GMS. Total games: {allGameOnDemo.Data.Games.Count}");
        // // var providerInfo = await gMSHelper.GetProviderInfoAsync(1058);

        // var availableGameCodes = new List<string> { "zeus", "gpas_bprog_pop", "gpas_wlinx_pop", "gpas_gstorm2_pop", "gpas_drise_pop", "gpas_fmhitbar_pop", "gpas_kgomoon_pop", "gpas_sbullet_pop", "gpas_mforest_pop", "gpas_cchfortune_pop", "gpas_sharks_pop", "gpas_bbmways_pop", "gpas_wcrusade_pop", "gpas_bcash_pop", "gpas_vemptr_pop", "gpas_etpop_pop", "gpas_grush_pop", "gpas_qccharm_pop", "gpas_gwizard_pop", "gpas_azboli_pop", "gpas_cxtremepp_pop", "gpas_mgccsahara_pop", "gpas_ggun_pop", "circ", "gpas_rabbitcash_pop", "gpas_focashco_pop", "gpas_fmhitbarpp_pop", "gpas_sking_pop", "gpas_jrush_pop", "gpas_cguard_pop", "gpas_gwish_pop", "gpas_koihar_pop", "gpas_sbullet_pop", "slion", "gpas_gguardians_pop", "gpas_pstrike_pop", "gpas_ceruption_pop", "gpas_ccsahara_pop", "gpas_dostormspp_pop", "gpas_ccskingofa_pop", "gpas_rslot_pop", "gpas_mgbwizard_pop", "gpas_harrow_pop", "gpas_jisland2_pop", "gpas_aogww_pop", "fishshr", "gpas_bcircus_pop", "gpas_tbirds_pop", "gpas_aogwfot_pop", "gpas_ffever_pop", "gpas_ccluck_pop", "gpas_wpisto_pop", "gpas_tqcempt_pop", "gpas_auncoil_pop", "gpas_mblocks_pop", "gpas_gstorm2_pop", "gpas_mkeeper_pop", "gpas_cxtreme_pop", "gpas_drise_pop", "gfal", "gpas_betwildspp_pop", "gpas_fmhitbar_pop", "gtsjxb", "gtsje", "hlf2", "donq", "supro", "ro101", "po", "3cb", "abbj", "car", "ctiv", "grel", "phot", "ashcpl", "bl", "ashlcl", "ashamw", "pmn", "hk", "gos", "ashhotj", "samz", "bob", "whk", "ct", "bib", "ssp", "irl", "catqc", "hb", "zcjb", "cm", "ashglss", "pisa", "ashwnoz", "gts50", "ges", "fxf", "mcb", "gtswg", "mfrt", "mobdt", "eas", "sfh", "lm", "gtsatq", "ashhof", "tpd2", "legwld", "ashicv", "arc", "hlf", "mgstk", "rng2", "gtspor", "ashsbd", "fbr", "strsawk", "scrdstns", "ashjut", "ashjid", "ashjah" }; 


        var gmsSessionTokenProd = "8f358ef66af64";
        var prodUrl = "http://gms-api.remotes.local";
        var gMSHelperProd = new GMSHelper(gmsSessionTokenProd, prodUrl);
        Console.WriteLine("Getting all game info from GMS...");
        var allGameOnProd = await gMSHelperProd.GetAllGameAsync(1029, true);
        Console.WriteLine($"Finished getting all game info from GMS. Total games: {allGameOnProd.Data.Games.Count}");

        var latamCurrencies = new List<string> { "ARS", "BOB", "CLP", "COP", "FKP", "GTQ", "PEN", "PYG", "SRD", "UYU" };

        //foreach(var game in allGameOnProd.Data.Games.OrderBy(g => g.GameId))
        foreach(var game in allGameOnProd.Data.Games.Where(g => g.IsEnabled && g.GameCode.StartsWith("Revolver", StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine($"GameId: {game.GameId}, GameCode: {game.GameCode}, Remark: {game.Remark}");
            game.IsEnabled = false;
            await gMSHelperProd.UpdateGameToGMSByGame(game, true);
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
