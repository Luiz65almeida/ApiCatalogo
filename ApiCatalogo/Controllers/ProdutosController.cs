﻿using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _Context;

        public ProdutosController(AppDbContext context)
        {
            _Context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produtos = _Context.Produtos.ToList();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados");
            }
            return produtos;
        }

        [HttpGet("{id:long}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _Context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Produto não encontrado");
            }
            return produto;
        }

        [HttpPost]
        public ActionResult Post (Produto produto)
        {

            if (produto is null)
                return BadRequest();

            _Context.Produtos.Add(produto);
            _Context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", new{ id = produto.ProdutoId}, produto);
        }
    }
}
