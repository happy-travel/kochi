# kochi
Library adds the ability to log HTTP requests and responses

## Usage
Add request logger to your http client
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var fukuokaOptions = vaultClient.Get(Configuration["Fukuoka:Options"]).GetAwaiter().GetResult(); 
        services.AddHttpClient("ClientName")
            .AddHttpRequestLoggingHandler(options =>
            {
                options.Endpoint = fukuokaOptions["endpoint"];
                options.LoggingCondition = request =>
                {
                    var content = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                    return !string.IsNullOrEmpty(content) && content.Contains("HotelBookingRequest");
                };
            });
        ...
    }
}
```
Add settings to appsettings.json
```json
{
  "Fukuoka": {
    "Options": "fukuoka/options"
  }
}
```
Add access to "fukuoka/options" in vault policy. Open "Policies" in Vault UI and add
```hcl
path "secrets/data/fukuoka/options" {
  capabilities = [ "read" ]
}
```

