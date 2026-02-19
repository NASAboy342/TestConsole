using System;

namespace TestConsole.model;

public class GetProviderCurrencyInfoRequest
{
	public bool IsGetForAllProvider { get; set; }
	public int ProviderId { get; set; }
	public bool IsReloadCache { get; set; }
	public bool IsIncludeDisabled { get; set; }
}
