using BussinesMS.Aplicacion.Mapeos;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades;
using BussinesMS.Infraestructura.Persistencia;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("=== INICIANDO BUSSINESMS API ===");

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Configurando Logger...");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

Console.WriteLine("Agregando servicios MVC...");
builder.Services.AddControllers();

Console.WriteLine("Agregando Swagger...");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BussinesMS API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

Console.WriteLine("Agregando FluentValidation...");
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

Console.WriteLine("Agregando AutoMapper...");
builder.Services.AddAutoMapper(typeof(MappingProfile));

Console.WriteLine("Configurando JWT...");
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

Console.WriteLine("Configurando Authentication JWT...");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT Auth failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
            var claims = context.Principal?.Claims;
            var usuarioId = int.Parse(claims?.FirstOrDefault(c => c.Type == "sub")?.Value ?? "0");
            var username = claims?.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? "";
            var rolId = int.Parse(claims?.FirstOrDefault(c => c.Type == "rolId")?.Value ?? "0");
            
            currentUser.SetUser(usuarioId, username, rolId);
            Console.WriteLine($"User authenticated: {username} (ID: {usuarioId})");
            return Task.CompletedTask;
        }
    };
});

Console.WriteLine("Configurando DbContexts...");
builder.Services.AddDbContext<AuthDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("AuthDB"))
    .LogTo(Console.WriteLine));

builder.Services.AddDbContext<SistemaDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("SistemaDB")));

builder.Services.AddDbContext<NavidadDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("NavidadDB")));

// Repositorios
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.ISistemaRepository, BussinesMS.Infraestructura.Repositorios.Auth.SistemaRepository>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.IRolRepository, BussinesMS.Infraestructura.Repositorios.Auth.RolRepository>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.IAlmacenRepository, BussinesMS.Infraestructura.Repositorios.Auth.AlmacenRepository>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.IUsuarioRepository, BussinesMS.Infraestructura.Repositorios.Auth.UsuarioRepository>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.IMenuRepository, BussinesMS.Infraestructura.Repositorios.Auth.MenuRepository>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Sistema.ICategoriaRepository, BussinesMS.Infraestructura.Repositorios.Sistema.CategoriaRepository>();

// Servicios
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.ISistemaService, BussinesMS.Aplicacion.Servicios.Auth.SistemaService>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.IRolService, BussinesMS.Aplicacion.Servicios.Auth.RolService>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.IAlmacenService, BussinesMS.Aplicacion.Servicios.Auth.AlmacenService>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.IUsuarioService, BussinesMS.Aplicacion.Servicios.Auth.UsuarioService>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Auth.IMenuService, BussinesMS.Aplicacion.Servicios.Auth.MenuService>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Sistema.ICategoriaService, BussinesMS.Aplicacion.Servicios.Sistema.CategoriaService>();
builder.Services.AddScoped<BussinesMS.Aplicacion.Interfaces.Sistema.IMigracionService, BussinesMS.Aplicacion.Servicios.Sistema.MigracionService>();

Console.WriteLine("Construyendo aplicación...");
var app = builder.Build();

