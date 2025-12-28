using Microsoft.AspNetCore.Mvc;
using DataRetriever.Services.Providers; // IDataProvider
using System;
using System.Text.Json;

namespace DataRetriever.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IDataProvider<string, string> _dataProvider;

        public DataController(IDataProvider<string, string> dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Retrieves data by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the data.</param>
        /// <returns>The requested data if found, otherwise 404 Not Found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var (found, value) = await _dataProvider.GetAsync(id);
            if (!found)
                return NotFound();

            return Ok(new { Id = id, Value = value });
        }

        /// <summary>
        /// Creates new data entry.
        /// </summary>
        /// <param name="value">The value of the data to save.</param>
        /// <returns>The ID of the newly created data.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement request)
        {
            string jsonString = JsonSerializer.Serialize(request.GetProperty("value"));

            var id = Guid.NewGuid().ToString();
            await _dataProvider.SaveAsync(id, jsonString);

            return Ok(new { Id = id });
        }
    }
}
