# kochi
Client for Fukuoka logging service.

## Description
Client integrated to HttpClient as a handler and sends logs to Fukuoka.
Errors that occur are logged using project ILogger

### Options
- Endpoint - required option, specifies where to send logs.
- LoggingCondition - this option allows to logs only the necessary requests. If not set logs all requests

## Usage
Add request logger to your http client
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        # Read Fukuoka options from Vault
        var fukuokaOptions = vaultClient.Get(Configuration["Fukuoka:Options"]).GetAwaiter().GetResult(); 
        services.AddHttpClient("ClientName")
            .AddHttpRequestLoggingHandler(options =>
            {
                # Set Fukuoka endpoint for client
                options.Endpoint = fukuokaOptions["endpoint"];
                # Log only requests which RequestBody contains HotelBookingRequest
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
Add settings to appsettings.json for getting Fukuoka options from Vault
```json
{
  "Fukuoka": {
    "Options": "fukuoka/options"
  }
}
```
Add access to "fukuoka/options" in Vault policy. Open "Policies" in Vault UI and add
```hcl
path "secrets/data/fukuoka/options" {
  capabilities = [ "read" ]
}
```
