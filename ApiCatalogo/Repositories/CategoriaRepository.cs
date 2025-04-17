using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories.Interfaces;
using ApiCatalogo.Repositories.Utils;
using ApiCatalogo.Paginador;
using ApiCatalogo.Paginador.Categoria;
using X.PagedList;

namespace ApiCatalogo.Repositories;

public class CategoriaRepository :Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base (context)
    {
    }

    public async Task<IPagedList<Categoria>> GetCategoriasAsync (CategoriasParameters categoriasParameters)
    {
        var categorias = await GetAllAsync();

        var categoriasOrdenados = categorias.OrderBy(p => p.CategoriaId).AsQueryable();
        
        var resultado = await categoriasOrdenados.ToPagedListAsync(categoriasParameters.PageNumber, categoriasParameters.PageSize);
        
        return resultado;
    }

    public async Task<IPagedList<Categoria>> GetCategoriasFilterNameAsync (CategoriaFilterName categoriasParams)
    {
        var categorias = await GetAllAsync();

        if (!string.IsNullOrEmpty(categoriasParams.Name))   
        {
            categorias = categorias.Where(c => c.Nome.Contains(categoriasParams.Name));
        }

        var categoriasFiltradas = await categorias.ToPagedListAsync(
            categoriasParams.PageNumber,
            categoriasParams.PageSize);

        return categoriasFiltradas;
    }
}

