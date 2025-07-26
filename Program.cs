using Microsoft.EntityFrameworkCore;
using PhasePlayWeb;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models.Entities;
using PhasePlayWeb.Services;
using Syncfusion.Licensing;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);
SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJzS0d+WFlPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9nSH5Rf0RlWHZfdXRSQ2g");

var licenseInfo = SyncfusionLicenseProvider.ValidateLicense(Platform.ASPNETCore);

if (licenseInfo)
{
    Console.WriteLine("Syncfusion license registered successfully.");
}
else
{
    Console.WriteLine("Syncfusion license registration failed.");
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSQL")));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IFunctional, Functional>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Skip migrations for now
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        // Only seed data
        SeedData.Initialize(services).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while starting the application.");
    }
}


// After building the app
app.UseCors("AllowFlutter");


app.Run();
