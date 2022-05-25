using BuildingBlocks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b =>
        {
            b.MigrationsAssembly("MainApp");
            b.CommandTimeout(1200);
        }
    );
    options.ConfigureWarnings(config =>
        {
            config.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
            config.Ignore(RelationalEventId.BoolWithDefaultWarning);
        }
    );
}, ServiceLifetime.Transient);

builder.Services.AddAuthentication("Cookie")
    .AddCookie("Cookie", config =>
    {
        config.Cookie.Name = "auth";
        config.ExpireTimeSpan = DateTime.Now.AddDays(1).TimeOfDay;
        config.LoginPath = "/Auth/Login";
        config.LogoutPath = "/Auth/LogOut";
        config.AccessDeniedPath = "/Auth/AccessDenied";
    });

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

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        "areas",
        "{area:exists}/{controller=Home}/{action=Index}/"
    );
    endpoints.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}");
});

app.Run();