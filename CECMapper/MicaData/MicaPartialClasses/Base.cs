using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicaData
{
    public class MicaBase
    {
        [Key]
        [NotMapped]
        public int myid { get; set; }

    }
}
