using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API_First_Lesson.Swagger;

/// <summary>
/// Swagger UI: для методов версионируемого контроллера показывает выбор версии API
/// с фиксированными значениями 1.0 и 2.0 непосредственно в параметрах метода.
/// Сама маршрутизация выполняется библиотекой Asp.Versioning.
/// </summary>
public sealed class ApiVersionDropdownOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor descriptor)
            return;

        // Фильтр нужен только контроллерам, у которых объявлены версии через [ApiVersion].
        var isVersioned = descriptor.ControllerTypeInfo
            .GetCustomAttributes(typeof(ApiVersionAttribute), inherit: true)
            .Any();

        if (!isVersioned)
            return;

        operation.Parameters ??= new List<OpenApiParameter>();

        var versionParameter = operation.Parameters.FirstOrDefault(p =>
            p.Name.Equals("api-version", StringComparison.OrdinalIgnoreCase) &&
            p.In == ParameterLocation.Query);

        if (versionParameter is null)
        {
            versionParameter = new OpenApiParameter
            {
                Name = "api-version",
                In = ParameterLocation.Query
            };
            operation.Parameters.Insert(0, versionParameter);
        }

        versionParameter.Required = true;
        versionParameter.Description = "Выберите версию API: 1.0 или 2.0";
        versionParameter.Schema = new OpenApiSchema
        {
            Type = "string",
            Enum = new List<IOpenApiAny>
            {
                new OpenApiString("1.0"),
                new OpenApiString("2.0")
            },
            Default = new OpenApiString("1.0")
        };

        // Для GET /api/Product поясняем различия контрактов обеих версий.
        if (context.ApiDescription.HttpMethod?.Equals("GET", StringComparison.OrdinalIgnoreCase) == true &&
            context.ApiDescription.RelativePath?.Equals("api/Product", StringComparison.OrdinalIgnoreCase) == true)
        {
            operation.Description = (operation.Description ?? string.Empty) +
                " 1.0: возвращает массив товаров, общее количество - в заголовке X-Total-Count. " +
                "2.0: возвращает объект { meta, data }, где каждый товар содержит вложенные " +
                "product/category, pricing и availability. " +
                "Для двух версий используется общий Swagger-документ, но разные методы контроллера.";
        }
    }
}
