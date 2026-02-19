using System;
using TestConsole.Enums;

namespace TestConsole.model;

public class SboGamelist
{
    public List<Game> Games { get; set; }
}

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ChineseName { get; set; }
    public string GameCode { get; set; }
    public string GameCode1 { get; set; }
    public string Url { get; set; }
    public string SubProvider { get; set; }
    public EnumSBOGameCategory Category { get; set; }
    public EnumSBONewGameType NewGameType { get; set; }
    public string FileName => GetFileName();

    private string GetFileName()
    {
        var nameWithoutSpecialChars = new string(Name.Where(c => char.IsLetterOrDigit(c)).ToArray());
        return $"1102_{Id}_{nameWithoutSpecialChars}";
    }

    public List<string> ProviderSupportedCurrencies { get; set; } = new List<string>();
}
