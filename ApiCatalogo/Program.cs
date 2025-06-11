using System.Text;
using System.Text.Json.Serialization;
using ApiCatalogo.Context;
using APICatalogo.DTOs.Mappings;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using ApiCatalogo.Repositories.Interfaces;
using ApiCatalogo.Repositories.Utils;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Threading.RateLimiting;
using APICatalogo.RateLimitOptions;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

//Configuraçãodo pipeline de requisições
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Adiciona suporte a controladores e configura JSON para ignorar ciclos de referência
    services.AddControllers()
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });
    
    // Configuração do CORS
    var OrigensComAcessoPermitido = "_origensComAcessoPermitido";
    
    services.AddCors(options =>
        options.AddPolicy(name: OrigensComAcessoPermitido,
            policy =>
            {
                policy.WithOrigins("http://www.apirequest.io");
            })
    );

    // Configuração do Swagger
    services.AddEndpointsApiExplorer();
    
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Bearer JWT ",
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
                new string[] {}
            }
        });
    });

    // Configuração do Identity
    services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
    
    // Configuração do banco de dados
    string mySqlConnection = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

    // Injeção de dependências
    services.AddScoped<ICategoriaRepository, CategoriaRepository>();
    services.AddScoped<IProdutoRepository, ProdutoRepository>();
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<ITokenService, TokenService>();

    // Configuração do AutoMapper
    services.AddAutoMapper(typeof(ProdutoDTOMappingProfile));
    
    // CConfiguração da Autenticação JWT
    var secretKey = builder.Configuration["JWT:SecretKey"]
                    ?? throw new ArgumentException("Invalid secret key!!");
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    });
    
    //Configuração políticas de autorização
    services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
       
        options.AddPolicy("SuperAdminOnly", policy => 
                                policy.RequireRole("Admin").RequireClaim("id", "Henrique"));
        options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
        
        options.AddPolicy("ExclusivePolicyOnly", policy =>
            policy.RequireAssertion(context => 
                context.User.HasClaim(claim =>
                    claim.Type == "id" && claim.Value == "Henrique" 
                    || context.User.IsInRole("SuperAdmin"))));
    });
    
    //Rate Limiting
    var myOptions = new MyRateLimitOptions();
    builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);

    services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpcontext.User.Identity?.Name ??
                              httpcontext.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit =myOptions.Window,
                    QueueLimit = myOptions.QueueLimit,
                    Window = TimeSpan.FromSeconds(myOptions.Window)
                }));
    });
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseRateLimiter();
    app.UseCors();

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}
