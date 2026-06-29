using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TestConsole.model;

public class GetProviderInfoResponse
{
	[JsonPropertyName("data")]
	public List<ProviderInfoData> Data { get; set; } = new();

	[JsonPropertyName("error")]
	public ProviderInfoError Error { get; set; } = new();

	[JsonPropertyName("serverId")]
	public object? ServerId { get; set; }
}

public class ProviderInfoData
{
	[JsonPropertyName("gpId")]
	public int GpId { get; set; }

	[JsonPropertyName("providerName")]
	public ProviderName ProviderName { get; set; } = new();

	[JsonPropertyName("supportCurrency")]
	public List<string> SupportCurrency { get; set; } = new();

	[JsonPropertyName("supportLanguage")]
	public List<string> SupportLanguage { get; set; } = new();

	[JsonPropertyName("currencyRegion")]
	public Dictionary<string, List<string>> CurrencyRegion { get; set; } = new();

	[JsonPropertyName("umInfo")]
	public UmInfo UmInfo { get; set; } = new();

	[JsonPropertyName("providerType")]
	public string ProviderType { get; set; } = string.Empty;

	[JsonPropertyName("latamSupportCurrency")]
	public List<string> LatamSupportCurrency { get; set; } = new();

	[JsonPropertyName("blockCountries")]
	public string BlockCountries { get; set; } = string.Empty;

	[JsonPropertyName("currencyRegionRestrictions")]
	public object? CurrencyRegionRestrictions { get; set; }

	[JsonPropertyName("canPlaceBetOnBothSide")]
	public bool CanPlaceBetOnBothSide { get; set; }

	[JsonPropertyName("haveCommissionStake")]
	public bool HaveCommissionStake { get; set; }

	[JsonPropertyName("betDetailApi")]
	public bool BetDetailApi { get; set; }

	[JsonPropertyName("isCanLoginByGameId")]
	public bool IsCanLoginByGameId { get; set; }

	[JsonPropertyName("betLimitFeatureProd")]
	public bool BetLimitFeatureProd { get; set; }

	[JsonPropertyName("isEnabled")]
	public bool IsEnabled { get; set; }
}

public class ProviderName
{
	[JsonPropertyName("cn")]
	public string Cn { get; set; } = string.Empty;

	[JsonPropertyName("en")]
	public string En { get; set; } = string.Empty;
}

public class UmInfo
{
	[JsonPropertyName("isUM")]
	public bool IsUM { get; set; }

	[JsonPropertyName("startTime")]
	public DateTime? StartTime { get; set; }

	[JsonPropertyName("endTime")]
	public DateTime? EndTime { get; set; }
}

public class ProviderInfoError
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("msg")]
	public string Msg { get; set; } = string.Empty;
}
