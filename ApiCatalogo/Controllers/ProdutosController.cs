using ApiCatalogo.Context;
using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Paginador;
using ApiCatalogo.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<ProdutoDTO>> Get([FromQuery] 
                                   ProdutoParameters produtoParameters)
    {
        var produtos = _unitOfWork.ProdutoRepository.GetProdutos(produtoParameters);

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProdutoDTO>> GetAll()
    {
        var produtos = _unitOfWork.ProdutoRepository.GetAll();
        if (produtos is null)
        {
            return NotFound();
        }
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    [HttpGet("{id}", Name = "ObterProduto")]
    public ActionResult<ProdutoDTO> Get(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Get(c => c.ProdutoId == id);
        
        if (produto is null)
        {
            return NotFound("Produto não encontrado");
        }

        var produtosDto = _mapper.Map<ProdutoDTO>(produto);

        return Ok(produtosDto);
    }

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<Produto>> GetProdutosCategorias(int id)
    {
        var produtos = _unitOfWork.ProdutoRepository.GetProdutoPorCategoria(id);

        if (produtos is null)
            return NotFound();

        return Ok(produtos);
    }

    [HttpPost]
    public ActionResult Post(ProdutoDTO produtoDto)
    {

        if (produtoDto is null)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        var novoProduto = _unitOfWork.ProdutoRepository.Create(produto);
        _unitOfWork.Commit();

        var Novoproduto = _mapper.Map<Produto>(novoProduto);

        return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
    }

    [HttpPatch("{id}/UpadatePartial")]
    public ActionResult<ProdutoDTOUpdateResponse> Patch
        (int id, JsonPatchDocument <ProdutoDTOUpadateRequest> patchProdutoDto)
    {

        if (patchProdutoDto is null || id <= 0)
        {
            return BadRequest("Produto não encontrado ou ID menor ou igual a zero");
        }

        var produto = _unitOfWork.ProdutoRepository.Get(c => c.ProdutoId == id);

        if (produto is null)
            return NotFound();

        var produtoUpadateRequest = _mapper.Map<ProdutoDTOUpadateRequest>(produto);

        patchProdutoDto.ApplyTo(produtoUpadateRequest, ModelState);

        if(!ModelState.IsValid || TryValidateModel(produtoUpadateRequest)){

            return BadRequest("Erro no validade");
        }

        _mapper.Map(produtoUpadateRequest, produto);

        _unitOfWork.ProdutoRepository.Update(produto);
        _unitOfWork.Commit();

        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));

    }

    [HttpPut("{id:int}")]
    public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {

            return BadRequest("Produto não encontrado");

        }

        var produto = _mapper.Map<Produto>(produtoDto);

        var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);
       _unitOfWork.Commit(); 

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

        return Ok(produtoAtualizado);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<ProdutoDTO> Delete(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Get(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }

        var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
        _unitOfWork.Commit();

        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);
        
        return Ok(produtoDeletado);
    }
}