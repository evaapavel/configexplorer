using System.Numerics;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Jolly.Research.Configuration.Explore;

/// <summary>
/// Helps to serialize the <see cref="IConfiguration"/> object.
/// </summary>
public class ConfigurationExplorer
{
    private const int TabSize = 2;
    
    private readonly IConfiguration _configuration;

    public ConfigurationExplorer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Serialize()
    {
        var stringBuilder = new StringBuilder();
        Serialize(_configuration, 0, stringBuilder);
        return stringBuilder.ToString();
    }

    private void Serialize(IConfiguration configurationPart, int indentation, StringBuilder stringBuilder)
    {
        switch (configurationPart)
        {
            case IConfigurationRoot configurationRoot:
                SerializeRoot(configurationRoot, indentation, stringBuilder);
                break;
            case IConfigurationSection configurationSection:
                SerializeSection(configurationSection, indentation, stringBuilder);
                break;
            default:
                throw new NotSupportedException(
                    $"This configuration object is not supported here: {configurationPart.GetType().FullName}");
        }
    }

    private void SerializeSection(IConfigurationSection configurationSection, int indentation, StringBuilder stringBuilder)
    {
        if (configurationSection.GetChildren().Any())
        {
            stringBuilder
                .AppendFormat("{0}\"{1}\": {{", IndentSpaces(indentation), configurationSection.Key)
                .AppendLine();

            foreach (var child in configurationSection.GetChildren())
            {
                Serialize(child, indentation + TabSize, stringBuilder);
            }
            
            stringBuilder
                .AppendFormat("{0}}}", IndentSpaces(indentation))
                .AppendLine();
        }

        stringBuilder
            .AppendFormat("{0}\"{1}\": {2}", IndentSpaces(indentation), configurationSection.Key, SerializeValue(configurationSection.Value))
            .AppendLine();
    }

    private void SerializeRoot(IConfigurationRoot configurationRoot, int indentation, StringBuilder stringBuilder)
    {
        stringBuilder.AppendLine("{");

        foreach (var child in configurationRoot.GetChildren())
        {
            Serialize(child, indentation + TabSize, stringBuilder);
        }
        
        stringBuilder.AppendLine("}");
    }

    private string IndentSpaces(int indentation)
    {
        return string.Empty.PadRight(indentation);
    }

    private string SerializeValue(string? value)
    {
        if (value == null)
        {
            return "null";
        }

        if (IsBool(value))
        {
            return value;
        }

        if (IsNumber(value))
        {
            return value;
        }

        return "\"" + value + "\"";
    }
    
    private bool IsNumber(string? value)
    {
        return double.TryParse(value, out _);
    }

    private bool IsBool(string? value)
    {
        return bool.TryParse(value, out _);
    }
}