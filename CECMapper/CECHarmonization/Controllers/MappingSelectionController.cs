using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CECHarmonization.Models;
using MicaData;

namespace CECHarmonization.Controllers
{
    public class MappingSelectionController : Controller
    {
        // GET: MappingSelection
        MicaDAL.MicaRepository ado = new MicaDAL.MicaRepository();
        private Entities micadb = new Entities();
        private MapContext mapdb = new MapContext();

        public ActionResult Index(Mapper m)
        {

            // in future versions, we may come to this page with parameters, but at this time we expect nulls

//            Mapper m = new Mapper();
            //if (!string.IsNullOrEmpty(target) && cohorts.Count != 0)
            //{
            //    m = new Mapper { selectedTargetVariableId = target, selectedCohorts = cohorts };// new List<Cohort> { new Cohort { Id = 885 }, new Cohort { Id = 888 }, new Cohort { Id = 890 } } };
            //}
            //else
            //{

               
            //    // prompt for whatever is missing and create Mapper
            //    //m = null; 
            //    //m = new Mapper { selectedTargetVariableId = "800", selectedCohortIds = new List<Cohort> { new Cohort { Id = 885 }, new Cohort { Id = 888 }, new Cohort { Id = 890 } } };

            //}

            return View(m);
        }

        [HttpPost]
        public ActionResult Edit( Mapper m, string action)
        {
            switch (action)
            {
                case "delete":
                    // delete action
                    break;
                case "save":
                    // save action
                    short_variable_vw sv =  micadb.short_variable_vw.Where(o => o.nid.ToString() == m.selection.selectedTargetVariableId.ToString()).First();
                    m.selection.selectedTarget = sv;
                    break;
            }

            return View("Index", m);
        }



    }
}