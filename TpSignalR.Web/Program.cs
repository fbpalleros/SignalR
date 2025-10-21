using TpSignalR.Logica;
using Microsoft.EntityFrameworkCore;
using TpSignalR.Repositorio;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBarcosLogica, BarcosLogica>();
builder.Services.AddDbContext<BarcoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TpSignalRConnection")));


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Home}/{id?}")
    .WithStaticAssets();


app.Run();
