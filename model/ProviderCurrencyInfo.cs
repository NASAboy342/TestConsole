using System;

namespace TestConsole.model;

public class ProviderCurrencyInfo
{
	public List<CurrencyInfo> ProviderCurrencyInfos { get; set; }
}

public class CurrencyInfo
{
    public int ProviderId { get; set; }
	public int SubProviderId { get; set; }
	public string CustomerCurrency { get; set; }
	public decimal ExchangeRate { get; set; }
	public string ProviderCurrency { get; set; }
	public string Remarks { get; set; }
	public bool IsEnabled { get; set; }
}

public class ProviderCurrencyInfoResponse
{
    public ProviderCurrencyInfo Data { get; set; }
}