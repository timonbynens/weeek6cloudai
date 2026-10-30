namespace Howest.Repositories;

public interface IBlobRepository
{
    Task UploadBlobAsync(string containerName, string blobName, byte[] data);
    Task<Uri> GetReadLinkAsync(string containerName, string blobName, TimeSpan validFor);
}

public class BlobRepository : IBlobRepository
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobRepository(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task UploadBlobAsync(string containerName, string blobName, byte[] data)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(blobName);
        using var stream = new MemoryStream(data);
        await blobClient.UploadAsync(stream, overwrite: true);
    }

    // Exercise 4: user delegation SAS, signed with a key Entra ID issues to our identity
    public async Task<Uri> GetReadLinkAsync(string containerName, string blobName, TimeSpan validFor)
    {
        var expiresOn = DateTimeOffset.UtcNow.Add(validFor);

        // SDK 12.30+ takes an options object instead of (startsOn, expiresOn)
        UserDelegationKey key = await _blobServiceClient.GetUserDelegationKeyAsync(
            new BlobGetUserDelegationKeyOptions(expiresOn)
            {
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5)   // a little slack for clock skew
            });

        var sas = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",                           // "b" = one blob
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresOn = expiresOn
        };
        sas.SetPermissions(BlobSasPermissions.Read);

        var token = sas.ToSasQueryParameters(key, _blobServiceClient.AccountName);

        var blobUri = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName)
            .Uri;

        return new UriBuilder(blobUri) { Query = token.ToString() }.Uri;
    }
}
