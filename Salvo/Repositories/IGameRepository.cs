using Salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Pages.Repositories
{
    public interface IGameRepository
    {
        IEnumerable<Game> GetAllGames();

        IEnumerable<Game> GetAllGamesWithPlayers();

        

    }
}
