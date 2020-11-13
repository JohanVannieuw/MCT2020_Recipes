using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Recipes_DB.Models;

public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : class
{
    public Recipes_DB1Context _context { get; set; }

    //ctor dependancy van de applicatie context:
    public GenericRepo(Recipes_DB1Context context)
    {
        this._context = context;
    }

    //interface implementatie:
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await this._context.Set<TEntity>().AsNoTracking().ToListAsync();
    }

    public IEnumerable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        //IQueryable nodig voor include
        IQueryable<TEntity> allEntities = _context.Set<TEntity>();
        foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
        {

            allEntities = allEntities.Include<TEntity, object>(includeProperty);
        }

        return allEntities;
    }


    public async Task<IEnumerable<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression)
    {
        //voorbeeld bij search: expression = "p => p.ProductID == id"  -- je kent immers de ID property niet.
        //returnt wel een collectie! Gebruik desnoods First().
        return await this._context.Set<TEntity>().Where(expression).AsNoTracking().ToListAsync();
    }

    public async Task<TEntity> GetAsyncByGuid(Guid Id)
    {
        //enkel bruikbaar bij een guid Key
        var entity = await _context.Set<TEntity>().FindAsync(Id);
        _context.Entry(entity).State = EntityState.Detached; //AsNoTracking() bestaat niet bij FindAsync
        return entity;
    }

    public async Task<TEntity> Create(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException($"{nameof(Create)} entity must not be null");
            //return null;
        }
        try
        {
            EntityEntry<TEntity> result = await _context.Set<TEntity>().AddAsync(entity);
            await SaveAsync();
            return entity;

        }
        catch (Exception)
        {
            throw new Exception($"{nameof(entity)} could not be saved");
        }
    }

    public async Task<TEntity> Update(TEntity entity, object key)
    {
        if (entity == null)
            return null;
        try
        {
            var exist = await _context.Set<TEntity>().FindAsync(key);
            if (exist != null)
            {

                _context.Entry(entity).State = EntityState.Detached;
                //indien ook update async
                // await Task.Factory.StartNew(() =>
                // {
                _context.Entry(exist).CurrentValues.SetValues(entity);
                // _context.Set<TEntity>().Update(entity);
                //});
                await SaveAsync();
                return entity;
            }
            return null;
        }
        catch (Exception exc)
        {
            throw new Exception($"{nameof(entity)} could not be updated. {exc.InnerException.Message}");
        }
    }

    public async Task<Object> Delete(TEntity entity)
    {
        await Task.Factory.StartNew(() =>
        {
            _context.Set<TEntity>().Remove(entity);

        });
        await SaveAsync();
        return await Task.FromResult<Object>(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }


    public async Task<bool> Exists(TEntity entity, object key)
    {
        TEntity exist = await _context.Set<TEntity>().FindAsync(key);
        if (exist != null) return true;
        return false;
    }



}

