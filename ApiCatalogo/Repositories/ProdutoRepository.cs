using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Paginador;
using ApiCatalogo.Repositories.Interfaces;
using ApiCatalogo.Repositories.Utils;

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

    IEnumerable<Produto> IProdutoRepository.GetProdutos(ProdutoParameters produtosParameters)
    {
        return GetAll()
            .OrderBy(p => p.Nome)
            .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
            .Take(produtosParameters.PageSize).ToList();
    }
}