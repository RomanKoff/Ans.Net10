// rev 2026-11-27

using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Ans.Net10.Web.Api
{

    /// <summary>
    /// Фильтр конечных точек Minimal API, ограничивающий доступ к диагностическим 
    /// и административным обработчикам на основе правил <see cref="AnsSystemAccessService"/>.
    /// </summary>
    public sealed class AnsSystemAccessEndpointFilter
        : IEndpointFilter
    {

        /// <inheritdoc />
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var accessService1 = context.HttpContext.RequestServices
                .GetRequiredService<AnsSystemAccessService>();

            if (!await accessService1.HasAccessAsync().ConfigureAwait(false))
                return Results.NotFound();

            return await next(context).ConfigureAwait(false);
        }

    }

}
