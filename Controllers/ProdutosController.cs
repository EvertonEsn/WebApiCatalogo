using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    // private readonly IProdutoRepository _produtoRepository;
    // private readonly IRepository<Produto> _repository;

    public ProdutosController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        // _produtoRepository = produtoRepository;
        // _repository = repository;
    }

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<Produto>> GetProdutosCategoria(int id)
    {
        var produtos = _unitOfWork.ProdutoRepository.GetProdutosPorCategoria(id);
        
        if (produtos == null)
            return NotFound();
        
        return Ok(produtos);
    }
    
    [HttpGet]
    // O retorno ActionResult e importante para utilizar metodos como o NotFound(), Ok() e etc...
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _unitOfWork.ProdutoRepository.GetAll();

        if (produtos is null)
        {
            return NotFound("Produtos nao encontrados...");
        }
    
        return Ok(produtos);
    }
    
    // Adicionando uma rota nomeada
    [HttpGet("{id:int}", Name = "ObterProduto")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Get(p => p.CategoriaId == id);

        if (produto is null)
        {
            return NotFound("Produto nao encontrado...");
        }
    
        return Ok(produto);
    }

    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        if (produto is null)
            return BadRequest();

        var novoProduto = _unitOfWork.ProdutoRepository.Create(produto);
        
        _unitOfWork.Commit();
    
        // Retorna 201 e aciona uma rota
        return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.ProdutoId)
        {
            return BadRequest("Dados Invalidoss...");
        }

        var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);
        
        _unitOfWork.Commit();
        
        return Ok(produtoAtualizado);
    }
    
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _unitOfWork.ProdutoRepository.Get(p => p.ProdutoId == id); 
        
        if(produto is null)
            return NotFound("Produto nao encontrado...");
        
        var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
        
        _unitOfWork.Commit();
        
        return Ok(produtoDeletado);
    }
}