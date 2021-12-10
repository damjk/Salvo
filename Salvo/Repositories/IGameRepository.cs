using salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace salvo.Repositories
{
    public interface IGameRepository : IRepositoryBase<Game>
    {
        IEnumerable<Game> GetAllGames();

        IEnumerable<Game> GetAllGamesWithPlayers();

        IEnumerable<Game> GetAllSalvoLocations();

        IEnumerable<Game> GetAllGamesWithPlayersAndSalvos();

        Game FindById(long id);
    }
}
