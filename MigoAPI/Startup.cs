using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            // This is needed just in case you are gonna use the respository pattern
            services.AddScoped<IRepository<User>, UserRepository>();

            // This is the requiered Db Configuration using EntityFramework core
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();

            services.AddVersionedApiExplorer(setupAction => 
            {
                setupAction.GroupNameFormat = "'v'VV";
            });

            // Needed packages for Api versioning => ApiVersioning and versioningExplorer
            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
                //setupAction.ApiVersionReader = new HeaderApiVersionReader("api-version");
                //setupAction.ApiVersionReader = new MediaTypeApiVersionReader();

            });

            var apiVersionDescriptionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            // This is the required OpenApi / Swagger configuration
            services.AddSwaggerGen(setupAction =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerDoc(
                    $"MigoOpenAPISpecification{description.GroupName}",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Migo API",
                        Version = description.ApiVersion.ToString(),
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
                }

                setupAction.DocInclusionPredicate((documentName, apiDescription) =>
                {
                    var actionApiVersionModel = apiDescription.ActionDescriptor
                    .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }

                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                        $"MigoOpenAPISpecificationv{v.ToString()}" == documentName);
                    }

                    return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                    $"MigoOpenAPISpecificationv{v.ToString()}" == documentName);

                });

                //setupAction.OperationFilter<GetBookOperationFilter>();
                //setupAction.OperationFilter<CreateBookOperationFilter>();

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

            // This allows to the API return the media type correctly (JSON or XML format)
            services.AddMvc(setupAction =>
            {
                // This allows to set globally these responses type attributes on each controller
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                setupAction.Filters.Add(new ProducesDefaultResponseTypeAttribute());

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            IApiVersionDescriptionProvider apiVersionDescriptionProvider)
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
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerEndpoint($"/swagger/" +
                        $"MigoOpenAPISpecification{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                    setupAction.RoutePrefix = "";
                }
                //setupAction.SwaggerEndpoint(
                //    "/swagger/MigoOpenAPISpecification/swagger.json",
                //    "Migo API");
                //setupAction.RoutePrefix = "";
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
