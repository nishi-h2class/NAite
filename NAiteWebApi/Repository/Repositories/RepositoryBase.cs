using NAiteWebApi.Repository.Contracts;
using System.Linq.Expressions;
using NAiteEntities.Models;
using Microsoft.EntityFrameworkCore;

namespace NAiteWebApi.Repository.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected NAiteContext NAiteContext { get; set; }
        public RepositoryBase(NAiteContext naiteContext)
        {
            NAiteContext = naiteContext;
        }

        public IQueryable<T> FindAll() => NAiteContext.Set<T>().AsNoTracking();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
            NAiteContext.Set<T>().Where(expression).AsNoTracking();

        public void Create(T entity) => NAiteContext.Set<T>().Add(entity);

        public void Update(T entity) => NAiteContext.Set<T>().Update(entity);

        public void Delete(T entity) => NAiteContext.Set<T>().Remove(entity);
    }
}
