using DemoWebMvc.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoWebMvc
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
            //google samesite���⣬���Թر��ܳɹ�����ʽ������Ҫ�����
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
           .AddCookie("Cookies")
           .AddOpenIdConnect("oidc", options =>
           {
               options.Authority = Configuration["ApiGatewayAddress"] + "/auth";//ͨ�����ط��ʼ�Ȩ����
               //options.Authority = Configuration["AuthAddress"];

               options.ClientId = "web client";
               options.ClientSecret = "web client secret";
               options.ResponseType = "code";

               options.RequireHttpsMetadata = false;

               options.SaveTokens = true;

               options.Scope.Add("orderApiScope");
               options.Scope.Add("productApiScope");
           });

            services.AddControllersWithViews();

            //services.AddSingleton<IServiceHelper, ServiceHelper>();
            services.AddSingleton<IServiceHelper, GatewayServiceHelper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceHelper serviceHelper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //��ô��֤��Ҫÿ������ȥConsul��ȡ��ַ��ͬʱ��Ҫ�õ����õĵ�ַ�б��أ�
            //Consul�ṩ�Ľ������������Blocking Queries �����������󣩡��������������https://www.consul.io/api-docs/features/blocking

            //��������ʱ ��ȡ�����б�
            //serviceHelper.GetServices();
        }
    }
}
