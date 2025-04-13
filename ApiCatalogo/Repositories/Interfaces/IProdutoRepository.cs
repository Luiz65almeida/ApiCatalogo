using ApiCatalogo.Models;
using ApiCatalogo.Paginador;

namespace ApiCatalogo.Repositories.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    public PagedList<Produto> GetProdutos(ProdutoParameters produtoParameters);
    public IEnumerable<Produto> GetProdutoPorCategoria(int id);
}

