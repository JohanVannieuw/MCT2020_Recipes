using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartServices.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CartServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                //context service ophalen
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<CartServicesContext>();
                    //context.Database.EnsureDeleted();//verwijder (-> niet doen in productie)
                   context.Database.EnsureCreated(); //maakt db aan,indien onbestaand,volgens modellen
         //context.Database.Migrate();//maakt db aan, indien onbestaand en voert migraties uit (HasData)

                    //context kan je via property dependancy doorgeven indien nodig
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
