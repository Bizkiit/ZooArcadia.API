using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Security.Claims;
using System.Text;
using ZooArcadia.API.Models.DbModels;
using ZooArcadia.API.Services;
using ZooArcadia.API.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200", "https://www.zooarcadia.ovh", "https://zooarcadia.ovh", "http://localhost")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddScoped<EmailService>();
builder.Services.AddSingleton<JwtKeyService>();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddDbContext<ZooArcadiaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ZooArcadia.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \\\"Authorization: Bearer {token}\\",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,
        },
        new List<string>()
    }});
});

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection(nameof(MongoDBSettings)));

builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<AnimalService>();
builder.Services.AddScoped<ImageService>();

var JWTSetting = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var sp = builder.Services.BuildServiceProvider();
        var logger = sp.GetRequiredService<ILogger<Program>>();
        var jwtKeyService = sp.GetRequiredService<JwtKeyService>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtKeyService.Key,
            ValidateIssuer = true,
            ValidIssuer = JWTSetting["Issuer"],
            ValidateAudience = true,
            ValidAudience = JWTSetting["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                logger.LogError($"Échec de l'authentification : {context.Exception.Message}");
                if (context.Exception is SecurityTokenExpiredException expiredException)
                {
                    logger.LogError($"Token expiré le : {expiredException.Expires}");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                logger.LogInformation("Token validé avec succès");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                logger.LogWarning("Token validation failed: " + context.Error);
                logger.LogWarning("Token validation error description: " + context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Administrateur"));
    options.AddPolicy("MultipleRolesPolicy", policy => policy.RequireRole("Administrateur", "Véterinaire", "Employé"));
});

builder.Services.AddScoped<JwtTokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZooArcadia.API v1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZooArcadia.API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ZooArcadiaDbContext>();
        var passwordMigration = new PasswordMigration(context);
        passwordMigration.MigratePasswords();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the passwords.");
    }
}

app.Run();
