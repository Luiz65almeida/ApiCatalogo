using ApiCatalogo.Context;
using ApiCatalogo.Models;

namespace ApiCatalogo.Repositories;
public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }
    IEnumerable<Produto> IProdutoRepository.GetProdutoPorCategoria(int id)
    {
        return GetAll().Where(c => c.CategoriaId == id);
    }
}