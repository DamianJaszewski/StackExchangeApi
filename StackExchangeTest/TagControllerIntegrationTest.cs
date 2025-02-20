using Microsoft.EntityFrameworkCore;
using StackExchangeApi.Models;
using StackExchangeApi;

namespace StackExchangeTest
{
    public class TagControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly DataContext _context;

        public TagControllerIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateDefaultClient();

            _scope = _factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<DataContext>();

            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _scope.Dispose();
        }

        [Fact]
        public async Task PopulateData_ReturnOk()
        {
            var response = await _client.GetAsync("/tag/populate");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public void TestEnvironment_Should_Create_Test_Database()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            string databaseName = context.Database.GetDbConnection().Database;
            Assert.Equal("stacktest2", databaseName);
        }

        [Fact]
        public void Database_Should_Be_Empty_Before_Each_Test()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            List<Item> items = context.Items.ToList();

            Assert.Empty(items);
        }

        [Fact]
        public async Task GetTagsPercentage_ReturnOk()
        {
            var response = await _client.GetAsync("/tag/percentage");

            response.EnsureSuccessStatusCode();
        }
    }
}
