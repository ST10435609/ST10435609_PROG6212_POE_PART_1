// start of CustomWebApplicationFactory
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RaceDay.Api.Data;

namespace RaceDay.Api.Tests
{

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                services.RemoveAll<DbContextOptions<RaceDayDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<RaceDayDbContext>>();

                services.AddDbContext<RaceDayDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            // seed the EventTypes your Events controller depends on
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RaceDayDbContext>();
            db.Database.EnsureCreated();

            return host;
        }
    }
}
// end of CustomWebApplicationFactory