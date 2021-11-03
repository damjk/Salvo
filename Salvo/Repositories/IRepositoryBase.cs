using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Salvo.Pages.Repositories
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<Task,bool>> expression);
        void Create(T entiry);
        void Update(T Entity);
        void Delete(T entity);
    }
}
