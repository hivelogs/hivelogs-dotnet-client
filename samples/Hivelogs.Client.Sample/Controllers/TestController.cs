using Hivelogs.Client.Sample.Dtos;
using Hivelogs.Client.Sample.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hivelogs.Client.Sample.Controllers
{
    [Route("api")]
    [ApiController]
    public class TestController(ITestService testService) : ControllerBase
    {
        ITestService _testService = testService;

        [HttpGet("test")]
        public IActionResult Index()
        {
            return Ok(new { Message = "Hi! It`s a test" });
        }

        [HttpPost("create")]
        public IActionResult Create(CreateDto dto) {
            
            return Ok(dto);
        }

        [HttpGet("exception")]
        public async Task<IActionResult> TestException()
        {
            await _testService.DoWithExceptionAsync();
            return Ok();
        }

        [HttpGet("service")]
        public async Task<IActionResult> TestService()
        {
            await _testService.DoWorkAsync();
            return Ok();
        }
    }
}
