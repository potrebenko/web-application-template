namespace Web.Api.Configuration;

public class ServiceConfig
{
    public string Path { get; set; } = "/listallservices";

    public List<ServiceDescriptor> Services { get; set; } = new();
}