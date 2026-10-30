namespace Howest.Functions;

public class AddPerson
{
    private readonly IAppService _appService;
    private readonly ILogger<AddPerson> _logger;

    public AddPerson(IAppService appService, ILogger<AddPerson> logger)
    {
        _appService = appService;
        _logger = logger;
    }

    [Function("AddPerson")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        var person = await req.ReadFromJsonAsync<Person>();

        if (person == null)
        {
            return new BadRequestObjectResult("Request body must be a person.");
        }

        await _appService.AddPersonAsync(person);

        _logger.LogInformation("Added person {id}", person.Id);

        return new OkObjectResult(person);
    }
}
