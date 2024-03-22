using Microsoft.Extensions.Configuration;

namespace Jolly.Research.Configuration.Explore;

public static class CfgExpl
{
    public static string ToString(IConfiguration configuration)
    {
        var configurationExplorer = new ConfigurationExplorer(configuration);
        return configurationExplorer.Serialize();
    }
}