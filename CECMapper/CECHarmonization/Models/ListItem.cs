using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CECHarmonization.Models
{
    public class TreeView
    {
        public TreeView()
        {
            this.Nodes = new List<TreeNode>();
          
        }

        public string selected_nodes { get; set; }
        public List<TreeNode> Nodes { get; set; }
    }

    public class TreeNode
    {
        public TreeNode()
        {
            ListItems = new List<ListItem>();
        }

        public List<ListItem> ListItems { get; set; }
    }

    public class ListItem : INodeItem
    {
        public string Id { get; set; }
        public string Parent { get; set; }
        public string Class { get; set; }
        public string Text { get; set; }
        public ListItemAHref Href { get; set; }
        public DataJsTree DataJsTree { get; set; }
        public List<ListItem> Nodes { get; set; }

        public ListItem(string text,
                        string id = "",
                        string parent = "#",
                        string className = "",
                        ListItemAHref ahref = null,
                        DataJsTree dataJsTree = null)
        {
            if (id != null)
                this.Id = id;

            this.Parent = parent;
            this.Text = text;
            this.Class = className;

            if (ahref != null)
                this.Href = ahref;
            else
                this.Href = new ListItemAHref();

            if (dataJsTree != null)
                this.DataJsTree = dataJsTree;
            else
                this.DataJsTree = new DataJsTree();

            Nodes = new List<ListItem>();
        }
    }


    public class ListItemAHref
    {
        public string Href { get; set; }
        public string Class { get; set; }

        public ListItemAHref(string href = "#", string cssClass = "")
        {
            this.Href = href;
            this.Class = cssClass;
        }
    }

    public class DataJsTree
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Opened { get; set; }
        public bool Selected { get; set; }
        public bool Disabled { get; set; }
    }

    
}