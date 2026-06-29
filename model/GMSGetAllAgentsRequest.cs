namespace TestConsole.model;

public class GMSGetAllAgentsRequest : GMSBaseRequest
{
    public string ModifiedBy { get; set; }
    public bool IsGetForAllProviders { get; set; }
    public int ProviderId { get; set; }
    public bool IsReloadCache { get; set; }
    public PagenationProperty PagenationProperty { get; set; }
}

public class PagenationProperty
{
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
    public int Pages { get; set; }
}
