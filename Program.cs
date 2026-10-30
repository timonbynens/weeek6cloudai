var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var credentials = new DefaultAzureCredential();   // who am I?

builder.Services
    .AddSingleton<IBlobRepository, BlobRepository>()
    .AddSingleton<ICosmosRepository, CosmosRepository>()
    .AddSingleton<IKeyVaultService, KeyVaultService>()
    .AddSingleton<IAppService, AppService>()
    .AddSingleton<BlobServiceClient>(provider =>
    {
        return new BlobServiceClient(
            new Uri($"https://{builder.Configuration["StorageAccountName"]}.blob.core.windows.net"),
            credentials);
    })
    .AddSingleton<SecretClient>(provider =>
    {
        return new SecretClient(
            new Uri(builder.Configuration["KeyVaultUri"]!),
            credentials);
    })
    .AddSingleton<CosmosClient>(provider =>
    {
        var keyVaultService = provider.GetRequiredService<IKeyVaultService>();
        var cosmosConnectionString =
            keyVaultService.GetSecretAsync("CosmosDbConnectionString").Result;

        // Cosmos needs a lowercase "id" property; camelCase maps Person.Id to it
        return new CosmosClient(cosmosConnectionString, new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });
    });

builder.Build().Run();
