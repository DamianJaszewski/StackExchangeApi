using Microsoft.AspNetCore.Mvc;
using StackExchangeApi.Models;
using StackExchangeApi.Services;
namespace StackExchangeApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost("populate")]
        public async Task<IActionResult> PopulateData()
        {
            await _tagService.PopulateDataAsync();
            return Ok("Data populated successfully.");
        }

        [HttpGet("percentage")]
        public async Task<IActionResult> GetTagsPercentage()
        {
            var tagsPercentage = await _tagService.CalculateTagsPercentageAsync();
            return Ok(tagsPercentage);
        }

        [HttpGet("paginate")]
        public async Task<IActionResult> GetPaginatedTags([FromQuery] TagQueryParams queryParams)
        {
            var paginatedTags = await _tagService.GetPaginatedTagsAsync(queryParams);
            return Ok(paginatedTags);
        }
    }
}
