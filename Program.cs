using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Presentation;
using IEnumerable.ForEach;
using LibGit2Sharp;
using LLama;
using LLama.Common;
using LLama.Native;
using MiNET.UI;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Org.BouncyCastle.Asn1.Mozilla;
using Org.BouncyCastle.Crmf;
using SixLabors.ImageSharp.Processing;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using TestConsole;
using TestConsole.Helper;
using TestConsole.Programs;
using TestConsole.Tests;
using Path = System.IO.Path;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();
        
        // Add browser-like headers to avoid 403 Forbidden errors
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Accept", "image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        httpClient.DefaultRequestHeaders.Add("Referer", "https://storage.dc-ace.com/");
        
        var providerId = 1096;
        var providerGames = await GetProviderGames(httpClient);
        var allGamesFromLandopia = await CallToLandopiaToGetAllGames(httpClient, providerId);
        await ProccessGameIcon(httpClient, providerGames, providerId, allGamesFromLandopia);

    }

    private static async Task<List<ProviderGame>?> GetProviderGames(HttpClient httpClient)
    {
        var file = File.ReadAllText("/Users/pinsopheaktra/Downloads/Untitled-1.json");
        var hacksawGames = JsonConvert.DeserializeObject<GameResponse>(file);
        var providerGames = hacksawGames?.Data
            .Select(g => new ProviderGame
            {
                gameId = g.GameId.ToString(),
                name = g.GameName,
                image = g.GameIcon
            })
            .ToList();
        return providerGames;
    }

    private static async Task ProccessGameIcon(HttpClient httpClient, List<ProviderGame>? providerGames, int providerId, Dictionary<string, List<PlatformGame>>? allGamesFromLandopia)
    {
        if (allGamesFromLandopia == null)
        {
            Console.WriteLine("No games found from Landopia.");
            return;
        }
        var providerName = allGamesFromLandopia[providerId.ToString()].FirstOrDefault()?.GameProviderName ?? "Unknown";

        Console.WriteLine($"🎯 {providerName} Games:");
        foreach (var game in allGamesFromLandopia[providerId.ToString()])
        {
            foreach (var gamenameinfo in game.GameNameInfos)
            {
                Console.WriteLine($"- {game.GameId}: {gamenameinfo.GameName} ({game.GameCode})");
                var url = providerGames.FirstOrDefault(g => g.gameId == game.GameCode)?.image ?? "";
                if (string.IsNullOrEmpty(url))
                {
                    Console.WriteLine($"No image URL found for GameId {game.GameId}");
                    continue;
                }
                try
                    {
                        var downloadsFolder = GetFolder(providerName, game);
                        var fileName = GetFileName(game, gamenameinfo);
                        var filePath = Path.Combine(downloadsFolder, fileName);

                        var imageBytes = await httpClient.GetByteArrayAsync(url);
                        imageBytes = ResizedImage(imageBytes);

                        await File.WriteAllBytesAsync(filePath, imageBytes);

                        Console.WriteLine($"Downloaded: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to download image for GameId {game.GameId}: {ex.Message}");
                    }
            }
        }
    }

    private static byte[] ResizedImage(byte[] imageBytes)
    {
        using (var inputStream = new MemoryStream(imageBytes))
        using (var image = SixLabors.ImageSharp.Image.Load(inputStream))
        {
            image.Mutate(x => x.Resize(200, 200));
            using (var outputStream = new MemoryStream())
            {
                image.Save(outputStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                imageBytes = outputStream.ToArray();
            }
        }

        return imageBytes;
    }

    private static string GetFileName(PlatformGame game, GameNameInfo gamenameinfo)
    {
        var imgUrl = gamenameinfo.IconUrl;
        var uri = new Uri(imgUrl);
        var fileName = Path.GetFileName(uri.LocalPath);
        return fileName;
    }

    private static string GetFolder(string providerName, PlatformGame game)
    {
        var userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var folderPath = Path.Combine(userProfileFolder,"Downloads", providerName, game.GameId.ToString());
        Directory.CreateDirectory(folderPath);
        return folderPath;
    }

    private static async Task<Dictionary<string, List<PlatformGame>>?> CallToLandopiaToGetAllGames(HttpClient httpClient, int providerId)
    {
        var postUrl = "https://capi-uat-land.techbodia.dev/Common/GetAllGames";
        var postBody = new
        {
            gpId = providerId,
            isGetAll = false,
            webId = 0
        };
        string jsonBody = JsonConvert.SerializeObject(postBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var postResponse = await httpClient.PostAsync(postUrl, content);
        var postJson = await postResponse.Content.ReadAsStringAsync();
        var allGames = JsonConvert.DeserializeObject<Dictionary<string, List<PlatformGame>>>(postJson);
        return allGames;
    }
}


// Models for first API
public class ProviderGame
{
    public string gameId { get; set; }
    public string name { get; set; }
    public string image { get; set; }
}

// Models for second API
public class PlatformGame
{
    public string GameProviderName { get; set; }
    public int GameProviderId { get; set; }
    public int providerId { get; set; }
    public string providerStatus { get; set; }
    public int GameId { get; set; }
    public List<GameNameInfo> GameNameInfos { get; set; } = new();
    public int GameCategory { get; set; }
    public int NewGameType { get; set; }
    public int Rank { get; set; }
    public string GameCode { get; set; }
    public string GameCode1 { get; set; }
    public string Device { get; set; }
    public string Platform { get; set; }
    public string SubProvider { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsUm { get; set; }
    public bool IsRetired { get; set; }
    public string Remark { get; set; }
    public bool IsJackpot { get; set; }
    public int Rows { get; set; }
    public int Reels { get; set; }
    public int Lines { get; set; }
    public double RTP { get; set; }
    public bool IsProvideCommission { get; set; }
    public bool HasHedgeBet { get; set; }
    public bool HasBuyFreeSpin { get; set; }
    public List<string> SupportedCurrencies { get; set; } = new();
    public List<string> BlockCountries { get; set; } = new();
    public bool IsNewGame { get; set; }
    public DateTime modifiedOn { get; set; }
    public string disableReason { get; set; }
}

public class GameNameInfo
{
    public string Language { get; set; }
    public string GameName { get; set; }
    public string IconUrl { get; set; }
}
