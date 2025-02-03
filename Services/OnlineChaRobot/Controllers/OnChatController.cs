using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineChaRobot.Services.DeepSeek;

namespace OnlineChaRobot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnChatController(IDeepSeekService deepSeekService) : ControllerBase
    {
        private readonly IDeepSeekService deepSeekService = deepSeekService;

        /* DeepSeek */
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Question))
                    return BadRequest(new { message = "سوال نمی‌تواند خالی باشد!" });

                var response = await deepSeekService.GetResponseAsync(request.Question, request.Model);
                return Ok(new { answer = response });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new { message = "درخواست لغو شد!" });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "خطای نامشخصی رخ داده است. لطفاً بعداً امتحان کنید." });
            }
        }

        [HttpGet("history/{model}")]
        public IActionResult GetHistory(string model)
        {
            var history = deepSeekService.GetChatHistory(model);
            return Ok(new { history });
        }
    }

    public record AskRequest(string Question, string? Model = null);












}
