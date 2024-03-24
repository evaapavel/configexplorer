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
    private StringBuilder _stringBuilder = null!;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="configuration">Root of the configuration object graph to serialize.</param>
    public ConfigurationExplorer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Serializes the configuration to a string. Beautifies the output to make it more human readable.
    /// </summary>
    /// <returns>Returns a string representation of the configuration stored in this instance.</returns>
    public string Serialize()
    {
        _stringBuilder = new StringBuilder();
        Serialize(_configuration, 0);
        return _stringBuilder.ToString();
    }

    private void Serialize(IConfiguration configurationPart, int indentation)
    {
        switch (configurationPart)
        {
            case IConfigurationRoot configurationRoot:
                SerializeRoot(configurationRoot, indentation);
                break;
            case IConfigurationSection configurationSection:
                SerializeSection(configurationSection, indentation);
                break;
            default:
                throw new NotSupportedException(
                    $"This configuration object is not supported here: {configurationPart.GetType().FullName}");
        }
    }

    private void SerializeRoot(IConfigurationRoot configurationRoot, int indentation)
    {
        _stringBuilder.AppendLine("{");

        foreach (var child in configurationRoot.GetChildren())
        {
            Serialize(child, indentation + TabSize);
        }
        
        _stringBuilder.AppendLine("}");
    }
    
    private void SerializeSection(IConfigurationSection configurationSection, int indentation)
    {
        if (IsSingleValueSection(configurationSection))
        {
            SerializeSingleValueSection(configurationSection, indentation);
            return;
        }

        if (IsArraySection(configurationSection))
        {
            SerializeArraySection(configurationSection, indentation);
            return;
        }
        
        SerializeSectionWithChildren(configurationSection, indentation);
    }

    private void SerializeSingleValueSection(IConfigurationSection configurationSection, int indentation, bool includeKey = true)
    {
        _stringBuilder
            .AppendFormat("{0}{1}{2},",
                IndentSpaces(indentation),
                includeKey ? ("\"" + configurationSection.Key + "\": ") : (""),
                SerializeValue(configurationSection.Value))
            .AppendLine();
    }

    private void SerializeArraySection(IConfigurationSection configurationSection, int indentation, bool includeKey = true)
    {
        _stringBuilder
            .AppendFormat("{0}{1}[",
                IndentSpaces(indentation),
                includeKey ? ("\"" + configurationSection.Key + "\": ") : (""))
            .AppendLine();

        foreach (var child in configurationSection.GetChildren())
        {
            SerializeArrayItem(child, indentation + TabSize);
        }
        
        _stringBuilder
            .AppendFormat("{0}],", IndentSpaces(indentation))
            .AppendLine();
    }
    
    private void SerializeArrayItem(IConfigurationSection configurationSection, int indentation)
    {
        if (IsSingleValueSection(configurationSection))
        {
            SerializeSingleValueSection(configurationSection, indentation, includeKey: false);
            return;
        }

        if (IsArraySection(configurationSection))
        {
            SerializeArraySection(configurationSection, indentation, includeKey: false);
            return;
        }
        
        SerializeSectionWithChildren(configurationSection, indentation, includeKey: false);
    }

    private void SerializeSectionWithChildren(IConfigurationSection configurationSection, int indentation, bool includeKey = true)
    {
        _stringBuilder
            .AppendFormat("{0}{1}{{",
                IndentSpaces(indentation),
                includeKey ? ("\"" + configurationSection.Key + "\": ") : (""))
            .AppendLine();

        foreach (var child in configurationSection.GetChildren())
        {
            SerializeSection(child, indentation + TabSize);
        }
        
        _stringBuilder
            .AppendFormat("{0}}},", IndentSpaces(indentation))
            .AppendLine();
    }

    // *********************************************************************************************************
    // Helpers
    // *********************************************************************************************************

    private bool IsSingleValueSection(IConfigurationSection configurationSection)
    {
        return ( ! configurationSection.GetChildren().Any() );
    }

    private bool IsArraySection(IConfigurationSection configurationSection)
    {
        var index = 0;
        foreach (var child in configurationSection.GetChildren())
        {
            if (child.Key != index.ToString())
            {
                return false;
            }

            index++;
        }

        return true;
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