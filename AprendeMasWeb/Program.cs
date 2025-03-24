using AprendeMasWeb.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using AprendeMasWeb.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using AprendeMasWeb.Recursos;

var builder = WebApplication.CreateBuilder(args);
var jwtKey = builder.Configuration["jwt:SecretKey"];
var jwt = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new ArgumentNullException(jwtKey, "Token no configurado")));

// Configura Firebase
var firebaseCredentialPath = builder.Configuration["Firebase:CredentialPath"];
if (string.IsNullOrEmpty(firebaseCredentialPath))
{
    throw new InvalidOperationException("La ruta del archivo de credenciales de Firebase no est� configurada.");
}

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(firebaseCredentialPath),
});

builder.Services.AddAuthentication()
	.AddGoogle(googleOptions =>
	{
		googleOptions.ClientId = "1036601032338-grtcli283ijj9988up3hp9rbhs9qlolg.apps.googleusercontent.com";
		googleOptions.ClientSecret = "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";
		googleOptions.CallbackPath = "/signin-google";
	});
// Agregar el servicio de sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Tiempo de expiraci�n de la sesi�n
    options.Cookie.HttpOnly = true; // La cookie de sesi�n solo es accesible desde el servidor
    options.Cookie.IsEssential = true; // Marcar la cookie como esencial
});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ITiposActividadesService, TiposActividadesService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddScoped<FuncionesGenerales>();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options => {

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = jwt,
        ValidateLifetime = true,
        ValidIssuer = "Aprende_Mas",
        ValidAudience = "Aprende_Mas",
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Usar el middleware de sesiones

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=IniciarSesion}/{id?}");

app.Run();
