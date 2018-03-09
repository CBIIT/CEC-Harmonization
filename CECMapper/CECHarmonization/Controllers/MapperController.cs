using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CECHarmonization.DATA;
using CECHarmonization.Models;
using MicaData;
using Omu.AwesomeMvc;

namespace CECHarmonization.Controllers
{
    public class MapperController : Controller
    {

        MicaRepository _mica_ado = new MicaRepository();
        private Entities _micaEntities = new Entities();
        private MapContext _mapdb = new MapContext();


        // GET: Mapper
        public ActionResult Index(short_variable_vw target, List<short_variable_vw> cohorts)
        {


            // in future versions, we may come to this page with parameters, but at this time we expect nulls

            Mapper m = new Mapper();
            // if parameters are null, show existing maps
            if (!string.IsNullOrEmpty(target.dataset_name) && cohorts.Count != 0)
            {
                
                //m = new Mapper { selectedTargetVariableId = target, selectedCohorts = cohorts };// new List<Cohort> { new Cohort { Id = 885 }, new Cohort { Id = 888 }, new Cohort { Id = 890 } } };
            }
            else
            {
                return RedirectToAction("GetMapInput");
                // prompt for whatever is missing and create Mapper
                //m = null; 
                //m = new Mapper { selectedTargetVariableId = "800", selectedCohortIds = new List<Cohort> { new Cohort { Id = 885 }, new Cohort { Id = 888 }, new Cohort { Id = 890 } } };

            }

            return View(m);
        }



        public PartialViewResult GetTarget(string target)
        {


            Mapper m = new Mapper();

            return PartialView(m);
        }


        public ActionResult GetMapInput()
        {

            return View();
        }

        public ActionResult GridGetItems(GridParams g, string search)
        {
            search = (search ?? "").ToLower();
            //var list = mapdb.Mappers.Where(o => o.ID.ToLower().Contains(search) || o.Product.ToLower().Contains(search));
            var list = _mapdb.Mappers.Where(o => o.Id.ToString() == search);


            var builder = new GridModelBuilder<Mapper>(list.AsQueryable(), g)
            {
                Key = "Id",
                //Map = o => new { o.Id, o.Person, o.Food, o.Date, o.Price, o.Location, ChefName = o.Chef.FirstName + " " + o.Chef.LastName },
                //MakeFooter = MakeFooter,
                //MakeHeader = gr =>
                //{
                //    //get first item in the group
                //    var first = gr.Items.First();

                //    //get the grouped column value(s) for the first item
                //    var val = string.Join(" ", AweUtil.GetColumnValue(gr.Column, first).Select(ToStr));

                //    return new GroupHeader
                //    {
                //        Content = string.Format(" {0} : {1} ( Count = {2}, <input type='checkbox' name='id' value='{3}'/> )",
                //                gr.Header,
                //                val,
                //                gr.Items.Count(),
                //            //gr.Items.Max(o => o.delta)
                //                null),
                //        Collapsed = collapsed
                //    };
                //}
            };

            var j = builder.Build();

            return Json(j);


            //return Json(new GridModelBuilder<Mapper>(list.OrderByDescending(o => o.ID).AsQueryable(), g)
            //{
            //    Key = "Id", // needed for api select, update, tree, nesting, EF
            //    GetItem = () => mapdb.Mappers.Where(o => o.ID == (Convert.ToInt32(g.Key))
            //}.Build());
        }

        //public ActionResult Delete(int id, string gridId)
        //{
        //    //var purchase = mapdb.Get<Mapper>(id);
        //    Mapper mDelete = _mapdb.Mappers.Find(id);

        //    return View(new DeleteConfirmInput
        //    {
        //        Id = id,
        //        GridId = gridId,
        //        Message = string.Format("Are you sure you want to delete Map Records for Target {0}", mDelete.Selection.selectedTargetDatasetId)
        //    });
        //}

        [HttpPost]
        public ActionResult Delete(Mapper input)
        {

            Mapper mDelete = _mapdb.Mappers.Find(input.Id);
            _mapdb.Mappers.Remove(mDelete);
            _mapdb.SaveChanges();
            return Json(new { input.Id });
        }


        public ActionResult Edit(int id)
        {
            Mapper m = _mapdb.Mappers.Where(o => o.Id == id).First();
            var input = new Mapper();
            //{
            //    selection = new  MapSelection {  .selectedCohorts = m.selection.selectedCohorts,
            //    selectedTargetDatasetId = m.selectedTargetDatasetId,
            //    selectedTargetVariableId = m.selectedTargetVariableId,
            //    mappings = m.mappings
            //};

            return View(input);
        }

        //[HttpPost]
        //public ActionResult Edit(Mapper input)
        //{
        //    if (!ModelState.IsValid) return View(input);

        //    var o = Db.Get<Purchase>(input.Id);
        //    o.Customer = input.Customer;
        //    o.Date = input.Date.Value;
        //    o.Product = input.Product;
        //    o.Quantity = input.Quantity;
        //    Db.Update(o);
        //    return Json(o);
        //}

