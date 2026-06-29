namespace TestConsole.model;

public class GMSGetAllAgentsResponse : GMSBaseResponse
{
    public GetAllAgentsData Data { get; set; }
}

public class GetAllAgentsData
{
    public List<ProviderAgentInfo> ProviderAgentInfos { get; set; }
    public PagenationProperty PagenationProperty { get; set; }
}

public class ProviderAgentInfo
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
    public string ProviderName { get; set; }
}
