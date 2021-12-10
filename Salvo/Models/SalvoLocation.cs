using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace salvo.Models
{
    public class SalvoLocation
    {
        public long Id { get; set; }

        public string Location { get; set; }

        public long SalvoId { get; set; }
        public Salvo Salvo { get; set; }
    }
}
