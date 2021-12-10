using salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace salvo.Repositories
{
    public class ScoreRepository : RepositoryBase<Score>, IScoreRepository
    {
        public ScoreRepository(SalvoContext repositoryContext) : base(repositoryContext)
        {

        }

        public void Save(Score score)
        {
            if (score.Id == 0)
                Create(score);
            else
                Update(score);
            SaveChanges();
        }
    }
}