Console.WriteLine("Ejecutando seed data...");
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    
    if (!await context.Sistemas.AnyAsync())
    {
        Console.WriteLine("Insertando datos iniciales...");
        
        // Crear roles primero para tener sus IDs
        // Admin tiene todos los menús (se asignará después de crear los menús)
        var adminRol = new Rol { Nombre = "Admin", MenuIds = "", IsActive = true, CreatedAt = DateTime.UtcNow };
        var vendedorTiendaRol = new Rol { Nombre = "VendedorTienda", MenuIds = "[1,2,3,4,11,12]", IsActive = true, CreatedAt = DateTime.UtcNow };
        var vendedorRutaRol = new Rol { Nombre = "VendedorRuta", MenuIds = "[1,2]", IsActive = true, CreatedAt = DateTime.UtcNow };
        var encargadoAlmacenRol = new Rol { Nombre = "EncargadoAlmacen", MenuIds = "[1,5,6,7,9,10,19]", IsActive = true, CreatedAt = DateTime.UtcNow };
        var contadorRol = new Rol { Nombre = "Contador", MenuIds = "[1,14]", IsActive = true, CreatedAt = DateTime.UtcNow };
        
        context.Roles.AddRange(adminRol, vendedorTiendaRol, vendedorRutaRol, encargadoAlmacenRol, contadorRol);
        
        context.Sistemas.AddRange(
            new Sistema { Nombre = "Regular", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Sistema { Nombre = "Navideño", IsActive = true, CreatedAt = DateTime.UtcNow }
        );
        
        context.Almacenes.AddRange(
            new Almacen { Nombre = "Tienda Principal", Codigo = "TP001", EsTienda = true, Direccion = "Centro", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Almacen { Nombre = "Almacén 1", Codigo = "A1001", EsTienda = false, Direccion = "Zona Industrial", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Almacen { Nombre = "Almacén 2", Codigo = "A2001", EsTienda = false, Direccion = "Zona Sur", IsActive = true, CreatedAt = DateTime.UtcNow }
        );
        
        // Primer save para sistemas, roles y almacenes
        await context.SaveChangesAsync();
        
        // Ahora crear usuario con el RolId del objeto en memoria
        var passwordHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("admin123")));
        var adminMenus = "[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20]";
        context.Usuarios.Add(new Usuario
        {
            Nombre = "Admin", Apellido = "Sistema", Email = "admin@mayoristams.com",
            Username = "admin", PasswordHash = passwordHash, SistemaIdDefault = 1,
            Menus = adminMenus,
            RolId = adminRol.Id, IsActive = true, CreatedAt = DateTime.UtcNow
        });
        
        // Segundo save para usuario
        await context.SaveChangesAsync();
        Console.WriteLine("Seed de sistemas, roles, almacenes y usuarios aplicado!");
    }
    
    // Seed de menús (siempre verifica, separado del seed principal)
    if (!await context.Menus.AnyAsync())
    {
        Console.WriteLine("Insertando menús...");
        context.Menus.AddRange(
            new Menu { Nombre = "Dashboard", Url = "/dashboard", Icono = "home", Orden = 1, JerarquiaName = "Principal", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Ventas", Url = "/ventas", Icono = "shopping-cart", Orden = 1, JerarquiaName = "Ventas", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Nueva Venta", Url = "/ventas/nueva", Icono = "plus", Orden = 1, JerarquiaName = "Ventas", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Lista Ventas", Url = "/ventas/lista", Icono = "list", Orden = 2, JerarquiaName = "Ventas", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Inventario", Url = "/inventario", Icono = "box", Orden = 1, JerarquiaName = "Inventario", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Productos", Url = "/inventario/productos", Icono = "package", Orden = 1, JerarquiaName = "Inventario", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Categorías", Url = "/inventario/categorias", Icono = "folder", Orden = 2, JerarquiaName = "Inventario", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Compras", Url = "/compras", Icono = "truck", Orden = 1, JerarquiaName = "Compras", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Nueva Compra", Url = "/compras/nueva", Icono = "plus", Orden = 1, JerarquiaName = "Compras", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Proveedores", Url = "/compras/proveedores", Icono = "users", Orden = 2, JerarquiaName = "Compras", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Caja", Url = "/caja", Icono = "dollar-sign", Orden = 1, JerarquiaName = "Caja", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Apertura", Url = "/caja/apertura", Icono = "unlock", Orden = 1, JerarquiaName = "Caja", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Cierre", Url = "/caja/cierre", Icono = "lock", Orden = 2, JerarquiaName = "Caja", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Reportes", Url = "/reportes", Icono = "bar-chart", Orden = 1, JerarquiaName = "Reportes", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Usuarios", Url = "/usuarios", Icono = "users", Orden = 1, JerarquiaName = "Seguridad", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Roles", Url = "/seguridad/roles", Icono = "shield", Orden = 2, JerarquiaName = "Seguridad", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Permisos", Url = "/seguridad/permisos", Icono = "key", Orden = 3, JerarquiaName = "Seguridad", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Menús", Url = "/seguridad/menus", Icono = "menu", Orden = 4, JerarquiaName = "Seguridad", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Almacenes", Url = "/configuracion/almacenes", Icono = "archive", Orden = 1, JerarquiaName = "Configuración", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Nombre = "Sistemas", Url = "/configuracion/sistemas", Icono = "settings", Orden = 2, JerarquiaName = "Configuración", SistemaId = 1, IsActive = true, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        Console.WriteLine("Seed de menús aplicado!");
        
        // Actualizar MenuIds del rol Admin con todos los menús
        var adminRolActualizado = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Admin");
        if (adminRolActualizado != null && string.IsNullOrEmpty(adminRolActualizado.MenuIds))
        {
            var todosLosMenuIds = await context.Menus.Where(m => m.IsActive).Select(m => m.Id).ToListAsync();
            adminRolActualizado.MenuIds = System.Text.Json.JsonSerializer.Serialize(todosLosMenuIds);
            await context.SaveChangesAsync();
            Console.WriteLine("MenuIds del rol Admin actualizado con todos los menús!");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error en seed: {ex.Message}");
}

// Swagger siempre activo
Console.WriteLine("Activando Swagger...");
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("=== SERVIDOR INICIADO ===");
Console.WriteLine("Swagger: http://localhost:5001/swagger");

var port = Environment.GetEnvironmentVariable("PORT") ?? "5001";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();