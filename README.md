# ConfigExplorer

Explores Microsoft.Extensions.Configuration.IConfiguration objects


Example usage:

```C#
var configuration = GetRequiredService<IConfiguration>();
System.Diagnostics.Debug.WriteLine(Jolly.Research.Configuration.Explore.CfgExpl.ToString(configuration));
```


