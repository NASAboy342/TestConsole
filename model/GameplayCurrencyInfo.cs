using System;

namespace TestConsole.model;

public class GameplayCurrencyInfo
{
	public int Id { get; set; }
	public int ProviderId { get; set; }
	public int SubProviderId { get; set; }
	public string CustomerCurrency { get; set; }
	public string ProviderCurrency { get; set; }
	public decimal ExchangeRate { get; set; }
	public string Remarks { get; set; }
	public bool IsEnabled { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedOn { get; set; }
	public string ModifiedBy { get; set; }
	public DateTime ModifiedOn { get; set; }
}
