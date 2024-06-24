using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // classe per la gestione dei log
        private readonly ILogger<WeatherForecastController> _logger;

        // costruttore per l'inizializzazione del logger
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            // esempio di utilizzo del logger (scrittura di un log)
            _logger.LogInformation("Getting weather forecast");

            try
            {
                // Lancio di un'eccezione per testare la scrittura di un log di errore
                // throw new Exception("This is an logging test exception");

                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            .ToArray();
            }
            catch (Exception ex)
            {
                // esempio di utilizzo del logger (scrittura di un log di errore)
                _logger.LogError(ex, "An error occurred while getting weather forecast");
                throw;
            }

        }
    }
}
