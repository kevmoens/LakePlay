using Azure.Identity;
using Blazored.LocalStorage;
using TriviaForCheeseHeads.Data;
using TriviaForCheeseHeads.Data.Login;
using TriviaForCheeseHeads.WebUtil;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using PubSub;
using System.Collections.Concurrent;
using TriviaForCheeseHeads.Data.MRT;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<List<TriviaQuestion>>();
builder.Services.AddSingleton<Game>();
builder.Services.AddSingleton<Countdown>();
builder.Services.AddSingleton<ConcurrentDictionary<Guid, UserLogin>>();
builder.Services.AddTransient<UserLoginRepo>();
builder.Services.AddTransient<LoginVerification>();
builder.Services.AddTransient<JsConsole>();
builder.Services.AddTransient<Import>();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<TriviaForCheeseHeadsSqliteContext>(options => options.UseSqlite("TriviaForCheeseHeads.sqlite"));
    builder.Services.AddScoped<ITriviaForCheeseHeadsRepo<TriviaQuestion>, TriviaForCheeseHeadsSqliteRepo>();
} else
{
    try
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri("https://lakeplaystore.vault.azure.net/"),
            new DefaultAzureCredential());
    }  catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    builder.Services.AddSingleton<TriviaForCheeseHeadsCosmosContext>();
    builder.Services.AddSingleton<ITriviaForCheeseHeadsRepo<TriviaQuestion>, TriviaForCheeseHeadsCosmosRepo>();
}

builder.Services.AddSingleton<Hub>(Hub.Default);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
	//context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
	context.Response.Headers.Append("Content-Security-Policy", "frame-ancestors 'self' https://trivia4cheeseheads.com;");
	await next();
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
