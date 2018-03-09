using System;
using System.Collections.Generic;


namespace MicaData
{
    public partial class variable_vw : MicaBase
    {
        public string field_variable_missing_text
        {
            get
            {
                if (field_variable_categories_missing == 1)
                    return "(Missing)";
                else return null;
            }

            set
            {
            }
        }
    }
}
