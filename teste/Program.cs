
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using teste.LojaContext;
using teste.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); 


builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer
    ("Server=DESKTOP-MHDLKJF\\SQLEXPRESS;Initial Catalog=Loja;User ID=Admin;Password=senha123#;language=Portuguese;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<Context>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 2;
    options.Password.RequireNonAlphanumeric = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
     .AddCookie(options =>
     {
         options.Cookie.Name = "AspNetCore.Cookies";
         options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
         options.SlidingExpiration = true;
     });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdminClaimAccess", 
        policy => policy.RequireClaim("CadastradoEm"));

    options.AddPolicy("IsAdminClaimAccess", 
        policy => policy.RequireClaim("IsAdmin", "true"));

    options.AddPolicy("IsFuncionarioClaimAccess", 
        policy => policy.RequireClaim("IsFuncionario", "true"));
});


builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
builder.Services.AddScoped<ISeedUserClaimsInitial, SeedUserClaimsInitial>();


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

await CriarPerfisUsuariosAsync(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "MinhaArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task CriarPerfisUsuariosAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        //var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
        //await service.SeedRolesAsync();
        //await service.SeedUserAsync();

        var service = scope.ServiceProvider.GetService<ISeedUserClaimsInitial>();
        await service.SeedUserClaims();
    }
}