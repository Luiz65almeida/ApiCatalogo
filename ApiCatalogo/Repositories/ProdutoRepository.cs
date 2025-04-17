using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Paginador;
using ApiCatalogo.Repositories.Interfaces;
using ApiCatalogo.Repositories.Utils;
using X.PagedList;

namespace ApiCatalogo.Repositories;
public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams)
    {
        var produtos = await GetAllAsync();

        if (produtosFiltroParams.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroParams.PrecoCriterio))
        {
            if (produtosFiltroParams.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco > produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
            }
            else if (produtosFiltroParams.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco < produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
            }
            else if (produtosFiltroParams.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco == produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
            }
        }
        
        var produtosFiltrados = await produtos.ToPagedListAsync(produtosFiltroParams.PageNumber,
            produtosFiltroParams.PageSize);
        
        return produtosFiltrados;
    }

    public async Task<IEnumerable<Produto>> GetProdutoPorCategoriaAsync(int id)
    {
        var produtos = await GetAllAsync();
       
        var produtosCategoria = produtos.Where(c => c.CategoriaId == id);

        return produtosCategoria;
    }
     public async Task<IPagedList<Produto>> GetProdutosAsync (ProdutoParameters produtoParameters)
     {
        var produtos = await GetAllAsync();
        
        var produtosOrdenados = produtos.OrderBy(p => p.ProdutoId).AsQueryable();
        
        var resultado =await produtosOrdenados.ToPagedListAsync(produtoParameters.PageNumber, produtoParameters.PageSize);
       
        return resultado;
     }
}