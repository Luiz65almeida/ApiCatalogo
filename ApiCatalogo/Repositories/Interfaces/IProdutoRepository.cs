using ApiCatalogo.Models;
using ApiCatalogo.Paginador;

namespace ApiCatalogo.Repositories.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    IEnumerable<Produto> GetProdutos(ProdutoParameters produtosParameters);
    IEnumerable<Produto> GetProdutoPorCategoria(int id);
}

