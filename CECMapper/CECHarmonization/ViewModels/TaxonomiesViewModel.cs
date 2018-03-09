using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CECHarmonization.Models;

namespace CECHarmonization.ViewModels
{
    public class TaxonomiesViewModel
    {

        public object CustomListName;
        public bool WasPosted { get; set; }
        public IList<Taxonomy> AvailableTaxonomies { get; set; }
        public IList<Taxonomy> SelectedTaxonomies { get; set; }
        public PostedTaxonomies PostedTaxonomies { get; set; }

        public IList<Term> AvailableTerms { get; set; }
        public IList<Term> SelectedTerms { get; set; }
        public PostedTerms PostedTerms { get; set; }

    }

    public class PostedTaxonomies
    {
        // this array will be used to POST values from the form to the controller
        public string[] TaxonomyIDs { get; set; }
    }

    public class PostedTerms
    {
        // this array will be used to POST values from the form to the controller
        public string[] TermIDs { get; set; }
    }

}