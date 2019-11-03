using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IO;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Dncblogs.Core;
using UEditorNetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Dncblogs.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Web.AotuMapper;

namespace Dncblogs.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //var builder = new ConfigurationBuilder();
            //builder.SetBasePath(Directory.GetCurrentDirectory());
            //builder.AddJsonFile("appsettings.json");
            //Configuration = builder.Build();
            this.Configuration = configuration;
            //初始化autoMapper 映射
            MapperConfig.MapperConfigIntit();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DatabaseSetting>(Configuration.GetSection("DatabaseSetting"));
            services.Configure<WebStaticConfig>(Configuration.GetSection("WebStaticConfig"));
            services.Configure<GitHubSetting>(Configuration.GetSection("GitHub"));

            //注入自己的HttpContext
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();
            services.AddTransient(typeof(BlogService));
            services.AddTransient(typeof(UserService));
            services.AddTransient(typeof(CategoryService));
            services.AddTransient(typeof(OpenSourceService));
            services.AddTransient(typeof(BlogCommentService));
            services.AddTransient(typeof(NewsService));
            services.AddTransient(typeof(NewsCommentService));
            services.AddTransient(typeof(NoteService));
            services.AddTransient(typeof(GitHubService));
            services.AddTransient(typeof(GitHubService));

            //services.AddTransientIDCommon("Dncblogs.Core");
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDistributedMemoryCache();
            //添加session
            services.AddSession(configure =>
            {
                configure.Cookie.Name = ".dncblogsCode";
                configure.IdleTimeout = TimeSpan.FromMinutes(30);
                configure.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, configureOptions =>
            {
                configureOptions.LogoutPath = "/User/LoginOut";
                configureOptions.ExpireTimeSpan = TimeSpan.FromDays(30);
                configureOptions.AccessDeniedPath = "/";
                configureOptions.LoginPath = "/User/Signin";
            });



            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 102400000;
            });
            services.AddUEditorService();

            services.AddMvc(options =>
            {
                options.Filters.Add<HttpGlobalExceptionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //文字被编码 https://github.com/aspnet/HttpAbstractions/issues/315
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            NLog.LogManager.LoadConfiguration(Path.Combine(env.ContentRootPath, "nlog.config"));
            //loggerFactory.ConfigureNLog(Path.Combine(env.ContentRootPath, "nlog.config"));
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areas", template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
