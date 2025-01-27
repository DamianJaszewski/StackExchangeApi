
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using StackExchangeApi;
using StackExchangeApi.Models;
using StackExchangeApi.Services;

namespace StackExchangeTest
{
    public class TagServiceUnitTest
    {
        private readonly Mock<DataContext> _contextMock;
        private readonly HttpClient _httpClientMock;
        private readonly Mock<ILogger<TagService>> _loggerMock;
        private readonly DataContext _context;
        private readonly TagService _tagService;

        public TagServiceUnitTest()
        {
            var _contextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase("TagControllerTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

            _context = new DataContext(_contextOptions);

            _httpClientMock = new HttpClient(new MockHttpMessageHandler());
            _loggerMock = new Mock<ILogger<TagService>>();

            _tagService = new TagService(_httpClientMock, _context, _loggerMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task FetchDataAsync_ReturnsCorrectData()
        {
            // Arrange
            var httpClientMock = new MockHttpMessageHandler();
            var responseContent = "{\"items\":[{\"name\":\"Tag1\",\"count\":10},{\"name\":\"Tag2\",\"count\":20}]}";
            httpClientMock.When("https://api.stackexchange.com/*").Respond("application/json", responseContent);
            var httpClient = new HttpClient(httpClientMock);

            var tagService = new TagService(httpClient, _context, _loggerMock.Object);

            // Act
            var result = await tagService.FetchDataAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task CalculateTagsPercentageAsync_ReturnsCorrectPercentages()
        {
            // Arrange

            List<Item> items = new List<Item>()
            {
                CreateExampleItem(10),
                CreateExampleItem(30),
                CreateExampleItem(60),
            };

            _context.AddRange(items);
            _context.SaveChanges();

            // Act
            var result = await _tagService.CalculateTagsPercentageAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("10%", result.First(x => x.TagId == 1).Percentage);
            Assert.Equal("30%", result.First(x => x.TagId == 2).Percentage);
            Assert.Equal("60%", result.First(x => x.TagId == 3).Percentage);

            //Clean up
            _context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task GetPaginatedTagsAsync_ReturnsCorrectPagination()
        {
            // Arrange
            List<Item> items = new List<Item>()
            {
                CreateExampleItem(30),
                CreateExampleItem(10),
                CreateExampleItem(40),
                CreateExampleItem(20),
            };

            _context.AddRange(items);
            _context.SaveChanges(); 

            var queryParams = new TagQueryParams
            {
                Page = 1,
                PageSize = 2,
                OrderBy = "count",
                IsAscending = false
            };

            // Act
            var result = await _tagService.GetPaginatedTagsAsync(queryParams);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(10, result[0].Count);
            Assert.Equal(20, result[1].Count);

            //Clean up
            _context.Database.EnsureDeleted();
        }

        private Item CreateExampleItem(int count)
        {
            Item item = new Item()
            {
                HasSynonyms = true,
                IsModeratorOnly = true,
                IsRequired = true,
                Count = count,
                Name = "",
                Collectives = new List<Collective>(),
            };

            return item;
        }
    }
}