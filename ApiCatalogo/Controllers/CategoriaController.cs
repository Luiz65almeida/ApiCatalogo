using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace ApiCatalogo.Controllers;

[Route("[Controller]")]
[ApiController]
public class CategoriaController : ControllerBase
{

    private readonly AppDbContext _context;
    public CategoriaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> GetAll()
    {

        var categoria = _context.Categorias.ToList();
        if (categoria is null)
        {
            return NotFound();
        }
        return categoria;
    }

    [HttpGet("{id:long}", Name = "ObterCategoria")]
    public ActionResult<Categoria> GetId(long id)
    {
        var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

        if (categoria == null)
        {
            return NotFound();
        }

        return categoria;
    }

    [HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        return _context.Categorias.Include(p => p.Produtos).ToList();
    }

    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        if (categoria is null)
        {
            return BadRequest();
        }

        _context.Categorias.Add(categoria);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterCategoria", new {id = categoria.CategoriaId}, categoria);
    }

    [HttpPut("{id:long}")]
    public ActionResult Put(int id, Categoria categoria)
    {

        if (id != categoria.CategoriaId)
        {
            return BadRequest("Categoria não encontrado");
        }

        _context.Entry(categoria).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(categoria);

    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

        if (categoria is null)
        {
            return NotFound("Produto não encontrado");
        }

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();

        return Ok(categoria.Nome + " Deletado com sucesso");
    }
}