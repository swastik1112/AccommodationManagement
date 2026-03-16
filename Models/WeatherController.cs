using Microsoft.AspNetCore.Mvc;

namespace AccommodationManagement.Models
{
    public class WeatherController : Controller
    {
        private readonly HttpClient _httpClient;

        public WeatherController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> GetWeather()
        {
            string apiKey = "5f41914a1e69d6a22ef1184573f65fb9";
            string city = "Nagpur";

            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

            var response = await _httpClient.GetStringAsync(url);

            return Content(response, "application/json");
        }
    }
}
