namespace Howest.Functions;

public class GetImageLink
{
    private readonly IAppService _appService;

    public GetImageLink(IAppService appService)
    {
        _appService = appService;
    }

    [Function("GetImageLink")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        string? blobName = req.Query["name"];

        if (string.IsNullOrWhiteSpace(blobName))
        {
            return new BadRequestObjectResult("Pass the blob name as ?name=...");
        }

        var link = await _appService.GetImageLinkAsync(blobName);

        return new OkObjectResult(new
        {
            url = link.ToString(),
            expiresInMinutes = 2
        });
    }
}
