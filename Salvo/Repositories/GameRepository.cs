using Microsoft.EntityFrameworkCore;
using salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace salvo.Repositories
{
    public class GameRepository : RepositoryBase<Game>, IGameRepository
    {
        public GameRepository(SalvoContext repositoryContext) : base(repositoryContext)
        {

        }

        public IEnumerable<Game> GetAllGames()
        {
            return FindAll()
                .OrderBy(ow => ow.CreationDate)
                .ToList();
        }

        public IEnumerable<Game> GetAllGamesWithPlayers()
        {
            return FindAll(source => source.Include(game => game.GamePlayers)
                    .ThenInclude(gameplayer => gameplayer.Player)
                        .ThenInclude(player => player.Scores))
                .OrderBy(game => game.CreationDate)
                .ToList();
        }

        public IEnumerable<Game> GetAllGamesWithPlayersAndSalvos()
        {
            return FindAll(source => source.Include(game => game.GamePlayers)
                    .ThenInclude(gp => gp.Player)
                        .ThenInclude(player => player.Scores)
                    .Include(game => game.GamePlayers)
                        .ThenInclude(gp => gp.Salvos)
                            .ThenInclude(salvo => salvo.Locations)
                    .Include(game => game.GamePlayers)
                        .ThenInclude(gp => gp.Ships)
                            .ThenInclude(ship => ship.Locations)
                          )
                            .OrderBy(game => game.CreationDate)
                            .ToList();
        }

        public IEnumerable<Game> GetAllSalvoLocations()
        {
            return FindAll(source => source
                          .Include(game => game.GamePlayers)
                            .ThenInclude(gp => gp.Salvos)
                                .ThenInclude(salvo => salvo.Locations)
                           )
                            .OrderByDescending(game => game.CreationDate)
                            .ToList();
        }

        public Game FindById(long id)
        {
            return FindByCondition(game => game.Id == id)
                    .Include(game => game.GamePlayers)
                        .ThenInclude(gamePlayers => gamePlayers.Player)
                    .FirstOrDefault();
        }
    }
}
