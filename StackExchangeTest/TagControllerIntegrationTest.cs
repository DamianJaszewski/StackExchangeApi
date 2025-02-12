using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using StackExchangeApi.Controllers;
using StackExchangeApi.Models;
using StackExchangeApi.Services;
using StackExchangeApi;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;

namespace StackExchangeTest
{
    public class TagControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly DataContext _context;
        private readonly HttpClient _httpClientMock;
        private readonly Mock<ILogger<TagService>> _loggerMock;
        private readonly TagService _tagService;
        private readonly TagController _tagController;
        private readonly HttpClient _client;

        public TagControllerIntegrationTest(WebApplicationFactory<Program> factory)
        {
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("TagControllerIntegrationTestDb")
                .Options;

            _context = new DataContext(contextOptions);
            _httpClientMock = new HttpClient(new MockHttpMessageHandler());
            _loggerMock = new Mock<ILogger<TagService>>();

            _tagService = new TagService(_httpClientMock, _context, _loggerMock.Object);
            _tagController = new TagController(_tagService);
            _client = factory.CreateDefaultClient();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetTagsPercentage_ReturnOk()
        {
            var response = await _client.GetAsync("/tag/percentage");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetTagsPercentage_GetRequest_ReturnsCorrectPercentages()
        {
            // Arrange
            var items = new List<Item>
            {
                CreateExampleItem(10),
                CreateExampleItem(30),
                CreateExampleItem(60)
            };

            _context.Items.AddRange(items);
            await _context.SaveChangesAsync();

            // Act
            var result = await _tagController.GetTagsPercentage() as OkObjectResult;
            var tagsPercentage = result?.Value as List<TagsPercentage>;

            // Assert
            Assert.NotNull(tagsPercentage);
            Assert.Equal(3, tagsPercentage.Count);
            Assert.Equal("10%", tagsPercentage.First(x => x.TagId == 1).Percentage);
            Assert.Equal("30%", tagsPercentage.First(x => x.TagId == 2).Percentage);
            Assert.Equal("60%", tagsPercentage.First(x => x.TagId == 3).Percentage);
        }

        [Fact]
        public async Task GetPaginatedTagsAsync_ReturnsCorrectPagination()
        {
            // Arrange

            List<Item> items = new List<Item>()
            {
                CreateExampleItem(10),
                CreateExampleItem(30),
                CreateExampleItem(60),
            };

            _context.AddRange(items);
            await _context.SaveChangesAsync();

            var queryParams = new TagQueryParams
            {
                Page = 1,
                PageSize = 2,
                OrderBy = "count",
                IsAscending = false
            };

            // Act
            var result = await _tagController.GetPaginatedTags(queryParams) as OkObjectResult;

            // Assert
            Assert.NotNull(result);

            //Clean up
            _context.Database.EnsureDeleted();
        }

        private static Item CreateExampleItem(int count)
        {
            return new Item
            {
                Name = $"Tag{Guid.NewGuid()}",
                Count = count
            };
        }
    }
}
