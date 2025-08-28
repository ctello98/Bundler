using Dapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Data.Sqlite;
using Quartz;
using Quartz.Impl;
using Solnet.Rpc;
using Solnet.Wallet;
using Bundler.Services;
using System.Buffers.Text;
using Bundler.Components;

var builder = WebApplication.CreateBuilder(args);

// Configuration for Solana endpoint based on Env
var solanaEnv = builder.Configuration["Solana:Env"];
var rpcUrl = solanaEnv == "Devnet"
    ? builder.Configuration["Solana:RpcUrl_Devnet"]
    : builder.Configuration["Solana:RpcUrl_Mainnet"];

// Load default payer keypair from Base58-encoded string
var secretKey = builder.Configuration["Solana:DefaultPayerKeyJson"]!;

var defaultPayer = Account.FromSecretKey(secretKey);

// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// Database (SQLite)
// Register IDbConnection so WalletService can be constructed
builder.Services.AddSingleton<System.Data.IDbConnection>(sp =>
    new SqliteConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Solana Services (using default payer Account)
builder.Services.AddSingleton(sp => new SolanaService(rpcUrl, defaultPayer));
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<BotService>();

builder.Services.AddScoped<FundingService>();



// Quartz Scheduler
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddHostedService<QuartzHostedService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();