using ApiCatalogo.Models;

namespace ApiCatalogo.Repositories.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    IEnumerable<Produto> GetProdutoPorCategoria(int id);
}

