using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace salvo.Models
{
    public class PlayerDTO
    {
        public long Id { get; set; }

        //[Required]
        //[StringLength(30, ErrorMessage = "The Name must be between 3 and 30 characters", MinimumLength = 3)]
        public string Name { get; set; }

        //[Required]
        //[DataType(dataType: DataType.EmailAddress)]
        //[StringLength(50, ErrorMessage = "The Email must be between 10 and 50 characters", MinimumLength = 10)]
        public string Email { get; set; }

        //[Required]
        //[DataType(dataType: DataType.Password)]
        public string Password { get; set; }
    }
}
