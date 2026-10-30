namespace Howest.Repositories;

public abstract class BaseRepository
{
    protected CosmosClient _cosmosClient;
    protected readonly string databaseId = "SecurityLabo";
    protected readonly string containerId = "person";

    public BaseRepository(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }
}

public interface ICosmosRepository
{
    Task AddPersonAsync(Person person);
}

public class CosmosRepository : BaseRepository, ICosmosRepository
{
    public CosmosRepository(CosmosClient cosmosClient) : base(cosmosClient) { }

    public async Task AddPersonAsync(Person person)
    {
        var database = _cosmosClient.GetDatabase(databaseId);
        var container = database.GetContainer(containerId);
        await container.CreateItemAsync(person, new PartitionKey(person.Id));
    }
}
