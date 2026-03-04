using System;

namespace TestConsole.model;

public class GMSGetAllGameRequest : GMSBaseRequest
{
    public int ProviderId { get; set; }
    public bool IsForSA { get; set; }
}
