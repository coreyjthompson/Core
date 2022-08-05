using System.Security.Claims;
using System.Security.Principal;

using FluentValidation.AspNetCore;

using MEI.AbbVie.Infrastructure.Data.Helpers;
using MEI.AbbVie.Infrastructure.Helpers;
using MEI.Core.Infrastructure.Data.Helpers;
using MEI.Core.Infrastructure.Helpers;
using MEI.Core.Infrastructure.Services;
using MEI.Logging;
using MEI.SPDocuments.Helpers;
using MEI.Travel.Helpers;
using MEI.Web.Areas.Travel.Validators;
using MEI.Web.Authorization;
using MEI.Web.FeatureFilters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

using NodaTime;

namespace MEI.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<InvoiceFormValidator>())
                .AddNewtonsoftJson();

            services.AddServerSideBlazor();

            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

            services.AddAuthorization(options =>
            {
                
                options.AddPolicy("IsJasonNichols", policy =>
                {
                    //policy.RequireUserName("MEIDOMAIN1\\jnichols");
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == ClaimTypes.GivenName && c.Value == "Jason")
                        && context.User.HasClaim(c => c.Type == ClaimTypes.Surname && c.Value == "Nichols"));
                });
                options.AddPolicy("AtLeast21", policy => policy.Requirements.Add(new Demo_MinimumAgeRequirement(21)));
                /*options.AddPolicy("IsJason",
                    policy => policy.RequireAssertion(context => 
                        context.User.HasClaim(c => c.Type == ClaimTypes.GivenName && c.Value == "Jason")));*/
                
            });

            services.AddSingleton<IAuthorizationHandler, Demo_MinimumAgeRequirementHandler>();

            services.AddOptions()
                .Configure<ApplicationOptions>(_configuration.GetSection("ApplicationOptions"));

            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);

            services.AddMEILogging(_configuration)
                .AddCoreInfrastructure(_configuration)
                .AddCoreData(_configuration)
                .AddTravelServices(directory)
                .AddAbbVieInfrastructure()
                .AddAbbVieData(_configuration);

            services.AddHttpContextAccessor()
            .AddScoped<IPrincipal>(
                provider =>
                {
                    var context = provider.GetService<IHttpContextAccessor>();
                    return context?.HttpContext.User;
                })
            .AddSingleton<IClock>(provider => NodaTime.SystemClock.Instance)
            .AddScoped<IUserResolverService, UserResolverService>()
            .AddTransient<ICorrelationProvider, CorrelationProvider>();

             services.AddFeatureManagement()
                .AddFeatureFilter<ClaimsFeatureFilter>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ApplicationOptions> options)
        {
            // Blazor only works from the root at present so we do url rewrites to put it in the proper location
            var rewrite = new RewriteOptions()
                .AddRewrite(@"^(.*)/_framework/blazor.boot.json", "/_framework/blazor.boot.json", skipRemainingRules: true)
                .AddRewrite(@"^(.*)/_blazor/negotiate", "/_blazor/negotiate", skipRemainingRules: true)
                .AddRewrite(@"^(.*)/_blazor?(.*)", "/_blazor?{1}", skipRemainingRules: true)
                .AddRewrite(@"^(.*)/(.*)/_framework/blazor.boot.json", "/_framework/blazor.boot.json", skipRemainingRules: true)
                .AddRewrite(@"^(.*)/(.*)/_blazor/negotiate", "/_blazor/negotiate", skipRemainingRules: true)
                .AddRewrite(@"^(.*)/(.*)/_blazor?(.*)", "/_blazor?{1}", skipRemainingRules: true);

            app.UseRewriter(rewrite);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(options.Value.SyncfusionLicenseKey);

            app.UseHttpsRedirection()
            .UseStaticFiles()
            .UseCookiePolicy()
            .UseRouting()
            .UseAuthentication() // required for claims transformation. see https://docs.microsoft.com/en-us/aspnet/core/security/authentication/windowsauth?view=aspnetcore-2.2&tabs=visual-studio#claims-transformations
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
            });

        }
    }
}
