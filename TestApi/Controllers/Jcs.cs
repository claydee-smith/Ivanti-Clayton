using Microsoft.AspNetCore.Mvc;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JcsController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        //public JcsController(ILogger<WeatherForecastController> logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet("MyJcs")]
        public IEnumerable<string> GetJcs()
        {
            return new List<string> { "Jcs", "deeDee" };
        }
    }
}