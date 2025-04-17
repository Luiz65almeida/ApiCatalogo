using ApiCatalogo.Models;
using ApiCatalogo.Paginador;
using ApiCatalogo.Paginador.Categoria;
using X.PagedList;

namespace ApiCatalogo.Repositories.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria> 
{
    
    public Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters);
    public Task<IPagedList<Categoria>> GetCategoriasFilterNameAsync (CategoriaFilterName categoriaFilterName);
  
}

