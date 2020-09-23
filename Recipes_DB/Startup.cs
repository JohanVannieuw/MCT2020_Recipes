using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Recipes_DB.Models;
using Serilog;

namespace Recipes_DB
{
    public class Startup
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            //path nodig voor Serilog
            this.hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //1. controllers 
            services.AddControllers();

            //2. Context 
            var connectionString = Configuration.GetConnectionString("Recipes_DB");

            services.AddDbContext<Recipes_DB1Context>(options => options.UseSqlServer(connectionString));

            //3. Repos 
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            //services.AddScoped(typeof(IRecipeRepo<>), typeof(RecipeRepo<>));

            ////4. Mapper 
            services.AddAutoMapper(typeof(Recipes_DBProfiles));

            ////5. Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Recipes_DB",
                    Version = "v1"
                });
            });

            ////6. Serilog
            Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Warning()
      .WriteTo.RollingFile(hostingEnvironment.ContentRootPath + "Serilogs/Recipes_DBLogging-{Date}.txt")
      .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Testdoeleinden:
            Log.Logger.Warning("0000 Serilog Warning test."); //Serilog voorziet zelf  tijd
            //env.EnvironmentName = "Production";  

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseSwagger(); //enable swagger
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger"; //path naar de UI: /swagger/index.html
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Recipes_DB v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
