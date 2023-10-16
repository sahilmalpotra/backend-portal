using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace Investment_Portal
{
    public static class ServiceConfiguration
    {
        public static void AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();





                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }





                options.OperationFilter<SwaggerDefaultValues>();





                // integrate xml comments
                // options.IncludeXmlComments(XmlCommentsFilePath);
            });
        }





        public static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "InvestmentPortal",
                Description = "Capstone Project",
                Version = description.ApiVersion.ToString(),
                //TermsOfService = new Uri(""),
                //License = new OpenApiLicense() { Name = "", Url = new Uri("") }
            };





            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }





            return info;
        }
    }
}

//creating pull request