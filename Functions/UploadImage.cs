namespace Howest.Functions;

public class UploadImage
{
    private readonly IAppService _appService;
    private readonly ILogger<UploadImage> _logger;

    public UploadImage(IAppService appService, ILogger<UploadImage> logger)
    {
        _appService = appService;
        _logger = logger;
    }

    [Function("UploadImage")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        if (!req.HasFormContentType)
        {
            return new BadRequestObjectResult("Request must have form content type.");
        }

        var form = await req.ReadFormAsync();
        var file = form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("No file was uploaded.");
        }

        byte[] fileData;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileData = memoryStream.ToArray();
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";

        await _appService.UploadBlobAsync("images", fileName, fileData);

        _logger.LogInformation("Successfully uploaded file: {fileName}", fileName);

        return new OkObjectResult(new
        {
            message = "File uploaded successfully",
            fileName,
            size = fileData.Length
        });
    }
}
