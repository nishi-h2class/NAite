using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NAiteEntities.Models;
using NAiteWebApi.Helpers;
using NAiteWebApi.Repository.Contracts;
using NAiteWebApi.Repository.Repositories;
using System.Diagnostics;
using System.Text;
using NAiteWebApi.Filter;

namespace NAiteWebApi.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// CORS構成
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .WithExposedHeaders("X-Pagination"));
            });
        }

        /// <summary>
        /// IIS構成
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
                //特になし（デフォルトのまま）
            });
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration config)
        {
            Debug.WriteLine(config.GetConnectionString("NAiteContext"));
            // MySQLのバージョン
            MySqlServerVersion serverVersion = new(new Version(9, 0, 1));
            services.AddDbContext<NAiteContext>(options =>
                options.UseMySql(
                    config.GetConnectionString("NAiteContext") ?? throw new InvalidOperationException("Connection string 'NAiteContext' not found."),
                    serverVersion,
                    x => x.MigrationsAssembly("NAiteEntities")));
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<ISortHelper<User>, SortHelper<User>>();
            services.AddScoped<ISortHelper<ItemRow>, SortHelper<ItemRow>>();
            services.AddScoped<ISortHelper<NAiteEntities.Models.File>, SortHelper<NAiteEntities.Models.File>>();
            services.AddScoped<ISortHelper<ItemDataImport>, SortHelper<ItemDataImport>>();

            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped<IAuthFilter, JwTFilter>();
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(NAiteSettings.GetTokenSecretKey()));
                var issuer = config.GetValue<string>("TokenValidation:Issuer");
                var audience = config.GetValue<string>("TokenValidation:Audience");

                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = secretKey
                };
            });

            services.AddControllers(opt =>
            {
                var authFilter = new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
                opt.Filters.Add(authFilter);
            });

            services.AddControllers(opt =>
            {
                opt.Filters.Add<JwTFilter>();
            });
        }

    }
}
