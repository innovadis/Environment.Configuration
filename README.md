# Environment.Configuration
[![NuGet](https://img.shields.io/nuget/v/Environment.Configuration.svg)](https://www.nuget.org/packages/Environment.Configuration/)

Adds strictly declared configuration to your project when using environment variables.

## Usage
### Install package
Run the following command in your project root: `dotnet add package Environment.Configuration`

### Create your configuration
Assuming the following environment variables:
```bash
MY_CONNECTION_STRING=some://connection.string
MY_BOOLEAN_SETTING=false
```

```csharp
public class MyConfiguration : ConfigurationSection
{
    private const string Path = "MyConfguration";

    public MyConfiguration(IConfiguration root) : base(root, Path)
    {
    }

    [ConfigurationProperty("MY_CONNECTION_STRING", IsRequired = true)]
    public string MyConnectionString => Convert.ToString(this["MY_CONNECTION_STRING"]);

    [ConfigurationProperty("MY_BOOLEAN_SETTING", IsRequired = false]]
    public bool MyBooleanSetting => Convert.ToBoolean(this["MY_BOOLEAN_SETTING"]);

    // etc.
}
```

### Use your configuration
```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public  IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var myConfig = Configuration.GetEnvironmentSection<MyConfiguration>();

        services.AddDbContext<SqlDatabaseContext>(options => 
        {
            options.UseSqlServer(myConfig.MyConnectionString);
        });

        // etc.
    }
}
```