using System;
using System.Collections.Generic;

namespace TestConsole.model;

public class QtechGameResponse
{
    public List<QtechGame> Items { get; set; }
}

public class QtechGame
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Provider Provider { get; set; }
    public string Description { get; set; }
    public List<Language> Languages { get; set; }
    public List<Currency> Currencies { get; set; }
    public List<Theme> Themes { get; set; }
    public List<Feature> Features { get; set; }
    public Volatility Volatility { get; set; }
    public string Category { get; set; }
    public List<SupportedDevice> SupportedDevices { get; set; }
    public List<string> ClientTypes { get; set; }
    public bool DemoSupport { get; set; }
    public bool FreeRoundSupport { get; set; }
    public string ReleaseDate { get; set; }
    public List<Image> Images { get; set; }
    public string Rtp { get; set; }
    public string MaxWinBetMultiplier { get; set; }
    public List<string> PortraitOrientationSupport { get; set; }
    public string ReleaseDateTime { get; set; }
}

public class Provider
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Language
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Currency
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Theme
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Feature
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Volatility
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class SupportedDevice
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Image
{
    public string Type { get; set; }
    public string Url { get; set; }
}
