// Usings necess�rios
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GestaoEventos.API.Data;
using GestaoEventos.API.Repositories;
 // Verifique se este � o namespace correto para IEventoRepository
using GestaoEventos.API.Services;
using GestaoEventos.API.Services.Interfaces;     // Verifique se este � o namespace correto para IEventoService
using GestaoEventos.API.Models;                 // Para o modelo Usuario
using GestaoEventos.API.Helpers;                // Para o PasswordHasher
using Microsoft.Extensions.Logging;             // Para ILogger no bloco de seed do admin

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Configura��o de CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de Depend�ncias
builder.Services.AddDbContext<EventosContext>(options =>
    options.UseInMemoryDatabase("EventosDb"));

builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IRepository<Ingresso>, IngressoRepository>();
builder.Services.AddScoped<IIngressoRepository, IngressoRepository>();
builder.Services.AddScoped<IIngressoService, IngressoService>();
builder.Services.AddScoped<IPdfService, PdfService>();


// Configura��o de Autentica��o JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "FALLBACK_SUPER_SECRET_KEY_MINIMUM_32_CHARS_LONG_FOR_DEV_ONLY_12345"))
    };
});

var app = builder.Build();

// Bloco para criar o usu�rio admin programaticamente na inicializa��o
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EventosContext>();
        // context.Database.EnsureCreated(); // �til para InMemory, mas o UseInMemoryDatabase j� faz isso.
        // Para BD real, use context.Database.Migrate();

        if (!context.Usuarios.Any(u => u.Email == "admin@eventhub.com"))
        {
            PasswordHasher.CreatePasswordHash("admin123", out byte[] hash, out byte[] salt);
            context.Usuarios.Add(new Usuario
            {
                // Id ser� gerado automaticamente pelo EF Core para InMemory se n�o especificado
                Email = "admin@eventhub.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                Papel = "Admin"
            });
            context.SaveChanges();
            Console.WriteLine(">>> Usu�rio admin padr�o criado/verificado com sucesso.");
        }
        else
        {
            Console.WriteLine(">>> Usu�rio admin padr�o j� existe.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>(); // Log para a classe Program
        logger.LogError(ex, "Ocorreu um erro ao tentar semear o usu�rio admin.");
    }
}

// Configure o pipeline de requisi��es HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

// Middlewares de Autentica��o e Autoriza��o (ordem importante)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();