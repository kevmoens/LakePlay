using Azure.Identity;
using Blazored.LocalStorage;
using LakePlay.Data;
using LakePlay.Data.Login;
using LakePlay.WebUtil;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using PubSub;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Configuration.AddAzureKeyVault(
    new Uri("https://lakeplaystore.vault.azure.net/"),
    new DefaultAzureCredential());
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<List<TriviaQuestion>>();
builder.Services.AddSingleton<Game>();
builder.Services.AddSingleton<ConcurrentDictionary<Guid, UserLogin>>();
builder.Services.AddTransient<UserLoginRepo>();
builder.Services.AddTransient<LoginVerification>();
builder.Services.AddTransient<JsConsole>();
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<LakePlayCosmosContext>();
    builder.Services.AddScoped<ILakePlayRepo<TriviaQuestion>, LakePlayCosmosRepo>();
} else
{
    builder.Services.AddDbContext<LakePlaySqliteContext>(options => options.UseSqlite("LakePlay.sqlite"));
    builder.Services.AddScoped<ILakePlayRepo<TriviaQuestion>, LakePlaySqliteRepo>();
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

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
