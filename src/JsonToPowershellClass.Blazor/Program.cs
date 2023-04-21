using Blazored.LocalStorage;
using JsonToPowershellClass.Blazor;
using JsonToPowershellClass.Blazor.Services;
using JsonToPowershellClass.Core.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SoloX.BlazorJsBlob;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTransient<IJsonClassGeneratorService, JsonClassGeneratorService>();
builder.Services.AddSingleton<BrowserService>();
builder.Services.AddJsBlob();

builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

await app.RunAsync();
