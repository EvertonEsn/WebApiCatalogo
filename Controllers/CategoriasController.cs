using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        try
        {
            var categorias = _context.Categorias
                .Take(10)
                .AsNoTracking()
                .ToList();

            if (categorias is null)
            {
                return NotFound("Categorias nao encontradas...");
            }
        
            return Ok(categorias);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Ocorreu um problema ao tratar sua solicitação");
        }
    }
    
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {
        try
        {
            var categoria = _context.Categorias
                .AsNoTracking()
                .FirstOrDefault(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound("Categoria nao encontrada...");
            }
        
            return Ok(categoria);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Ocorreu um problema ao tratar sua solicitação");
        }
    }

    [HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> GetCategorias()
    {
        try
        {
            var categoriasProdutos = _context.Categorias
                .AsNoTracking()
                .Include(p => p.Produtos)
                .ToList();
        
            return Ok(categoriasProdutos);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Ocorreu um problema ao tratar sua solicitação");
        }
    }
    
    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        try
        {
            if (categoria is null)
            {
                return BadRequest();
            }
        
            _context.Categorias.Add(categoria);
        
            // O SaveChanges persiste os dados do contexto na tabela do Bd
            _context.SaveChanges();
        
            // Retorna 201 e aciona uma rota
            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Ocorreu um problema ao tratar sua solicitação");
        }
    }
    
    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
        try
        {
            if (id != categoria.CategoriaId)
            {
                return BadRequest();
            }

            _context.Entry(categoria).State = EntityState.Modified;
        
            _context.SaveChanges();

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Ocorreu um problema ao tratar sua solicitação");
        }
    }
    
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        try
        {
            var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound("Categoria nao encontrada...");
            }
        
            _context.Categorias.Remove(categoria);
        
            _context.SaveChanges();

            return Ok(categoria);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "Ocorreu um problema ao tratar sua solicitação");
        }
    }
}