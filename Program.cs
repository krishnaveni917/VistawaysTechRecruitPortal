using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;
using VistaWaysTechRecruitPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // If someone who isn't logged in (or isn't an Admin) hits an /Admin/* URL directly,
    // don't reveal the admin login page via a redirect — just return 404, as if the route
    // doesn't exist for them.
    var originalOnRedirectToLogin = options.Events.OnRedirectToLogin;
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/Admin"))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }
        return originalOnRedirectToLogin(context);
    };

    var originalOnRedirectToAccessDenied = options.Events.OnRedirectToAccessDenied;
    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/Admin"))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }
        return originalOnRedirectToAccessDenied(context);
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CandidateOnly", policy => policy.RequireRole("Candidate"));
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<CandidateIdGenerator>();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<CredentialEmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


using (var scope = app.Services.CreateScope())
{
    // Safely apply EF Core migrations: only in Development or when explicitly enabled via configuration.
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var env = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    bool applyMigrations = env.IsDevelopment() || config.GetValue<bool>("ApplyMigrations", false);

    if (applyMigrations)
    {
        try
        {
            var pending = db.Database.GetPendingMigrations();
            if (pending.Any())
            {
                logger.LogInformation("Applying {Count} pending EF Core migrations.", pending.Count());
                db.Database.Migrate();
                logger.LogInformation("EF Core migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending EF Core migrations.");
            }
        }
        catch (Exception ex)
        {
            // Log and continue. Do not rethrow to avoid crashing the app during startup.
            logger.LogError(ex, "An error occurred while applying EF Core migrations.");
        }
    }
    else
    {
        logger.LogInformation("Automatic migrations are disabled. Set ApplyMigrations=true to enable.");
    }

    await SeedAdminAccountAsync(scope.ServiceProvider);
}

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Splash}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

static async Task SeedAdminAccountAsync(IServiceProvider services)
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    const string adminRole = "Admin";
    const string candidateRole = "Candidate";
    string adminEmail = configuration["AdminUser:Email"] ?? "admin@vistawaystech.com";
    string adminPassword = configuration["AdminUser:Password"] ?? "Vista@12345";

    // Ensure Admin role exists
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Ensure Candidate role exists
    if (!await roleManager.RoleExistsAsync(candidateRole))
    {
        await roleManager.CreateAsync(new IdentityRole(candidateRole));
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, adminPassword);
    }

    if (!await userManager.IsInRoleAsync(adminUser, adminRole))
    {
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}
