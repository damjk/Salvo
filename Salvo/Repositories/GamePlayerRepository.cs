using Microsoft.EntityFrameworkCore;
using salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace salvo.Repositories
{
    public class GamePlayerRepository : RepositoryBase<GamePlayer>, IGamePlayerRepository
    {
        public GamePlayerRepository(SalvoContext repositoryContext) : base(repositoryContext)
        {

        }

        public GamePlayer GetGamePlayerView(long idGamePlayer)
        {
            return FindAll(source => source.Include(gamePlayer => gamePlayer.Ships)
                                               .ThenInclude(ship => ship.Locations)
                                           .Include(gamePlayer => gamePlayer.Salvos)
                                               .ThenInclude(salvo => salvo.Locations)
                                           .Include(gamePlayer => gamePlayer.Game)
                                               .ThenInclude(game => game.GamePlayers)
                                                   .ThenInclude(gp => gp.Player)
                                           .Include(gamePlayer => gamePlayer.Game)
                                               .ThenInclude(game => game.GamePlayers)
                                                   .ThenInclude(gp => gp.Salvos)
                                                   .ThenInclude(salvo => salvo.Locations)
                                           .Include(gamePlayer => gamePlayer.Game)
                                               .ThenInclude(game => game.GamePlayers)
                                                   .ThenInclude(gp => gp.Ships)
                                                   .ThenInclude(ship => ship.Locations))
                                           .Where(gamePlayer => gamePlayer.Id == idGamePlayer)
                                           .OrderBy(game => game.JoinDate)
                                           .FirstOrDefault();
        }

        public void Save(GamePlayer gamePlayer)
        {
            //Create(gamePlayer);
            //SaveChanges();
            if (gamePlayer.Id == 0)
                Create(gamePlayer);
            else
                Update(gamePlayer);
            SaveChanges();
        }

        public GamePlayer FindById(long id)
        {
            return FindByCondition(gamePlayer => gamePlayer.Id == id)
                    .Include(gamePlayer => gamePlayer.Game)
                        .ThenInclude(game => game.GamePlayers)
                            .ThenInclude(game => game.Salvos)
                                .ThenInclude(salvo => salvo.Locations)
                    .Include(gamePlayer => gamePlayer.Game)
                        .ThenInclude(game => game.GamePlayers)
                            .ThenInclude(game => game.Ships)
                                .ThenInclude(ship => ship.Locations)
                    .Include(gamePlayer => gamePlayer.Salvos)
                        .ThenInclude(salvo => salvo.Locations)
                    .Include(gamePlayer => gamePlayer.Ships)
                        .ThenInclude(ship => ship.Locations)
                    .Include(gamePlayer => gamePlayer.Player)
                    .FirstOrDefault();
        }
    }
}
