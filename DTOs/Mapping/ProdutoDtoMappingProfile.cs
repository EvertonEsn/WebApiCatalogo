using APICatalogo.Models;
using AutoMapper;

namespace APICatalogo.DTOs.Mapping;

public class ProdutoDtoMappingProfile : Profile
{
    public ProdutoDtoMappingProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap();
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Produto, ProdutoDTOUpdateRequest>().ReverseMap();
        CreateMap<Produto, ProdutoDTOUpdateResponse>().ReverseMap();
    }
}