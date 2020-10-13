using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

public interface IGenericRepo<T>
{
    //T: generic EntityType
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetByExpressionAsync(Expression<Func<T, bool>> expression); //LINQ expressie
    Task<T> GetAsyncByGuid(Guid Id);
    IEnumerable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);

    Task<T> Create(T entity);
    Task<T> Update(T entity, object key);
    Task<Object> Delete(T entity);  //returns Guid of int
    Task SaveAsync();
    Task<bool> Exists(T entity, object key);
}
