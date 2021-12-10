using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace salvo.Models
{
    public class Game
    {
        public long Id { get; set; }

        public DateTime CreationDate { get; set; }

        public ICollection<GamePlayer> GamePlayers { get; set; }
        public ICollection<Score> Scores { get; set; }
    }
}
