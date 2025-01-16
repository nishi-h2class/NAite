using NAiteWebApi.Exceptions;

namespace NAiteWebApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void AddGlobalErrorHandler(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<GlobalErrorHandlingMiddleware>();
        }
    }
}
