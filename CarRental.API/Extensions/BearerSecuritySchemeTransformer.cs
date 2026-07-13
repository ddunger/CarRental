using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CarRental.API.Extensions
{
    internal sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider
    ) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

            if (!authenticationSchemes.Any(s => s.Name == "Bearer"))
                return;

            document.Components ??= new OpenApiComponents();
            document.AddComponent("OAuth2Password", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "Login with username and password",
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/api/identity/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>()
                    }
                }
            });

            var securityRequirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("OAuth2Password", document)] = []
            };

            foreach (var (path, pathItem) in document.Paths)
            {
                foreach (var (operationType, operation) in pathItem.Operations ?? [])
                {
                    // Find the matching endpoint descriptor
                    var descriptor = context.DescriptionGroups
                        .SelectMany(g => g.Items)
                        .FirstOrDefault(d =>
                        string.Equals(d.HttpMethod, operationType.ToString(), StringComparison.OrdinalIgnoreCase) &&
                        d.RelativePath != null &&
                        ("/" + d.RelativePath).Equals(path, StringComparison.OrdinalIgnoreCase));

                    // Skip [AllowAnonymous] endpoints
                    bool isAnonymous = descriptor?.ActionDescriptor.EndpointMetadata
                        .OfType<IAllowAnonymous>()
                        .Any() ?? false;

                    if (!isAnonymous)
                    {
                        operation.Security ??= new List<OpenApiSecurityRequirement>();
                        operation.Security.Add(securityRequirement);
                    }
                }
            }
        }
    }
}