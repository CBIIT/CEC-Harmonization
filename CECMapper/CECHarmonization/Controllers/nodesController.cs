using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MicaData;
using Omu.AwesomeMvc;

namespace CECHarmonization.Controllers
{

    public class taxonomy_vocabulary_Controller : Controller
    {
        private Entities db = new Entities();

        // get SelectableItem list for Taxonomy        
        public ActionResult GetItems(int? v)
        {

            var data = db.taxonomy_vocabulary.Select(u => new SelectableItem
            {
                Value = u.vid.ToString(),
                Text = u.name,
                Selected = u.vid == v
            });

            return Json(data);
        }
    }

    public class taxonomy_term_data_Controller : Controller
    {
        private Entities db = new Entities();

        // get SelectableItem list for Terms        
        public ActionResult GetItems(int? v, int[] parent)
        {

            parent = parent ?? new int[] { };
            //v = v ?? new int[] { };

            //var data = db.taxonomy_term_data.Where(o => o.tid == parent).ToList().Select(u => new SelectableItem
            //var data = db.taxonomy_term_data.Where(o => o.vid == parent).Select(u => new SelectableItem

            //IEnumerable<taxonomy_term_data> data1 = db.taxonomy_term_data.Where(o => parent.Contains(Convert.ToInt32(o.vid)));

            var data = db.taxonomy_term_data.Select(u => new SelectableItem
            {
                Value = u.tid.ToString(),
                Text = u.name,
                Selected = u.tid == v,
                V = u.vid.ToString()
            });


            var data2 = data.ToList().Where(o => parent.Contains(Convert.ToInt32(o.V)));


            return Json(data2);
        }




    }

    public class nodes_Controller : Controller
    {
        private Entities db = new Entities();


        public ActionResult GetItems(GridParams g, int[] abc, bool collapsed)
        {
            //passing 2 Functions MakeFooter and MakeHeader to customize header and add footer
            try
            {
                var builder = new GridModelBuilder<variable_vw>(db.variable_vw.AsQueryable(), g)
                {
                    Key = "Id",
                    //Map = o => new { o.Id, o.Person, o.Food, o.Date, o.Price, o.Location, ChefName = o.Chef.FirstName + " " + o.Chef.LastName },
                    MakeFooter = MakeFooter,
                    MakeHeader = gr =>
                    {
                        //get first item in the group
                        var first = gr.Items.First();

                        //get the grouped column value(s) for the first item
                        var val = string.Join(" ", AweUtil.GetColumnValue(gr.Column, first).Select(ToStr));

                        return new GroupHeader
                        {
                            Content = string.Format(" {0} : {1} ( Count = {2}, Max Price = {3} )",
                                    gr.Header,
                                    val,
                                    gr.Items.Count(),
                                //gr.Items.Max(o => o.delta)
                                    null),
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

        public ActionResult GetTaxonomy(GridParams g)
        {
            var list = db.taxonomy_vocabulary.AsQueryable();

            var model = new GridModelBuilder<taxonomy_vocabulary>(list.AsQueryable(), g)
            {
                Key = "vid"
            }.Build();

            return Json(model);

            //return Json(new GridModelBuilder<Mica_Variable>(list, g)
            //{
            //    // Key = "Id", // needed for Entity Framework, nesting, tree, grid api (select, update)
            //    // EF can't do paging unless it orders at least by one column so we will order by that column when there is no sorted columns
            //}.Build());
        }


    }




}
    
