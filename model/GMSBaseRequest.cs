using System;

namespace TestConsole.model;

public class GMSBaseRequest
{
    public string SessionToken { get; set; }
    public string SecretKey = "StagingSecretKey";
}
