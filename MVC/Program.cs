using MVC.Middleware;
using Npgsql;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddScoped<NpgsqlConnection>(service =>
{
    var connectionString = service
        .GetRequiredService<IConfiguration>()
        .GetConnectionString("pgcon");

    return new NpgsqlConnection(connectionString);
});

builder.Services.AddScoped<IUserInterface, UserRepository>();
builder.Services.AddScoped<ITaskInterface, TaskRepository>();
builder.Services.AddScoped<IAdminInterface, AdminRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();