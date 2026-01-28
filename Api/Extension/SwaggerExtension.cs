using Microsoft.OpenApi;

namespace Api.Extension
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddApiDoc(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Sistema de Controle de Comissões",
                    Version = "v1",
                    Description = "API RESTful para cadastro de vendedores, controle de Invoices e cálculo automático de comissões.\n\n" +
                                  "Este sistema permite:\n" +
                                  "- Cadastro de vendedores com validações de CPF, Email e percentual de comissão\n" +
                                  "- CRUD de Invoices com cálculo automático de comissão\n" +
                                  "- Dashboard gerencial com indicadores de vendas e comissões\n" +
                                  "- Filtros por período, vendedor e status da Invoice\n" +
                                  "\nDesenvolvido para teste técnico - Clean Architecture em camadas Domain, Application, Infrastructure e Web."
                });

                c.DocumentFilter<TagDocumentFilter>();
            });

            return services;
        }

        public static WebApplication UseApiDoc(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Sistema de Comissões v1");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });

            return app;
        }
    }

    public class TagDocumentFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, Swashbuckle.AspNetCore.SwaggerGen.DocumentFilterContext context)
        {
            swaggerDoc.Tags = new HashSet<OpenApiTag>
            {
                new OpenApiTag { Name = "Vendedores", Description = "Cadastro e gerenciamento de vendedores" },
                new OpenApiTag { Name = "Invoices", Description = "CRUD de Invoices e cálculo de comissão" },
                new OpenApiTag { Name = "Dashboard", Description = "Indicadores de vendas e comissões" }
            };
        }
    }
}
