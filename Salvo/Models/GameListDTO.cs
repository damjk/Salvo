﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace salvo.Models
{
    public class GameListDTO
    {
        public string Email { get; set; }

        public ICollection<GameDTO> Games { get; set; }
    }
}
