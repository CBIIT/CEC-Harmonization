using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CECHarmonization.Models;
using MicaData;


namespace CECHarmonization.ViewModels
{
    public class PreMapViewModel
    {
        
        public int PreMapperID { get; set; }

        public object CustomListName;
        public bool WasPosted { get; set; }

        public IList<Taxonomy> AvailableTaxonomies { get; set; }
        public IList<Taxonomy> SelectedTaxonomies { get; set; }
        public PostedTaxonomies PostedTaxonomies { get; set; }
    

        public IList<Term> AvailableTerms { get; set; }
        public IList<Term> SelectedTerms { get; set; }
        public PostedTerms PostedTerms { get; set; }

        //public IEnumerable<field_data_field_variable_categories> ValueDomains { get; set; }


    }
}