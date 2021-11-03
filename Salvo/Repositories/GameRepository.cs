using Salvo.Models;
using Salvo.Pages.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Repositories
{
    public class GameRepository : RepositoryBase<Game> , IGameRepository
    {
        public GameRepository(SalvoContext repositoryContext ) : base(repositoryContext)
        {

        }
        

        IEnumerable<Game> IGameRepository.GetAllGames()
        {
            throw new NotImplementedException();
        }

        

        IEnumerator<Game> IGameRepository.GetAllGamesWithplayers()
        {
            throw new NotImplementedException();
        }
    }
}
