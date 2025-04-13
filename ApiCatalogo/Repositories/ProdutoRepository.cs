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
     public PagedList<Produto> GetProdutos (ProdutoParameters produtoParameters)
     {
        var produtos = GetAll().OrderBy(p => p.ProdutoId).AsQueryable();
        var produtosOrdenados = PagedList<Produto>.ToPagedList(produtos, produtoParameters.PageNumber, produtoParameters.PageSize);
        return produtosOrdenados;
     }
}