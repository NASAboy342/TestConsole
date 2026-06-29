namespace TestConsole.model;

public class GMSUpdateAgentRequest : GMSBaseRequest
{
    public string ModifiedBy { get; set; }
    public List<AgentInfoUpdate> AgentInfos { get; set; }
}

public class AgentInfoUpdate
{
    public int ProviderId { get; set; }
    public string Currency { get; set; }
    public string ApiUrl { get; set; }
    public string ApiUrl1 { get; set; }
    public string AgentId { get; set; }
    public string AgentName { get; set; }
    public string Cert { get; set; }
    public string Cert1 { get; set; }
    public int WebId { get; set; }
    public string CreatedBy { get; set; }
    public string ModifiedBy { get; set; }
}
