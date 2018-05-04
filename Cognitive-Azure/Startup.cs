using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEscapades.AspNetCore.SecurityHeaders;
using Services.Implementation;
using Services.Interfaces;

namespace Cognitive_Azure
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();

            services.AddMvc()
                    .AddFeatureFolders();


            services.AddSingleton<ICloudStorageService, CloudStorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ICloudStorageService cloudService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseSecurityHeaders(new HeaderPolicyCollection()
                   .AddCustomHeader("Content-Security-Policy", "default-src 'none\';" +
                                                               "script-src \'self\' https://cognitive-azure.azurewebsites.net https://maxcdn.bootstrapcdn.com https://code.jquery.com/;" +
                                                               "style-src \'self\' \'unsafe-inline\' https://maxcdn.bootstrapcdn.com;" +
                                                               "font-src \'self\' https://maxcdn.bootstrapcdn.com;" +
                                                               "img-src \'self\' https://cognitiveservice.blob.core.windows.net;" +
                                                               "form-action \'self\';" +
                                                               "upgrade-insecure-requests;" +
                                                               "block-all-mixed-content;" +
                                                               "frame-ancestors 'none';" +
                                                               "report-uri https://818ad812755587db2606b9d22c5ca5b7.report-uri.com/r/d/csp/wizard;")
                   .AddCustomHeader("X-Content-Type-Options", "nosniff")
                   .AddCustomHeader("X-Frame-Options", "DENY")
                   .AddCustomHeader("X-XSS-Protection", "1; mode=block")
                   .AddCustomHeader("Strict-Transport-Security", "max-age=31536000; includeSubDomains") //"; preload")
                   .AddCustomHeader("referrer-policy", "no-referrer-when-downgrade")
                   .RemoveCustomHeader("X-Powered-By")
                   .RemoveServerHeader()
                );
               
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Images}/{action=Index}/{id?}");
            });

            cloudService.CreateContainersIfNotExist().Wait();
        }
    }
}
