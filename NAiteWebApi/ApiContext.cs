using AspNetCoreCurrentRequestContext;
using NAiteEntities.Models;

namespace NAiteWebApi
{
    public class ApiContext
    {
        private static IHttpContextAccessor _httpContextAccessor = null!;

        public static HttpContext Current => _httpContextAccessor.HttpContext = null!;

        public static string AppBaseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";

        internal static void Configure(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }

        public static User? CurrentUser
        {
            get
            {
                var data = AspNetCoreHttpContext.Current.Items["API_" + nameof(CurrentUser)] as User;
                return data;
            }
            set
            {
                AspNetCoreHttpContext.Current.Items["API_" + nameof(CurrentUser)] = value;
            }
        }
    }

    public static class HttpContextExtensions
    {
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app)
        {
            ApiContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            return app;
        }
    }
}
