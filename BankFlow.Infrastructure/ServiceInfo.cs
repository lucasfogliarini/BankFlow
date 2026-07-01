using System.Reflection;

namespace BankFlow.Infrastructure;

public class ServiceInfo(string name, string version)
{
    public string Name { get; set; } = name;
    public string Version { get; set; } = version;
    public static ServiceInfo Get()
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName();
        var serviceName = assemblyName?.Name ?? "Unknown Service Name";
        var serviceVersion = assemblyName?.Version?.ToString() ?? "Unknown Version";

        return new ServiceInfo(serviceName, serviceVersion);
    }
}


