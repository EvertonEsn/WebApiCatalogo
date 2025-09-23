using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repository.Interfaces;

namespace APICatalogo.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }
}