using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
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

            //2b. Cors 
            services.AddCors(options =>
            {
                options.AddPolicy("MyAllowOrigins", builder =>
                {
                    builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    //.AllowAnyOrigin() // niet toegelaten indien credentials
                    .WithOrigins("https://localhost", "http://localhost")
                    .AllowCredentials()
                    ;
                });
            });

            //3. Repos 
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            //services.AddScoped(typeof(IRecipeRepo<>), typeof(RecipeRepo<>));

            ////4. Mapper 
            services.AddAutoMapper(typeof(Recipes_DBProfiles));

            ////5. Swagger
            ///

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //2. Include de xml file
       
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "RecipesAPI v1.0", Version = "v1.0" });
                c.SwaggerDoc("v2.0", new OpenApiInfo { Title = "RecipesAPI latest", Version = "v2.0" });
                c.IncludeXmlComments(xmlPath);
            });


            ////6. Serilog
          Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Warning()
      .WriteTo.RollingFile(hostingEnvironment.ContentRootPath + "Serilogs/Recipes_DBLogging-{Date}.txt")
      .CreateLogger();

            //7. XML formatter
            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;//gebruik headers (default:false =>Json)
                options.ReturnHttpNotAcceptable = true; //Not Acceptable media type returnt 406
                                                        //specifieke XML formatter
                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            }).AddXmlSerializerFormatters();  //voegt de standard XML formatter toe (v3)

            //8. ratelimiting 
            //Rate limiting
            //opzetten van MemoryCache om rates te bewaren
            services.AddMemoryCache();

            //één of meerdere RateLimitRules definiëren in appSettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //Singletons voor stokeren vd waarden
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            //9.Versioning
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); //major, minor >> komt in controller
                options.AssumeDefaultVersionWhenUnspecified = true;
              //  options.ApiVersionReader = new System.Web.Mvc.QueryStringOrHeaderApiVersionReader("x-api-version");
            });

            //10. Caching 
            services.AddResponseCaching();

            //11. Extra docs - Swagger  >> zie hoger 
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Testdoeleinden:
            Log.Logger.Warning("0000 Serilog Warning test."); //Serilog voorziet zelf  tijd
            //env.EnvironmentName = "Production";  

            app.UseCors("MyAllowOrigins");


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            // app.UseIpRateLimiting();

            app.UseResponseCaching();
            app.UseSwagger(); //enable swagger
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger"; //path naar de UI: /swagger/index.html
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Recipes_DB v1.0");
                c.SwaggerEndpoint("/swagger/v2.0/swagger.json", "Recipes_DB latest");
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
