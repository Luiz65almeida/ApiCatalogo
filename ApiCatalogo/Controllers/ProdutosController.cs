using ApiCatalogo.Context;
using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Paginador;
using ApiCatalogo.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ApiCatalogo.Controllers;

[Route("[Controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet("filter/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosFiltroPreco produtosFiltroPrecoParams)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFiltroPrecoParams);
        
        var metadata = new
        {
            produtos.Count,
            produtos.PageSize,
            produtos.PageCount,
            produtos.TotalItemCount,
            produtos.HasNextPage,
            produtos.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }
    

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] 
                                   ProdutoParameters produtoParameters)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosAsync(produtoParameters);

        var metadata = new
        {
            produtos.Count,
            produtos.PageSize,
            produtos.PageCount,
            produtos.TotalItemCount,
            produtos.HasNextPage,
            produtos.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }
    
    [HttpGet]
    [Authorize (Policy = "UserOnly")]
    public async Task< ActionResult<IEnumerable<ProdutoDTO>>> GetAll()
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetAllAsync();
        
        if (produtos is null)
        {
            return NotFound();
        }
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    [HttpGet("{id}", Name = "ObterProduto")]
    public async Task< ActionResult<ProdutoDTO>> Get(int id)
    {
        var produto = await _unitOfWork.ProdutoRepository.GetAsync(c => c.ProdutoId == id);
        
        if (produto is null)
        {
            return NotFound("Produto não encontrado");
        }

        var produtosDto = _mapper.Map<ProdutoDTO>(produto);

        return Ok(produtosDto);
    }

    [HttpGet("produtos/{id}")]
    public async Task< ActionResult<IEnumerable<Produto>>> GetProdutosCategorias(int id)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutoPorCategoriaAsync(id);

        if (produtos is null)
            return NotFound();

        return Ok(produtos);
    }

    [HttpPost]
    public async Task< ActionResult> Post(ProdutoDTO produtoDto)
    {

        if (produtoDto is null)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        var novoProduto = _unitOfWork.ProdutoRepository.Create(produto);
        await _unitOfWork.CommitAsync();

        var Novoproduto = _mapper.Map<Produto>(novoProduto);

        return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
    }

    [HttpPatch("{id}/UpadatePartial")]
    public async Task< ActionResult<ProdutoDTOUpdateResponse>> Patch
        (int id, JsonPatchDocument <ProdutoDTOUpadateRequest> patchProdutoDto)
    {

        if (patchProdutoDto is null || id <= 0)
        {
            return BadRequest("Produto não encontrado ou ID menor ou igual a zero");
        }

        var produto = await _unitOfWork.ProdutoRepository.GetAsync(c => c.ProdutoId == id);

        if (produto is null)
            return NotFound();

        var produtoUpadateRequest = _mapper.Map<ProdutoDTOUpadateRequest>(produto);

        patchProdutoDto.ApplyTo(produtoUpadateRequest, ModelState);

        if(!ModelState.IsValid || TryValidateModel(produtoUpadateRequest)){

            return BadRequest("Erro no validade");
        }

        _mapper.Map(produtoUpadateRequest, produto);

        _unitOfWork.ProdutoRepository.Update(produto);
        await _unitOfWork.CommitAsync();

        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));

    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {

            return BadRequest("Produto não encontrado");

        }

        var produto = _mapper.Map<Produto>(produtoDto);

        var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);
       await _unitOfWork.CommitAsync(); 

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

        return Ok(produtoAtualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task< ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }

        var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
        await _unitOfWork.CommitAsync();

        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);
        
        return Ok(produtoDeletado);
    }
}