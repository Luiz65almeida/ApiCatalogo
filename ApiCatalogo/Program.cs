using ApiCatalogo.Context;
using ApiCatalogo.Repositories;
using ApiCatalogo.Repositories.Interfaces;
using ApiCatalogo.Repositories.Utils;
using APICatalogo.DTOs.Mappings;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configuração do pipeline de requisições
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

    // Configuração do Swagger
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    // Configuração do banco de dados
    string mySqlConnection = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

    // Injeção de dependências
    services.AddScoped<ICategoriaRepository, CategoriaRepository>();
    services.AddScoped<IProdutoRepository, ProdutoRepository>();
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Configuração do AutoMapper
    services.AddAutoMapper(typeof(ProdutoDTOMappingProfile));

}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}
