
using Microsoft.EntityFrameworkCore;
using StackExchangeApi.Services;

namespace StackExchangeApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().
                AddJsonOptions(options => {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient("StackExchangeClient", client =>
            {
                client.BaseAddress = new Uri("https://api.stackexchange.com/2.3/");
                client.DefaultRequestHeaders.Add("User-Agent", "YourAppName");
            });

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("BasicConnection"));
            });

            builder.Services.AddScoped<ITagService, TagService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                try
                {
                    Console.WriteLine("Creating database...");
                    dbContext.Database.Migrate(); 
                    Console.WriteLine("Database ready!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during implementation: {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
