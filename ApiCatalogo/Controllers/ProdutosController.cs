using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controllers;

[Route("[Controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutosController(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> GetAll()
    {
        var produtos = _produtoRepository.GetAll();
        if (produtos is null)
        {
            return NotFound("Produtos não encontrados");
        }
        return Ok(produtos);
    }

    [HttpGet("{id}", Name = "ObterProduto")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _produtoRepository.Get(c => c.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado");
        }
        return Ok(produto);
    }

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<Produto>> GetProdutosCategorias(int id)
    {
        var produtos = _produtoRepository.GetProdutoPorCategoria(id);

        if (produtos is null)
            return NotFound();

        return Ok(produtos);
    }

    [HttpPost]
    public ActionResult Post(Produto produto)
    {

        if (produto is null)
            return BadRequest();

        var novoProduto = _produtoRepository.Create(produto);

        return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.ProdutoId)
        {

            return BadRequest("Produto não encontrado");

        }

       var produtoAtualizado = _produtoRepository.Update(produto);

        return Ok(produtoAtualizado);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _produtoRepository.Get(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }

        var produtoDeletado = _produtoRepository.Delete(produto);
        return Ok(produtoDeletado);
    }
}