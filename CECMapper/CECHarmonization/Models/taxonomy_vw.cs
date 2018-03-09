using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CECHarmonization.Models
{
    public partial class taxonomy_vw
    {
        public long tid { get; set; }
        public long vid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Nullable<long> parent { get; set; }

    }
}