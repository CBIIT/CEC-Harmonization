using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicaData.MicaPartialClasses
{
    class TaxonomyBase
    {
        string entity_type {get; set;}
        string bundle {get; set;}
        int deleted {get; set;}
        int entity_id {get; set;}
        int revision_id {get; set;}
        string language {get; set;}
        int delta {get; set;}
        int tid { get; set; }
    }
}
