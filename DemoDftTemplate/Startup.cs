using DemoDftTemplate.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DemoDftTemplate
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                // .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        /// <summary>
        /// The commented out code below for google logging is to allow the solution to build locally. 
        /// Inside GCP this is not needed but to authenticate locally you need a service account file.
        /// 
        /// SET THE ENVIRONMENT VARIABLE TO MAKE THE APPLICATION WORKING ON YOUR LOCAL MACHINE
        /// TO DOWNLOAD THE JSON CREDENTIALS FILE, CHECK THE FOLLOWING LINK: 
        /// https://cloud.google.com/docs/authentication/production#obtaining_and_providing_service_account_credentials_manually 
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // if (Configuration.GetSection("DevEnvironment").Value.Equals("local"))
            // {
            //    System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"Services\IAPClient\[nameOfFile.json]");
            // }

            // services.AddGoogleExceptionLogging(options =>
            // {
            //     options.ProjectId = Configuration.GetSection("ProjectId").Value;
            //     options.ServiceName = Configuration.GetSection("ServiceName").Value;
            // });

            // services.AddGoogleTrace(options =>
            // {
            //     options.ProjectId = Configuration.GetSection("ProjectId").Value;
            // });

            //services.AddHttpClient("tracesOutgoing").AddOutgoingGoogleTraceHandler();

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(20);
                options.SlidingExpiration = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //If a data layer is required, uncomment the following line or delete as appropriate.
            //AddTransient<DefaultContext>(_ => new DefaultContext(Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddScoped<ApplicationUser, ApplicationUser>();
            services.Configure<ConfigSettings>(Configuration);

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = "MySchemeHandler";

                // you can also skip this to make the challenge scheme handle the forbid as well
                options.DefaultForbidScheme = "MySchemeHandler";

                // of course you also need to register that scheme, e.g. using
                options.AddScheme<Security.MySchemeHandler>("MySchemeHandler", "MySchemeHandler");
            });

            // Authorisation to be able to access pages - look in Security folder to set and Controllers to use
            services.AddScoped<IAuthorizationHandler, Security.HasDevRightsHandler>();
            services.AddScoped<IAuthorizationHandler, Security.HasAdminRightsHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasDevRights", policy =>
                    policy.Requirements.Add(new Security.HasDevRightsRequirement()));

                options.AddPolicy("HasAdminRights", policy =>
                    policy.Requirements.Add(new Security.HasAdminRightsRequirement()));
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            //app.UseGoogleTrace();
            //loggerFactory.AddGoogle(app.ApplicationServices, Configuration.GetSection("ProjectId").ToString());
            
            app.UseForwardedHeaders();
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.SameOrigin());

            app.UseCsp(opts => opts
            .BlockAllMixedContent()
            .StyleSources(s => s.Self())
            .StyleSources(s => s.UnsafeInline().CustomSources("https://rsms.me/inter/inter.css"))
            .FontSources(s => s.Self().CustomSources("https://rsms.me/inter/font-files/"))
            .FormActions(s => s.Self())
            .FrameAncestors(s => s.Self())
            .ImageSources(s => s.Self()
            .CustomSources("https://www.googletagmanager.com", "https://www.google-analytics.com"))
            .ScriptSources(s => s.Self())
            .ScriptSources(s => s.UnsafeInline().CustomSources("https://www.googletagmanager.com", "https://www.google-analytics.com"))
            );

            app.Use(async (context, next) =>
            {
                if (context.Request.IsHttps || context.Request.Headers["X-Forwarded-Proto"] == Uri.UriSchemeHttps)
                {
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
                    await next();
                }
                else
                {
                    string queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
                    var https = "https://" + context.Request.Host + context.Request.Path + queryString;
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
                    context.Response.Redirect(https);
                }

            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.Strict
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
