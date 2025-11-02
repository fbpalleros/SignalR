using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using PedidosYa.Web.Hubs;
using TpSignalR.Logica;
using TpSignalR.Repositorio;
using TpSignalR.Web.Hubs;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

// Configurar Data Protection para persistir claves y evitar errores de cookie
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("./keys"))
    .SetApplicationName("SignalRApp");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUsuarioLogica, UsuarioLogica>();
builder.Services.AddScoped<IServicioLogica, ServicioLogica>();
builder.Services.AddScoped<IMensajeLogica, MensajeLogica>();

// Configurar todos los DbContext con la misma conexión y especificar assembly para migraciones
var connectionString = builder.Configuration.GetConnectionString("TpSignalRConnection");

builder.Services.AddDbContext<InicializacionContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("TpSignalR.Web")));
builder.Services.AddDbContext<ServicioContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("TpSignalR.Web")));
builder.Services.AddDbContext<ServicioRepartoContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("TpSignalR.Web")));

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

app.MapHub<PedidoHub>("/pedidoHub");
app.MapHub<ChatHub>("/chatHub");

app.Run();
