namespace Howest.Functions;

public class GetSmtpServer
{
    private readonly IAppService _appService;

    public GetSmtpServer(IAppService appService)
    {
        _appService = appService;
    }

    [Function("GetSmtpServer")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var smtpServer = await _appService.GetSmtpServer();
        return new OkObjectResult(new { smtpServer });
    }
}
