using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using server.Models;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemperatureController : ControllerBase
    {
        public const string SensorServiceUrl = "https://temperature-sensor-service.herokuapp.com/sensor/{0}";

        private readonly IHttpClientFactory _httpClientFactory;

        public TemperatureController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([Required(AllowEmptyStrings = false)]string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync(string.Format(SensorServiceUrl, Uri.EscapeDataString(id)));
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var sensorData = JsonSerializer.Deserialize<Sensor>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    return Ok(sensorData);
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
        }
    }
}
