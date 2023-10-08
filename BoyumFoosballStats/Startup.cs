using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BoyumFoosballStats.BlobStorage;
using BoyumFoosballStats.BlobStorage.Model;
using BoyumFoosballStats.Components.TeamCard.Models;
using BoyumFoosballStats.Models;
using BoyumFoosballStats.Services;
using BoyumFoosballStats.Services.Interface;
using CosmosDb.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

namespace BoyumFoosballStats
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMudServices();

            services.Scan(scan => scan
                .FromCallingAssembly()
                .AddClasses(classes => classes.AssignableTo<IViewModelBase>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            
            
            services.AddOptions();
            var tokenCredential = new DefaultAzureCredential();
            var secretClient =
                new SecretClient(new Uri(Configuration.GetValue<string>("KeyVaultUrl")!), tokenCredential);
            Configuration["CosmosConnectionStrings:ConnectionString"] =
                secretClient.GetSecret("CosmosDbConnectionString").Value.Value;
            Configuration["BlobStorageSettings:BlobUrl"] =
                secretClient.GetSecret("BlobStorageConnectionString").Value.Value;
            services.Configure<CosmosDbSettings>(Configuration.GetSection("CosmosConnectionStrings"));
            services.Configure<BlobStorageOptions>(Configuration.GetSection("BlobStorageSettings"));
            services.AddSingleton<IPlayerCrudService, PlayerCrudService>();
            services.AddSingleton<IMatchCrudService, MatchCrudService>();
            services.AddSingleton<ISessionCrudService, SessionCrudService>();
            services.AddTransient<ITeamCardViewModel, TeamCardViewModel>();
            services.AddTransient<ITeamCardViewModel, TeamCardViewModel>();
            services.AddTransient<IMatchMakingService, MatchMakingService>();
            services.AddTransient<IAzureBlobStorageHelper, AzureBlobStorageHelper>();

            services.AddSingleton<IPlayerAnalysisService, PlayerAnalysisService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}