using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MicaData;

namespace CECHarmonization.Models
{
    public class Taxonomy
    {

        public bool IsSelected { get; set; }  // Boolean value to select a checkbox on the list

        public int vid { get; set; }
        public string name { get; set; }
        public string machine_name { get; set; }
        public string description { get; set; }
        public int hierarchy { get; set; }
        public string module { get; set; }
        public int weight { get; set; }

    }

       
}