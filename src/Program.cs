using DotnetTestTask.Components;
using DotnetTestTask.Data;
using DotnetTestTask.Services;
using Microsoft.EntityFrameworkCore;
using Blazored.Modal;
using DotnetTestTask.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();
builder.Services.AddBlazoredModal();

var connectionString = (
    Environment.GetEnvironmentVariable("APPSETTINGS_DB_STR")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(connectionString)
);
builder.Services.AddScoped<IStoreItemService, DbStoreItemService>();
builder.Services.AddScoped<IInvoiceItemService, DbInvoiceItemService>();
builder.Services.AddScoped<IInvoiceService, DbInvoiceService>();
builder.Services.AddScoped<IInvoiceCreatorService, DbInvoiceCreatorService>();
builder.Services.AddScoped<IXlsxGenerator, XlsxGenerator>();
builder.Services.AddScoped<IWebUtils, WebUtils>();

var app = builder.Build();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider
        .GetRequiredService<AppDbContext>()
        .Database
        .Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
