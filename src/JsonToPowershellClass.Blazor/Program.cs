using Blazored.LocalStorage;
using JsonToPowershellClass.Blazor;
using JsonToPowershellClass.Blazor.Services;
using JsonToPowershellClass.Core.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTransient<IJsonClassGeneratorService, JsonClassGeneratorService>();

builder.Services.AddSingleton<BrowserService>();

builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

await app.RunAsync();
