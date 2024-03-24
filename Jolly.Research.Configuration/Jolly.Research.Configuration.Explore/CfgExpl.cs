using Microsoft.Extensions.Configuration;

namespace Jolly.Research.Configuration.Explore;

/// <summary>
/// Static wrapper for the <see cref="ConfigurationExplorer"/> class.
/// </summary>
public static class CfgExpl
{
    /// <summary>
    /// Converts a given complex object with configuration to a human readable string.
    /// </summary>
    /// <param name="configuration">Configuration to convert.</param>
    /// <returns>Returns a string representation of the given configuration object graph.</returns>
    public static string ToString(IConfiguration configuration)
    {
        var configurationExplorer = new ConfigurationExplorer(configuration);
        return configurationExplorer.Serialize();
    }
}