using Microsoft.EntityFrameworkCore;
using Salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Salvo.Pages.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {       
        protected SalvoContext RepositoryContext { get; set; }
        public RepositoryBase(SalvoContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }
        public void Create(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> FindAll()
        {
            return this.RepositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<Task, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void Update(T Entity)
        {
            throw new NotImplementedException();
        }
    }
}
