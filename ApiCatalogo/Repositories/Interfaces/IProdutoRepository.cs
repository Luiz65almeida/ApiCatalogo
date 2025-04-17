using ApiCatalogo.Models;
using ApiCatalogo.Paginador;
using X.PagedList;

namespace ApiCatalogo.Repositories.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    public Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroPreco);
    public Task<IEnumerable<Produto>> GetProdutoPorCategoriaAsync(int id);
    public Task<IPagedList<Produto>> GetProdutosAsync(ProdutoParameters produtoParameters);
    
}

