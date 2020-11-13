using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace APIGateway
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddOcelot();

            var providerKey = Configuration.GetSection("Tokens:AuthenticationProviderKey").Value;

            //authenticatie met Bearer
            services.AddAuthentication(svc =>
            {
                svc.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                svc.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(providerKey,
              options =>
              {
                  options.RequireHttpsMetadata = false;
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

            //Cors noodzakelijk voor front website
            services.AddCors(options =>
            {
                options.AddPolicy("MyAllowOrigins", builder =>
                {
                    builder.AllowAnyMethod()
                   .AllowAnyHeader()
                   //.AllowAnyOrigin()
                   .WithOrigins("http://localhost:29507", "http://localhost:32809", "http://localhost:10568", "http://localhost:80") //naar appSettings…
                   .AllowCredentials(); //.MUST!
                });
            });


        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //front toelaten
            app.UseCors("MyAllowOrigins");

            var currentUrl = "";
            app.Use((context, next) =>
            {
                currentUrl = context.Request.GetDisplayUrl();
                return next.Invoke();
            });


            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {


                {
                    endpoints.MapGet("/", async context =>
                    {
                        if (env.IsDevelopment())
                        {
                            await context.Response.WriteAsync(
                                    $"<div>Hello World van (Docker en)  Ocelot gateway op {currentUrl} !</div>" +
                                    "<ul>" +
                                    "<li><a href='/categories'>Lijst van categori&euml;n-API </a></li>" +
                                    "<li><a href='/recipes'>Lijst van gerechten-API </a></li>" +
                                    "<li><a href='/carts'>Shopping cart-API van onze testuser.(Beveiligd &#x1F61C; )</a></li>" +
                                       "</ul></br>" +
                                    "<ul>" +
                                    "<li><a href='https://localhost:44361' target='_blank' rel='noopener noreferrer'/>AdminCorner op andere webserver.(IdentityServices)</a></li>" +
                                    "</ul>")
                                ;
                        };
                    });

                }
            });
            app.UseWebSockets(); //vóór UseOcelot
            await app.UseOcelot();

        }
    }
}