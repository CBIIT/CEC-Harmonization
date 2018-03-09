using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CECHarmonization.Models
{
    public class Term 
    {
        public bool IsSelected { get; set; }  // Boolean value to select a checkbox on the list

        public int tid { get; set; }
        public int vid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string format { get; set; }
        public int weight { get; set; }
        public string uuid { get; set; }


    }

   

}