        public ActionResult Create()
        {
            return View("Edit");
        }

        //[HttpPost]
        //public ActionResult Create(PurchaseInput input)
        //{
        //    if (!ModelState.IsValid) return View("Edit", input);
        //    var o = new Purchase
        //    {
        //        Customer = input.Customer,
        //        Date = input.Date.Value,
        //        Quantity = input.Quantity,
        //        Product = input.Product
        //    };
        //    Db.Insert(o);
        //    return Json(o);
        //}


        public ActionResult GetDatasetItems(int? v, string type)
        {

            var data = _micaEntities.dataset_vw.Where(o => o.field_dataset_type_value == type).Select(u => new SelectableItem
            {
                Value = u.entity_id.ToString(),
                Text = u.title,
                Selected = u.entity_id == v
            });

            return Json(data);

            //var ds = micadb.dataset_vw.Select(o => new SelectableItem(o.nid, o.title, v == 0)).ToList();

            //return Json(ds);// value, text, selected
        }


        public ActionResult GetVariableItems(int? v, int? parent)
        {

            List<SelectableItem> items = new List<SelectableItem>();

            IEnumerable<short_variable_vw> data = _micaEntities.short_variable_vw.Where(o => o.dataset_id.ToString() == parent.ToString()).ToList();

            foreach (short_variable_vw var in data)
            {
                items.Add(new SelectableItem { Text = var.title, Value = var.nid, Selected = v == var.nid });

            }

            return Json(items);

            //            var data2 = data.ToList().Where(o => o.V == parent.ToString());


            //var data = micadb.variable_vw.Select(u => new SelectableItem
            //{
            //    Value = u.field_label_value.ToString(),
            //    Text = u.field_label_value,
            //    Selected = u.nid == v,
            //    V = u.dataset_id.ToString()
            //}).Distinct();


            //var data2 = data.ToList().Where(o => o.V == parent.ToString());

            //           return Json(data2);
            //            return Json(micadb.variable_vw.Where(o => o.dataset_id == parent)
            //              .Select(o => new SelectableItem(o.entity_id, o.dataset_name, v == o.dataset_id)));// key, text, selected
        }


        //public PartialViewResult GetTargetSelection(string id, Mapper m)
        //{

        //    if (!String.IsNullOrEmpty(id))
        //        ViewBag.IDs = id;
        //    else
        //        ViewBag.IDs = m.Selection.selectedTargetVariableId;


        //    return PartialView("TargetGrid");

        //}


        public ActionResult TargetGetItems(GridParams g, string IDList)
        {

            if (!string.IsNullOrEmpty(IDList))
            {
                Entities db = new Entities();
                MicaRepository ado = new MicaRepository();
                IDList = IDList.Replace("[", "");
                IDList = IDList.Replace("]", "");
                IDList = IDList.Replace("\"", "");

                //List<string> nodes = IDList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                String[] nodes = IDList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();


                //passing 2 Functions MakeFooter and MakeHeader to customize header and add footer
                try
                {

                    var dt = db.short_variable_vw.ToList().Where(o => nodes.Contains(o.nid.ToString()));
                    //var dt = ado.GetVariable_vw(GroupedIds);


                    //var builder = new GridModelBuilder<variable_vw>(db.variable_vw.AsQueryable(), g)
                    var builder = new GridModelBuilder<short_variable_vw>(dt.AsQueryable(), g)
                    {
                        Key = "nid"
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


        public ActionResult GetCohortSelection(string id, Mapper m)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                    ViewBag.IDs = id;
                else
                    ViewBag.IDs = null;
                //    ViewBag.IDs = m.Selection.selectedCohortVariableId;

                //int selected_id = Int32.Parse(m.Selection.selectedCohortVariableId);

                //m.selectedCohortIds.Add(new Cohort { Id = selected_id, Text = "unknown" });

            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
        
            return PartialView("CohortGridHTML",m);

        }



        public ActionResult CohortGetItems(GridParams g, string IDList)
        {

            if (!string.IsNullOrEmpty(IDList))
            {
                Entities db = new Entities();
                MicaRepository ado = new MicaRepository();
                IDList = IDList.Replace("[", "");
                IDList = IDList.Replace("]", "");
                IDList = IDList.Replace("\"", "");

                //List<string> nodes = IDList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                String[] nodes = IDList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();


                //passing 2 Functions MakeFooter and MakeHeader to customize header and add footer
                try
                {

                    var dt = db.short_variable_vw.ToList().Where(o => nodes.Contains(o.nid.ToString()));
                    //var dt = ado.GetVariable_vw(GroupedIds);


                    //var builder = new GridModelBuilder<variable_vw>(db.variable_vw.AsQueryable(), g)
                    var builder = new GridModelBuilder<short_variable_vw>(dt.AsQueryable(), g)
                    {
                        Key = "nid"
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



    }

}