using Microsoft.EntityFrameworkCore;
using WebAPIAutores;
using System.Text.Json.Serialization;
using WebAPIAutores.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAPIAutores.Filtros;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using WebAPIAutores.Servicios;
using WebAPIAutores.Utilidades;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebAPIAutores.Validaciones;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddControllers(opciones =>
{
    opciones.Filters.Add(typeof(FiltroDeExcepcion));
    opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
}).AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}).AddNewtonsoftJson();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"));
});
//builder.Services.AddTransient<IServicio, ServicioA>(); // crea una nueva instancia transitoria de la clase ServicioA
//builder.Services.AddScoped<IServicio, ServicioA>(); // crea instancias distintas de la clase ServicioA
//builder.Services.AddSingleton<IServicio, ServicioA>(); // tenemos siempre la misma instancia

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"])),
        ClockSkew = TimeSpan.Zero
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebAPIAutores",
        Version = "v1",
        Description = "Este es un web apu para trabajar con autores y libros",
        Contact = new OpenApiContact
        {
            Email = "dayelcode@gmail.com",
            Name = "Dayel",
            Url = new Uri("https://dayel.blog")
        },
        License = new OpenApiLicense
        {
            Name = "MIT"
        }
    });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebAPIAutores", Version = "v2" });
    c.OperationFilter<AgregarParametroHATEOAS>();
    c.OperationFilter<AgregarParametroXVersion>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
    c.IncludeXmlComments(rutaXML);
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
});

builder.Services.AddDataProtection();
builder.Services.AddTransient<HashService>();

builder.Services.AddCors(opciones => // configuring cors
{
    opciones.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader()
            .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
        //.WithExposedHeaders() exponer cabezeras que vamos a exponer desde la web api
    });
});

builder.Services.AddTransient<GeneradorEnlaces>();
builder.Services.AddTransient<HATEOASAutorFilterAttribute>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseLoguearRespuestaHTTP();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAutores v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebAPIAutores v2");
    });
}

app.UseHttpsRedirection();

//app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
