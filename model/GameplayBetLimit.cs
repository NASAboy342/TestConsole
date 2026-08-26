using System;

namespace TestConsole.model;

public class GameplayBetLimit
{
	public int Id { get; set; }
	public int ProviderId { get; set; }
	public string Currency { get; set; }
	public string BetLimitCode { get; set; }
	public string ProviderBetLimitCode { get; set; }
	public long MinBet { get; set; }
	public long MaxBet { get; set; }
	public long MaxPerMatch { get; set; }
	public string Remark { get; set; }
	public DateTime CreatedOn { get; set; }
	public string CreatedBy { get; set; }
	public DateTime ModifiedOn { get; set; }
	public string ModifiedBy { get; set; }
	public bool IsEnabled { get; set; }
	public bool IsDefault { get; set; }
	public string GameType { get; set; }
}
