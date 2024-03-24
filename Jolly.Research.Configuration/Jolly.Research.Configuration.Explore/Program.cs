using Microsoft.Extensions.Configuration;

namespace Jolly.Research.Configuration.Explore;

/// <summary>
/// Exposes the main application entry point.
/// </summary>
public class Program
{
    /// <summary>
    /// Main application entry point.
    /// </summary>
    /// <param name="args">Command line parameters.</param>
    public static void Main(string[] args)
    {
        var configuration = GetRequiredService<IConfiguration>();
        System.Diagnostics.Debug.WriteLine(Jolly.Research.Configuration.Explore.CfgExpl.ToString(configuration));
    }

    // Dummy.
    // Provide an implementation of your own.
    private static T GetRequiredService<T>()
    {
        return default(T)!;
    }
}