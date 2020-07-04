using System;
using System.IO;
using dao;
using dao.DbInit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    using (var dbInitializer = new DbInitializer(services))
                    {
                        dbInitializer.SeedUserData().Wait();
                        DbInitializer.SeedOtherTables(services.GetRequiredService<ApplicationDbContext>());
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddCommandLine(args)
                        .AddJsonFile("appsettings.local.json", true)
                        .Build());

                    webBuilder.ConfigureAppConfiguration((builderContext, config) =>
                    {
                        config.AddJsonFile("appsettings.local.json", true);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
