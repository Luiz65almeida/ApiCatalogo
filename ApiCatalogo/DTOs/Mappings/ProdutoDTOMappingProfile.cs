using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using AutoMapper;

namespace APICatalogo.DTOs.Mappings;

public class ProdutoDTOMappingProfile : Profile
{
    public ProdutoDTOMappingProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap();
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Produto, ProdutoDTOUpadateRequest>().ReverseMap();
        CreateMap<Produto, ProdutoDTOUpdateResponse>().ReverseMap();


    }
}
