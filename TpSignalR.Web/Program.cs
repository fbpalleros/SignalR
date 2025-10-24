using TpSignalR.Logica;
using Microsoft.EntityFrameworkCore;
using TpSignalR.Repositorio;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRegistroLogica, RegistroLogica>();
builder.Services.AddScoped<IBarcoLogica, BarcoLogica>(); // <-- registrar la lógica de barcos
builder.Services.AddDbContext<RegistroContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TpSignalRConnection")));
builder.Services.AddDbContext<BarcoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TpSignalRConnection")));

// --- Agregado para usar sesión ---
builder.Services.AddDistributedMemoryCache(); // almacenamiento temporal en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // tiempo de expiración
    options.Cookie.HttpOnly = true; // seguridad
    options.Cookie.IsEssential = true; // necesaria para el funcionamiento de la app
});
// --- Agregado para usar sesión ---

var app = builder.Build();


app.UseSession(); // --- Agregado para usar sesión ---


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
    pattern: "{controller=Registro}/{action=LogIn}/{id?}")
    .WithStaticAssets();


app.Run();
