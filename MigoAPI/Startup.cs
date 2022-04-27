using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MigoAPI.Authentication;
using MigoAPI.Data;
using MigoAPI.Models;
using MigoAPI.Repository;
using MigoAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MigoAPI
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
            // This allows to the API return the media type correctly (JSON or XML format)
            services.AddMvc(setupAction =>
            {
                // This allows to set globally these responses type attributes on each controller
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                setupAction.Filters.Add(new ProducesDefaultResponseTypeAttribute());

                // This two lines are required too (Authentication feature)
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                setupAction.Filters.Add(new AuthorizeFilter());

                // This line ensures that the response media type should be json or xml (as we specified here)
                setupAction.ReturnHttpNotAcceptable = true;

                // Returns a xml and json format
                setupAction.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                var jsonOutPutFormatter = setupAction.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();

                if (jsonOutPutFormatter != null)
                {
                    // remove text/json as it isn't the approved media type
                    // for working with JSON at API level
                    if (jsonOutPutFormatter.SupportedMediaTypes.Contains("text/json"))
                    {
                        jsonOutPutFormatter.SupportedMediaTypes.Remove("text/json");
                    }
                }
            });

            // This is needed just in case you are gonna use the respository pattern
            services.AddScoped<IRepository<User>, UserRepository>();

            // This service provides the authentication feature
            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

            // This is the requiered Db Configuration using EntityFramework core
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();

            // This is the required OpenApi / Swagger configuration
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc(
                    "MigoOpenAPISpecification",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Migo API",
                        Version = "1",
                        Description = "Through this API you can acces to MIGO data",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                        {
                            Email = "isaac.ir@hotmail.com",
                            Name = "Isaac I.R.",
                            Url = new Uri("https://github.com/Texmo100")
                        },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense()
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/license/MIT")
                        }
                    });

                setupAction.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    Description = "Input your username and password to access this API"
                });

                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basicAuth"
                            }
                        }, new List<string>() 
                    }
                });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                setupAction.IncludeXmlComments(xmlCommentsFullPath);
            });

            // This ensures that the API behaves correctly
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext = 
                        actionContext as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // If there are modelState errors & all keys were correctly
                    // found/parsed we're dealing with validation errors
                    if (actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    // If one of the keys wasn't correctly found/ couldn't parsed
                    // We're dealing with null / unparsable input
                    return new BadRequestObjectResult(actionContext.ModelState);

                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // This two configurations need to be added here in order to execute correctly Swagger UI (Graphic interface)
            app.UseSwagger();

            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    "/swagger/MigoOpenAPISpecification/swagger.json",
                    "Migo API");
                setupAction.RoutePrefix = "";
            });

            // Set this for authentication feature
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
