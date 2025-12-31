using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mapping;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    
    [HttpGet]
    public ActionResult<IEnumerable<CategoriaDTO>> Get()
    {
        var categorias = _unitOfWork.CategoriaRepository.GetAll();

        var categoriasDto = categorias.ToCategoriaDTOList();
        
        return Ok(categoriasDto);
    }
    
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<CategoriaDTO> Get(int id)
    {
        var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id = {id} nao encontrada...");
            return NotFound($"Categoria com id = {id} nao encontrada...");
        }
        
        var categoriaDto =  categoria.ToCategoriaDTO();
        
        return Ok(categoriaDto);
    }
    
    [HttpPost]
    public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning($"Dados Invalidos...");
            return BadRequest("Dados Invalidos...");
        }

        var categoria = categoriaDto.ToCategoria(); 
    
        var categoriaCriada = _unitOfWork.CategoriaRepository.Create(categoria);

        var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();
        
        _unitOfWork.Commit();
        
        // Retorna 201 e aciona uma rota
        return new CreatedAtRouteResult("ObterCategoria",
            new { id = novaCategoriaDto.CategoriaId },
            novaCategoriaDto);
    }
    
    [HttpPut("{id:int}")]
    public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
    {
        if (id != categoriaDto.CategoriaId)
        {
            _logger.LogWarning($"Dados Invalidos...");
            return BadRequest("Dados Invalidos...");
        }
        
        var categoria = categoriaDto.ToCategoria(); 

        var categoriaAtualizada = _unitOfWork.CategoriaRepository.Update(categoria);
        
        _unitOfWork.Commit();
        
        var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

        return Ok(categoriaAtualizadaDto);
    }
    
    [HttpDelete("{id:int}")]
    public ActionResult<CategoriaDTO> Delete(int id)
    {
        var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id = {id} nao encontrada...");
            return NotFound($"Categoria com id = {id} nao encontrada...");
        }
    
        var categoriaExcluida = _unitOfWork.CategoriaRepository.Delete(categoria);
        
        _unitOfWork.Commit();
        
        var categoriaExcluidaDto = categoriaExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluidaDto);
    }
}