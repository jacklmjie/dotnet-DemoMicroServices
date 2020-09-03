using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;
using Ocelot.Provider.Polly;
using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

namespace DemoAPIGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "Gateway API", Version = "v1", Description = "# gateway api..." });
            });

            services.AddControllers();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication("orderService", options =>
            {
                options.Authority = Configuration["AuthAddress"];//��Ȩ���ĵ�ַ
                options.ApiName = "orderApi";
                options.SupportedTokens = SupportedTokens.Both;
                options.ApiSecret = "orderApi secret";
                options.RequireHttpsMetadata = false;
            })
            .AddIdentityServerAuthentication("projectService", options =>
            {
                options.Authority = Configuration["AuthAddress"];//��Ȩ���ĵ�ַ
                options.ApiName = "productApi";
                options.SupportedTokens = SupportedTokens.Both;
                options.ApiSecret = "productApi secret";
                options.RequireHttpsMetadata = false;
            });

            //���ocelot����
            services.AddOcelot()
                //���consul֧��
                .AddConsul()
                //��ӻ���
                .AddCacheManager(x =>
                {
                    x.WithDictionaryHandle();
                })
                //���Polly
                .AddPolly();
            //Ocelot����֧��Consul���������⣬����EurekaҲ���ԣ�EurekaҲ��һ�����Ƶ�ע������
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/order/swagger/v1/swagger.json", "Order API V1");
                c.SwaggerEndpoint("/project/swagger/v1/swagger.json", "Project API V1");
            });

            //����Ocelot�м��
            app.UseOcelot().Wait();
        }
    }
}
