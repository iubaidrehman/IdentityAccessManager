using Duende.IdentityServer;
using IdentityAccessManager.Identity;
using IdentityAccessManager.Identity.Data;
using IdentityAccessManager.Identity.Models;
using IdentityAccessManager.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
// Note: Identity Server doesn't need controllers for OAuth/OpenID Connect endpoints

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// IdentityServer
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    options.EmitStaticAudienceClaim = true;
    
    // Ensure discovery endpoints are exposed
    options.Discovery.ShowEndpoints = true;
    options.Discovery.ShowKeySet = true;
    options.Discovery.ShowIdentityScopes = true;
    options.Discovery.ShowApiScopes = true;
    options.Discovery.ShowClaims = true;
    
    // Set the issuer URI
    options.IssuerUri = "https://localhost:51181";
})
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryClients(Config.Clients)
    .AddAspNetIdentity<ApplicationUser>()
    .AddProfileService<ProfileService>();

// Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        
        // Configure Google OAuth2 settings
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
    });

// Authorization
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("default");

// Add some debugging middleware to see what requests are coming in
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

// Add debugging for Identity Server
app.Use(async (context, next) =>
{
    Console.WriteLine($"Before IdentityServer: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"After IdentityServer: {context.Response.StatusCode}");
});

app.UseIdentityServer();
app.UseAuthorization();

// Add a basic health check endpoint
app.MapGet("/health", () => "Identity Server is running!");

// Note: Identity Server endpoints are handled by UseIdentityServer(), no need for MapControllers()

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    await SeedData.SeedAsync(userManager, roleManager);
}

app.Run(); 