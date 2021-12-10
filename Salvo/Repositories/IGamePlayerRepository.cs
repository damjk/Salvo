using salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace salvo.Repositories
{
    public interface IGamePlayerRepository : IRepositoryBase<GamePlayer>
    {
        public GamePlayer GetGamePlayerView(long idGamePlayer);
        void Save(GamePlayer gamePlayer);
        GamePlayer FindById(long id);
    }
}
