using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mapping;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(IUnitOfWork unitOfWork, ILogger<CategoriasController> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(PagedList<Categoria> categorias)
    {
        var metadados = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
        };
     
        var categoriasDto = categorias.ToCategoriaDTOList();
        
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadados));
        
        return Ok(categoriasDto);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAll([FromQuery] CategoriasFiltroNome categoriasFiltro)
    {
        var categorias = await _unitOfWork.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasFiltro);

        return ObterCategorias(categorias);
    }
    
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> GetAsync(int id)
    {
        var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id = {id} nao encontrada...");
            return NotFound($"Categoria com id = {id} nao encontrada...");
        }
        
        var categoriaDto =  categoria.ToCategoriaDTO();
        
        return Ok(categoriaDto);
    }
    
    [HttpPost]
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning($"Dados Invalidos...");
            return BadRequest("Dados Invalidos...");
        }

        var categoria = categoriaDto.ToCategoria(); 
    
        var categoriaCriada = _unitOfWork.CategoriaRepository.Create(categoria);

        var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();
        
        await _unitOfWork.CommitAsync();
        
        // Retorna 201 e aciona uma rota
        return new CreatedAtRouteResult("ObterCategoria",
            new { id = novaCategoriaDto.CategoriaId },
            novaCategoriaDto);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning($"Dados Invalidos...");
            return BadRequest("Dados Invalidos...");
        }
        
        var categoria = categoriaDto.ToCategoria(); 

        var categoriaAtualizada = _unitOfWork.CategoriaRepository.Update(categoria);
        
        await _unitOfWork.CommitAsync();
        
        var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

        return Ok(categoriaAtualizadaDto);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id = {id} nao encontrada...");
            return NotFound($"Categoria com id = {id} nao encontrada...");
        }
    
        var categoriaExcluida = _unitOfWork.CategoriaRepository.Delete(categoria);
        
        await _unitOfWork.CommitAsync();
        
        var categoriaExcluidaDto = categoriaExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluidaDto);
    }
}