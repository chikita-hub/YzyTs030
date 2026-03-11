using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProPla.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// PostgreSQL + DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.AccessDeniedPath = "/auth/denied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


// --- app 作成直後に追加 ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    const string adminRole = "Admin";
    const string adminUserName = "admin";
    const string adminEmail = "admin@example.local";
    const string adminPassword = "Admin@12345"; // 初回のみ。運用時は必ず変更

    // 1) Role 作成
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
        if (!roleResult.Succeeded)
        {
            throw new Exception("Failed to create Admin role: " +
                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }
    }

    // 2) User 作成
    var adminUser = await userManager.FindByNameAsync(adminUserName);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminUserName,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (!createResult.Succeeded)
        {
            throw new Exception("Failed to create admin user: " +
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
        }
    }

    // 3) Role 付与
    if (!await userManager.IsInRoleAsync(adminUser, adminRole))
    {
        var addRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
        if (!addRoleResult.Succeeded)
        {
            throw new Exception("Failed to add Admin role to admin user: " +
                string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
        }
    }
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
