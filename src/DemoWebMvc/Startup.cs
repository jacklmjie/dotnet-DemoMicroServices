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
            //google samesite问题，调试关闭能成功，正式环境需要解决下
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
           .AddCookie("Cookies")
           .AddOpenIdConnect("oidc", options =>
           {
               options.Authority = Configuration["ApiGatewayAddress"] + "/auth";//通过网关访问鉴权中心
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

            //怎么保证不要每次请求都去Consul获取地址，同时又要拿到可用的地址列表呢？
            //Consul提供的解决方案：——Blocking Queries （阻塞的请求）。详情请见官网：https://www.consul.io/api-docs/features/blocking

            //程序启动时 获取服务列表
            //serviceHelper.GetServices();
        }
    }
}
