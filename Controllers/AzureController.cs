using chatbot_agentic.Services;
using Microsoft.AspNetCore.Mvc;

namespace chatbot_agentic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AzureController : Controller
    {
        private readonly IAzureService _azureService;

        public AzureController(IAzureService azureService)
        {
            _azureService = azureService;
        }

        [HttpPost]
        [Route("AskQuestion")]
        public async Task<IActionResult> AskQuestion([FromForm] string prompt, [FromForm] IFormFile? file)
        {
            string documentId = string.Empty;

            if (file != null)
            {
                using (var fileStream = new MemoryStream())
                {
                    await file.CopyToAsync(fileStream);
                    documentId = await _azureService.ImportDocumentAsync(fileStream, file.FileName);
                }
            }

            var response = await _azureService.AskQuestion(prompt, documentId);

            return Ok(response);
        }
    }
}
