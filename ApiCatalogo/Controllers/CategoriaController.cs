using ApiCatalogo.Context;
using ApiCatalogo.DTOs;
using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Models;
using ApiCatalogo.Paginador;
using ApiCatalogo.Paginador.Categoria;
using ApiCatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using X.PagedList;


namespace ApiCatalogo.Controllers;

[Route("[Controller]")]
[ApiController]
public class CategoriaController : ControllerBase
{

    private readonly IUnitOfWork _unitOfWork;

    public CategoriaController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("pagination")]
    public async Task <ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias =  await _unitOfWork.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

        return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategorias([FromQuery] CategoriaFilterName categoriaFilterName)
    {
        var categoriasFiltradas = await _unitOfWork.CategoriaRepository.GetCategoriasFilterNameAsync(categoriaFilterName);

        return ObterCategorias(categoriasFiltradas);
    }
    
    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.PageCount,
            categorias.TotalItemCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var categoriasDto = categorias.ToCategoriaDTOList();
        return Ok(categoriasDto);
    }
    
    [Authorize]
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAll()
    {
        var categorias = await _unitOfWork.CategoriaRepository.GetAllAsync();

        if (categorias is null)
            return NotFound("Categoria não encontrada");
        
        var categoriasDto = categorias.ToCategoriaDTOList();
        
        return Ok(categoriasDto);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task< ActionResult<CategoriaDTO>> GetId(int id)
    {
        var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id );

        if (categoria == null)
        {
            return NotFound();
        }

        var categoriaDto = categoria.ToCategoriaDTO();
    
        return Ok(categoriaDto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDTO)
    {
        if (categoriaDTO is null)
        {
            return BadRequest();
        }

        var categoria = new Categoria()
        {
            CategoriaId = categoriaDTO.CategoriaId,
            Nome = categoriaDTO.Nome,
            ImagemUrl = categoriaDTO.ImagemUrl
        };

        var categoriaCriada =  _unitOfWork.CategoriaRepository.Create(categoria);
        _unitOfWork.CommitAsync();

        var novaCategoria = categoriaCriada.ToCategoriaDTO();

        return new CreatedAtRouteResult("ObterCategoria", new {id = categoriaCriada.CategoriaId}, categoriaCriada);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {

        if (id != categoriaDto.CategoriaId)
        {
            return BadRequest("Categoria não encontrado");
        }

        var categoria = new Categoria()
        {
            CategoriaId = categoriaDto.CategoriaId,
            Nome = categoriaDto.Nome,
            ImagemUrl = categoriaDto.ImagemUrl
        };

        var categoriaAtualizada = _unitOfWork.CategoriaRepository.Update(categoria);
        await _unitOfWork.CommitAsync();

        var categoriaAtualizadaDto = new CategoriaDTO()
        {
            CategoriaId = categoriaAtualizada.CategoriaId,
            Nome = categoriaAtualizada.Nome,
            ImagemUrl = categoriaAtualizada.ImagemUrl
        };

        return Ok(categoriaAtualizadaDto);

    }

    [HttpDelete("{id:int}")]
    [Authorize (Policy = "AdminOnly")]
    public async Task<ActionResult> Delete(int id)
    {
        var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            return NotFound("Produto não encontrado");
        }
        
        var categoriaExluida = _unitOfWork.CategoriaRepository.Delete(categoria);
        await _unitOfWork.CommitAsync();

        var CategoriaExluidaDto = new CategoriaDTO()
        {
            CategoriaId = categoriaExluida.CategoriaId,
            Nome = categoriaExluida.Nome,
            ImagemUrl = categoriaExluida.ImagemUrl
        };

        return Ok(CategoriaExluidaDto + " Deletado com sucesso");
    }
}