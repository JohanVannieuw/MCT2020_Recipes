using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CartServices.Data;
using CartServices.Repositories;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CartServices
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //1. context en repo's 
            services.AddDbContext<CartServicesContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("CartServicesContext")), ServiceLifetime.Scoped);
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            services.AddScoped<ICartRepo, CartRepo>();
          
            //2. looping
            services.AddControllers()
               .AddNewtonsoftJson(options =>
               {
                  //circulaire referenties verhinderen
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
               });



            //2. swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CartServices v1",
                    Version = "v1",
                    Description = "Een API voor het bevragen van de cartitems in de cart.",
                    Contact = new OpenApiContact
                    {
                        Name = "JohanV",
                        Email = "johan.vannieuwenhuyse@howest.be"
                    }
                });
            });

            //3. authenticatie via Ocelot
            services.AddAuthentication(svc =>
            {
                svc.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                svc.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer",
             options =>
             {
                 options.RequireHttpsMetadata = false;
                 //options.Audience = //Configuration.GetSection("Tokens:Audience").Value;
                 //options.ClaimsIssuer = Configuration.GetSection("Tokens:Issuer").Value;
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = Configuration["Tokens:Issuer"],
                     ValidAudience = Configuration["Tokens:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                 };
                 options.SaveToken = true;
             });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CartServicesContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                ModelBuilderExtensions.env = env; //property dependancy als test
            }

            //2.3 Swagger
            app.UseSwagger();                              
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CartServices v1");
            });

            app.UseRouting();

            //vóór de endpoints.
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

          
 
            
        }
    }
}
