using System.Linq.Expressions;

namespace APICatalogo.Repository.Interfaces;

public interface IRepository<T>
{
    // Cuidado para não Ferir a Isp -> Interface Segregation Principle
    IEnumerable<T> GetAll();
    T? Get(Expression<Func<T, bool>> predicate);
    T Create(T entity);
    T Update(T entity);
    T Delete(T entity);
}