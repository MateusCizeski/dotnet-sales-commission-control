using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Api.OpenApi
{
    /// <summary>Adds the API description and the "Bearer" security scheme to the OpenAPI document (Swagger UI "Authorize" button).</summary>
    public class BearerSecurityTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Info = new OpenApiInfo
            {
                Title = "Sales Commission API",
                Version = "v1",
                Description = "Sellers, invoices and automatic commission calculation. " +
                              "Log in with POST /api/auth/login and use the token as \"Bearer {token}\". " +
                              "List endpoints accept filter, sort, page, limit and fields (ApiBase query string)."
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Access token returned by POST /api/auth/login."
            };

            document.Security ??= [];
            document.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });

            return Task.CompletedTask;
        }
    }
}
