using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CECHarmonization.ViewModels;
using CECHarmonization.DATA;
using Omu.AwesomeMvc;
using CECHarmonization.Models;
using MicaData;
using System.IO;
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace CECHarmonization.Controllers
{

    public class IndexInput
    {
        public int? Tax { get; set; }
        public int? Term { get; set; }
    }

    public class PreMapController : Controller
    {
        DATA.MicaRepository ado = new DATA.MicaRepository();
        private Entities db = new Entities();

        public ActionResult Index()
        {


            //if (AlreadyPopulated == false)
            //{
            var treeView = new TreeView();
            var treeNode = new TreeNode();
            var rootNode = new ListItem("Root node 1", "node1", "-99", null, null, null);

            // get the top level
            IEnumerable<taxonomy_vw> selectedTaxonomies = ado.GetTaxonomy_vw(null, "parent is null");

            foreach (taxonomy_vw tv in selectedTaxonomies)
            {
                ListItem li = new ListItem(tv.name, tv.tid.ToString(), rootNode.Parent.ToString(), null, null, null);

                // populate the children on this item
                PopulateTree(li);

                //add this item to the root node
                rootNode.Nodes.Add(li);
            }

            AlreadyPopulated = true;

            treeNode.ListItems.Add(rootNode);
            treeView.Nodes.Add(treeNode);


            return View(treeView);
            //return Json(rootNode);
            //}
            //else
            //{
            //    return null;
            //}

        }



        /// <summary>
        /// First I had to resolve the issue that a browser refresh would repaint the whole treeview. It’s possible that I simply missed this when I cherry picked code from the FileManager codeproject example.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Test(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            Session["AlreadyPopulated"] = false;
            return View();
        }



        /// <summary>
        /// Populate the Taxonomy TreeView 
        /// </summary>
        /// <param name="">
        /// <param name="node">The "master" node, to populate</param>
        public void PopulateTree(ListItem parent_node)
        {

            if (parent_node.Nodes == null)
            {
                var new_node = new List<ListItem>();

            }

            int index = (parent_node.Id).IndexOf("_") + 1;
            string parent = (parent_node.Id).Substring(index);

            // get the information at this level
            IEnumerable<taxonomy_vw> selectedTaxonomies = ado.GetTaxonomy_vw(null, string.Format("parent = {0}", parent));

            // loop through each taxonomy
            foreach (taxonomy_vw tv in selectedTaxonomies)
            {
                // create a new node
                ListItem li = new ListItem(tv.name, '-' + tv.vid.ToString() + '_' + tv.tid.ToString(), tv.parent.ToString(), null, null, null);

                // populate the children on this item
                PopulateTree(li);

                //add this item to the root node
                parent_node.Nodes.Add(li);
            }

        }

        // Don't load the jsTree treeview again if it has already been populated.
        // Note: this causes a bug where the tree won't repaint on browser refresh
        public bool AlreadyPopulated
        {
            get
            {
                return (Session["AlreadyPopulated"] == null ? false : (bool)Session["AlreadyPopulated"]);
            }
            set
            {
                Session["AlreadyPopulated"] = (bool)value;
            }

        }


        public ActionResult GetTreeSelection(string id, string children, string selectedText)
        {

            // add this selection to Session (to be used for export to excel
            Session["PreMap_selectedText"] = selectedText;

            //  if (!String.IsNullOrEmpty(children))
            //      ViewBag.IDs = id + "," + children;
            //  else
            ViewBag.IDs = id;

            

            return PartialView("_GridView");

        }


        // End JSTree


        /// <summary>
        /// get the items for the main PreMapping grid
        /// <param name="g"></param>
        /// <param name="abc"></param>
        /// <param name="collapsed"></param>
        /// <param name="IDList"></param>
        /// <returns></returns>
        public ActionResult GetItems(GridParams g, int[] abc, bool collapsed, string IDList, string search)
        {

            search = (search ?? "").ToLower();

            if (!string.IsNullOrEmpty(IDList))
            {
                Entities db = new Entities();
                DATA.MicaRepository ado = new DATA.MicaRepository();
                IDList = IDList.Replace("[", "");
                IDList = IDList.Replace("]", "");
                IDList = IDList.Replace("\"", "");

                //List<string> nodes = IDList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                String[] nodearray = IDList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();


                // the last item was most recently selected-- find its children and add to the list
                //  ********

                Dictionary<string, string> GroupedIds = new Dictionary<string, string>();
                string lastgroup = null;

                foreach (string item in nodearray)
                {

                    if (item.Contains('_'))  // skip top level item, it does not have a node id
                    {
                        string[] i = item.Split('_');
                        if (i[0] != lastgroup)


                            // ned to build something like this:   (a.tids REGEXP '^243|_243' or a.tids REGEXP '^244|_244')
                            try
                            {
                                GroupedIds.Add(i[0], "a.tids REGEXP " + "'^" + i[1] + "|_" + i[1] + "'");
                            }
                            catch (ArgumentException)
                            {
                                string cur;
                                // remove the current item that is already in the dictionary; group items into a single string and Add back
                                GroupedIds.TryGetValue(i[0], out cur);
                                GroupedIds.Remove(i[0]);
                                GroupedIds.Add(i[0], cur + " or a.tids REGEXP " + "'^" + i[1] + "|_" + i[1] + "'");

                            }

                    }
                }

                //passing 2 Functions MakeFooter and MakeHeader to customize header and add footer
                try
                {
                    // add this selection to Session 
                    Session["PreMap_GroupedIds"] = GroupedIds;
                    Session["PreMap_search"] = search;

                    //var dt = db.variable_vw.ToList().Where(o => nodes.Contains(o.nid.ToString()));
                    var dt = ado.GetVariable_vw(GroupedIds).Where(o => o.study_name.ToLower().Contains(search));

                    int cnt = dt.Count();
                    ViewBag.CNT = cnt;

                    //var builder = new GridModelBuilder<variable_vw>(db.variable_vw.AsQueryable(), g)
                    var builder = new GridModelBuilder<variable_vw>(dt.AsQueryable(), g)
                    {
                        Key = "nid",
                        //Map = o => new { o.Id, o.Person, o.Food, o.Date, o.Price, o.Location, ChefName = o.Chef.FirstName + " " + o.Chef.LastName },
                        MakeFooter = MakeFooter,
                        MakeHeader = gr =>
                        {
                            //get first item in the group
                            var first = gr.Items.First();

                            //get the grouped column value(s) for the first item
                            var val = string.Join(" ", AweUtil.GetColumnValue(gr.Column, first).Select(ToStr));

                            string content_str = null;
                            if (gr.Column == "field_label_value")
                                content_str = " <font color='darkslateblue'><b>{0} ({2})</b> : {1} <br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Item Description</b>: {3} </font> )";
                            else
                                content_str = " <font color='darkslateblue'><b>{0} ({2})</b> : {1} </font>";


                                return new GroupHeader
                                {
                                    //Content = string.Format(" <font color='darkslateblue'><b>{0}</b> : {1} &nbsp;&nbsp;&nbsp; <b>Count</b> = {2} <br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Item Description</b> = {3} </font> )",
                                    Content = string.Format(content_str,
                                            gr.Header,
                                            val,
                                            gr.Items.Count(),
                                            gr.Items.Max(o => o.body_value)
                                        //gr.Items.Count(),
                                        //gr.Items.Max(o => o.delta)
                                        //null
                                            ),
                                    Collapsed = collapsed

                                };
                        }
                    };

                    var j = builder.Build();

                    return Json(j);
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    return null;
                }
            }
            return null;
        }

        private object MakeFooter(GroupInfo<variable_vw> info)
        {
            //will add the word Total at the grid level footer (Level == 0)
            var pref = info.Level == 0 ? "Total " : "";

            return new
            {
                Title = pref + " count = " + info.Items.Count()   //,
                //Location = info.Items.Select(o => o.Location).Distinct().Count() + " distinct locations",
                //Date = pref + " max: " + info.Items.Max(o => o.Date).Date.ToShortDateString(),
                //Price = info.Items.Sum(o => o.Price),
                //ChefCount = info.Items.Select(o => o.Chef.Id).Distinct().Count() + " chefs"
            };
        }

        private string ToStr(object o)
        {
            return o is DateTime ? ((DateTime)o).ToShortDateString() : o.ToString();
        }


    }


    public class taxonomy_vocabularyController : Controller
    {
        private Entities db = new Entities();

        //// get SelectableItem list for Taxonomy        
        //public ActionResult GetItems(int? v)
        //{

        //    var data = db.taxonomy_vocabulary.Select(u => new SelectableItem
        //                            {
        //                                Value = u.vid.ToString(),
        //                                Text = u.name,
        //                                Selected = u.vid == v
        //                            });

        //    return Json(data);
        //}
    }

    public class taxonomy_term_dataController : Controller
    {
        private Entities db = new Entities();

        // get SelectableItem list for Terms        
        //public ActionResult GetItems(int? v, int[] parent)
        //{

        //    parent = parent ?? new int[] { };
        //    //v = v ?? new int[] { };

        //    //var data = db.taxonomy_term_data.Where(o => o.tid == parent).ToList().Select(u => new SelectableItem
        //    //var data = db.taxonomy_term_data.Where(o => o.vid == parent).Select(u => new SelectableItem

        //    //IEnumerable<taxonomy_term_data> data1 = db.taxonomy_term_data.Where(o => parent.Contains(Convert.ToInt32(o.vid)));

        //    var data = db.taxonomy_term_data.Select(u => new SelectableItem
        //    {
        //        Value = u.tid.ToString(),
        //        Text = u.name,
        //        Selected = u.tid == v,
        //        V = u.vid.ToString()
        //    });


        //    var data2 = data.ToList().Where(o => parent.Contains(Convert.ToInt32(o.V)));


        //    return Json(data2);
        //}




    }

    //public class nodesController : Controller
    //{
    //    private Entities db = new Entities();


    //    public ActionResult GetItems(GridParams g, int[] abc, bool collapsed)
    //    {
    //        //passing 2 Functions MakeFooter and MakeHeader to customize header and add footer
    //        try
    //        {
    //            var builder = new GridModelBuilder<variable_vw>(db.variable_vw.AsQueryable(), g)
    //                {
    //                    Key = "Id",
    //                    //Map = o => new { o.Id, o.Person, o.Food, o.Date, o.Price, o.Location, ChefName = o.Chef.FirstName + " " + o.Chef.LastName },
    //                    MakeFooter = MakeFooter,
    //                    MakeHeader = gr =>
    //                    {
    //                        //get first item in the group
    //                        var first = gr.Items.First();

    //                        //get the grouped column value(s) for the first item
    //                        var val = string.Join(" ", AweUtil.GetColumnValue(gr.Column, first).Select(ToStr));

    //                        return new GroupHeader
    //                        {
    //                            Content = string.Format(" {0} : {1} ( Count = {2}, <input type='checkbox' name='id' value='{3}'/> )",
    //                                    gr.Header,
    //                                    val,
    //                                    gr.Items.Count(),
    //                                //gr.Items.Max(o => o.delta)
    //                                    null),
    //                            Collapsed = collapsed
    //                        };
    //                    }
    //                };

    //            var j = builder.Build();

    //            return Json(j);
    //        }
    //        catch (Exception ex)
    //        {
    //            string message = ex.Message;
    //            return null;
    //        }
    //    }

    //    private object MakeFooter(GroupInfo<variable_vw> info)
    //    {
    //        //will add the word Total at the grid level footer (Level == 0)
    //        var pref = info.Level == 0 ? "Total " : "";

    //        return new
    //        {
    //            Title = pref + " count = " + info.Items.Count()   //,
    //            //Location = info.Items.Select(o => o.Location).Distinct().Count() + " distinct locations",
    //            //Date = pref + " max: " + info.Items.Max(o => o.Date).Date.ToShortDateString(),
    //            //Price = info.Items.Sum(o => o.Price),
    //            //ChefCount = info.Items.Select(o => o.Chef.Id).Distinct().Count() + " chefs"
    //        };
    //    }

    //    private string ToStr(object o)
    //    {
    //        return o is DateTime ? ((DateTime)o).ToShortDateString() : o.ToString();
    //    }

    //    public ActionResult GetTaxonomy(GridParams g)
    //    {
    //        var list = db.taxonomy_vocabulary.AsQueryable();

    //        var model = new GridModelBuilder<taxonomy_vocabulary>(list.AsQueryable(), g)
    //        {
    //            Key = "vid"
    //        }.Build();

    //        return Json(model);

    //        //return Json(new GridModelBuilder<Mica_Variable>(list, g)
    //        //{
    //        //    // Key = "Id", // needed for Entity Framework, nesting, tree, grid api (select, update)
    //        //    // EF can't do paging unless it orders at least by one column so we will order by that column when there is no sorted columns
    //        //}.Build());
    //    }


    //}



}

