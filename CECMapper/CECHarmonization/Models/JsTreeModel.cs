using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CECHarmonization.Models
{

    public class JsTreeModel
    {
        public string data;
        public JsTreeAttribute attr;
        // this was "open" but changing it to “leaf” adds “jstree-leaf” to the class   
        public string state = "leaf";
        public List<JsTreeModel> children;
    }

    public class JsTreeAttribute
    {
        public string id;
    }
}