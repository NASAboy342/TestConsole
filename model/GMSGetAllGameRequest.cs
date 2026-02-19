using System;

namespace TestConsole.model;

public class GMSGetAllGameRequest : GMSBaseRequest
{
    public int ProviderId { get; set; }
    public bool isForSA { get; set; }
}
