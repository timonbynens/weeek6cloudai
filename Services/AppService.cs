namespace Howest.Services;

public interface IAppService
{
    Task UploadBlobAsync(string containerName, string blobName, byte[] data);
    Task AddPersonAsync(Person person);
    Task<string> GetSmtpServer();
    Task<Uri> GetImageLinkAsync(string blobName);
}

public class AppService : IAppService
{
    private const string ImagesContainer = "images";

    private readonly IBlobRepository _blobRepository;
    private readonly ICosmosRepository _cosmosRepository;
    private readonly IKeyVaultService _keyVaultService;

    public AppService(
        IBlobRepository blobRepository,
        ICosmosRepository cosmosRepository,
        IKeyVaultService keyVaultService)
    {
        _blobRepository = blobRepository;
        _cosmosRepository = cosmosRepository;
        _keyVaultService = keyVaultService;
    }

    public async Task UploadBlobAsync(string containerName, string blobName, byte[] data)
    {
        await _blobRepository.UploadBlobAsync(containerName, blobName, data);
    }

    public async Task AddPersonAsync(Person person)
    {
        person.Id = Guid.NewGuid().ToString();
        await _cosmosRepository.AddPersonAsync(person);
    }

    public async Task<string> GetSmtpServer()
    {
        return await _keyVaultService.GetSecretAsync("SmtpServer");
    }

    public async Task<Uri> GetImageLinkAsync(string blobName)
    {
        return await _blobRepository.GetReadLinkAsync(ImagesContainer, blobName, TimeSpan.FromMinutes(2));
    }
}
