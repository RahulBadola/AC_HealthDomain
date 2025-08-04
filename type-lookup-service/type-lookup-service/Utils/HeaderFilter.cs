using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace type_lookup_service.Utils
{
    /// <summary>
    /// Operation filter to add the requirement of the custom header
    /// </summary>
    public class HeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "CorrelationId",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "string" },
                Required = false

            });

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "TenantId",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "string" },
                Required = false
            });

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "SegmentId",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "string" },
                Required = true
            });

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "Domain",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "string" },
                Required = false
            });
        }

    }
}