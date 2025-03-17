using ApiCatalogo.Context;
using ApiCatalogo.DTOs;
using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



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

    [HttpGet]
    public ActionResult<IEnumerable<CategoriaDTO>> GetAll()
    {
        var categorias = _unitOfWork.CategoriaRepository.GetAll();

        if (categorias == null)
        {
            return NotFound("Categoria não encontrada");
        }

        var categoriasDto = new List<CategoriaDTO>();
        foreach (var categoria in categorias)
        {
            var categoriaDto = new CategoriaDTO
            {
                CategoriaId = categoria.CategoriaId,
                Nome = categoria.Nome,
                ImagemUrl = categoria.ImagemUrl
            };
            categoriasDto.Add(categoriaDto);
        }
        return Ok(categoriasDto);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<CategoriaDTO> GetId(int id)
    {
        var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id );

        if (categoria == null)
        {
            return NotFound();
        }

        var categoriaDTO = categoria.ToCategoriaDTO();

        return Ok(categoriaDTO);
    }

    [HttpPost]
    public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDTO)
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
        _unitOfWork.Commit();

        var novaCategoria = categoriaCriada.ToCategoriaDTO();

        return new CreatedAtRouteResult("ObterCategoria", new {id = categoriaCriada.CategoriaId}, categoriaCriada);
    }

    [HttpPut("{id:long}")]
    public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDTO)
    {

        if (id != categoriaDTO.CategoriaId)
        {
            return BadRequest("Categoria não encontrado");
        }

        var categoria = new Categoria()
        {
            CategoriaId = categoriaDTO.CategoriaId,
            Nome = categoriaDTO.Nome,
            ImagemUrl = categoriaDTO.ImagemUrl
        };

        var CategoriaAtualizada = _unitOfWork.CategoriaRepository.Update(categoria);
        _unitOfWork.Commit();

        var CategoriaAtualizadaDto = new CategoriaDTO()
        {
            CategoriaId = CategoriaAtualizada.CategoriaId,
            Nome = CategoriaAtualizada.Nome,
            ImagemUrl = CategoriaAtualizada.ImagemUrl
        };

        return Ok(CategoriaAtualizadaDto);

    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            return NotFound("Produto não encontrado");
        }


        var categoriaExluida = _unitOfWork.CategoriaRepository.Delete(categoria);
        _unitOfWork.Commit();

        var CategoriaExluidaDto = new CategoriaDTO()
        {
            CategoriaId = categoriaExluida.CategoriaId,
            Nome = categoriaExluida.Nome,
            ImagemUrl = categoriaExluida.ImagemUrl
        };

        return Ok(CategoriaExluidaDto + " Deletado com sucesso");
    }
}