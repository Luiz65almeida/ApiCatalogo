using ApiCatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Context;

public class AppDbContext : DbContext{

    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options){

    }
    
    public DbSet<Categoria>? categorias { get; set; }

    public DbSet<Produto>? Produtos { get; set; }

}
