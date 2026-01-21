using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    // private readonly IProdutoRepository _produtoRepository;
    // private readonly IRepository<Produto> _repository;

    public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        // _produtoRepository = produtoRepository;
        // _repository = repository;
    }

    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(PagedList<Produto> produtos)
    {
        var metadados = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasNext,
            produtos.HasPrevious
        };
        
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadados));
        
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        
        return Ok(produtosDto);
    }

    [HttpGet("filter/preco")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco produtosFilterParams)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFilterParams);

        return ObterProdutos(produtos);
    }

    [HttpGet("produtos/{id}")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosCategoria(int id, [FromQuery] ProdutosParameters  produtosParams)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosPorCategoriaAsync(id, produtosParams);
        
        if (produtos == null)
            return NotFound();
        
        return ObterProdutos(produtos);
    }

    [HttpGet]
    // O retorno ActionResult e importante para utilizar metodos como o NotFound(), Ok() e etc...
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAll([FromQuery] ProdutosParameters produtosParams)
    {
        var produtos = await _unitOfWork.ProdutoRepository.GetProdutosAsync(produtosParams);

        if (produtos is null)
        {
            return NotFound("Produtos nao encontrados...");
        }

        return ObterProdutos(produtos);
    }
    
    // Adicionando uma rota nomeada
    [Authorize(Policy = "UserOnly")]
    [HttpGet("{id:int}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
        var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.CategoriaId == id);

        if (produto is null)
        {
            return NotFound("Produto nao encontrado...");
        }
        
        var produtoDto = _mapper.Map<ProdutoDTO>(produto);
        
        return Ok(produtoDto);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDto)
    {
        if (produtoDto is null)
            return BadRequest();
        
        var produto = _mapper.Map<Produto>(produtoDto);

        var novoProduto = _unitOfWork.ProdutoRepository.Create(produto);
        
        await _unitOfWork.CommitAsync();
        
        var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);
    
        // Retorna 201 e aciona uma rota
        return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDto.ProdutoId }, novoProdutoDto);
    }

    [HttpPatch("{id:int}/UpdatePartial")]
    public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
    {
        if(patchProdutoDTO is null || id <= 0)
            return BadRequest();
        
        var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
        
        if (produto is null)
            return NotFound();
        
        var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);
        
        patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);
        
        if(!ModelState.IsValid || !TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);
        
        _mapper.Map(produtoUpdateRequest, produto);
        
        _unitOfWork.ProdutoRepository.Update(produto);
        await _unitOfWork.CommitAsync();
        
        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
            return BadRequest("Dados Invalidos...");
        
        var produto = _mapper.Map<Produto>(produtoDto);

        var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);
        
        await _unitOfWork.CommitAsync();
        
        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);
        
        return Ok(produtoAtualizadoDto);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id); 
        
        if(produto is null)
            return NotFound("Produto nao encontrado...");
        
        var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
        
        await _unitOfWork.CommitAsync();
        
        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);
        
        return Ok(produtoDeletadoDto);
    }
}