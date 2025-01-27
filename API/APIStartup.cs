using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace StrikerBot.API
{
    class APIStartup
    {
        public IConfiguration Configuration { get; }

        public APIStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRateLimiter(x =>
            {
                x.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                x.AddPolicy("auth-ip-limit", context => RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection?.RemoteIpAddress.ToString(),
                    factory: fac => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromSeconds(30),
                    }
                ));
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddMvc();
            services.AddCors();
            services.AddMemoryCache();
            services.AddLogging(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning);
                builder.AddFilter("System", LogLevel.Warning);
                builder.AddConsole();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCors(x =>
            {
                x.AllowAnyOrigin();
                x.AllowAnyHeader();
            });

            app.UseRouting();
#if !DEBUG
            app.UseRateLimiter();
#endif
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
