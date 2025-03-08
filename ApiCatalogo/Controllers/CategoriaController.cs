using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace ApiCatalogo.Controllers;

[Route("[Controller]")]
[ApiController]
public class CategoriaController : ControllerBase
{

    private readonly IRepository<Categoria> _categoriaRepository;
    public CategoriaController(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> GetAll()
    {
        var categoria = _categoriaRepository.GetAll();

        return Ok(categoria);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<Categoria> GetId(int id)
    {
        var categoria = _categoriaRepository.Get(c => c.CategoriaId == id );

        if (categoria == null)
        {
            return NotFound();
        }

        return Ok(categoria);
    }

    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        if (categoria is null)
        {
            return BadRequest();
        }

       var categoriaCriada =  _categoriaRepository.Create(categoria);

        return new CreatedAtRouteResult("ObterCategoria", new {id = categoriaCriada.CategoriaId}, categoriaCriada);
    }

    [HttpPut("{id:long}")]
    public ActionResult Put(int id, Categoria categoria)
    {

        if (id != categoria.CategoriaId)
        {
            return BadRequest("Categoria não encontrado");
        }

        _categoriaRepository.Update(categoria);
        return Ok(categoria);

    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _categoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            return NotFound("Produto não encontrado");
        }

        var categoriaExluida = _categoriaRepository.Delete(categoria);
        return Ok(categoriaExluida + " Deletado com sucesso");
    }
}