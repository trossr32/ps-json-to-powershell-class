using JsonToPowershellClass.Blazor;
using JsonToPowershellClass.Blazor.Services;
using JsonToPowershellClass.Core.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SoloX.BlazorJsBlob;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTransient<IJsonClassGeneratorService, JsonClassGeneratorService>();
builder.Services.AddSingleton<BrowserService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<LoaderService>();
builder.Services.AddJsBlob();

await builder.Build().RunAsync();
