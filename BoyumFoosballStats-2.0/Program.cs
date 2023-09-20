using BoyumFoosballStats_2._0;
using BoyumFoosballStats_2._0.Services;
using BoyumFoosballStats_2._0.Services.Interface;
using BoyumFoosballStats.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Scan(scan => scan
    .FromCallingAssembly()
    .AddClasses(classes => classes.AssignableTo<IViewModelBase>())
    .AsImplementedInterfaces()
    .WithTransientLifetime());

builder.Services.AddSingleton<IPlayerService, PlayerCrudService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();


await builder.Build().RunAsync();
