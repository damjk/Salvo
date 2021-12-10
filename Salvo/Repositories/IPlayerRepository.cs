using Microsoft.EntityFrameworkCore.Query;
using salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace salvo.Repositories
{
    public interface IPlayerRepository : IRepositoryBase<Player>
    {
        Player FindByEmail(string email);
        void Save(Player player);
    }
}
