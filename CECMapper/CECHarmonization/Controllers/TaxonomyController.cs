using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CECHarmonization.Models;
using CECHarmonization.ViewModels;
using MicaData;
using Omu.AwesomeMvc;

namespace CECHarmonization.Controllers
{
    public class TaxonomyController : Controller
    {

        public ActionResult TaxonomyIndex(string[] taxonomies, PostedTaxonomies postedTaxonomies)
        {
            return View(GetTaxonomiesModel(taxonomies, postedTaxonomies));
        }


        public ActionResult TaxonomyPost(string[] taxonomies, PostedTaxonomies postedTaxonomies)
        {
            TaxonomiesViewModel tvm = GetTermsModel(postedTaxonomies);

            return View("TermIndex", tvm);

        }

        public ActionResult TermPost(string[] terms, PostedTerms postedTerms)
        {

            string result = string.Join(", ", postedTerms.TermIDs);

            TempData["IDs"] = result;

            return View("VariableIndex");

        }


        // POST: Taxonomy/Edit/5
        [HttpPost]
        public ActionResult TaxonomyIndex(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here



                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }





        // todo: move to Data Service
        private TaxonomiesViewModel GetTaxonomiesModel(string[] taxonomies, PostedTaxonomies postedTaxonomies)
        {
            MicaDAL.MicaRepository db = new MicaDAL.MicaRepository();
            // setup properties
            var model = new TaxonomiesViewModel();
            var selectedTaxonomies = new List<Taxonomy>();
            var postedTaxonomyIDs = new string[0];
            if (postedTaxonomies == null) postedTaxonomies = new PostedTaxonomies();

            // if an array of posted city ids exists and is not empty,
            // save selected ids
            if (taxonomies != null && taxonomies.Any())
            {
                postedTaxonomyIDs = taxonomies;
                postedTaxonomies.TaxonomyIDs = taxonomies;
            }
            // if a view model array of posted city ids exists and is not empty,
            // save selected ids
            if (postedTaxonomies.TaxonomyIDs != null && postedTaxonomies.TaxonomyIDs.Any())
            {
                postedTaxonomyIDs = postedTaxonomies.TaxonomyIDs;
                model.WasPosted = true;
            }
            // if there are any selected ids saved, create a list of taxonomies
            if (postedTaxonomyIDs.Any())
                selectedTaxonomies = db.GetMicaTaxonomyVocabularies().ToList();

            //.GetAll().Where(x => postedTaxonomyIDs.Any(s => x.Id.ToString().Equals(s))).ToList();

            // setup a view model
            model.AvailableTaxonomies = db.GetMicaTaxonomyVocabularies().ToList();
            model.SelectedTaxonomies = selectedTaxonomies;
            model.PostedTaxonomies = postedTaxonomies;

            return model;
        }

        // todo: move to Data Service
        private TaxonomiesViewModel GetTermsModel(PostedTaxonomies postedTaxonomies)
        {
            MicaDAL.MicaRepository db = new MicaDAL.MicaRepository();
            // setup properties
            var model = new TaxonomiesViewModel();
            var selectedTerms = new List<Term>();
            var postedTermIDs = new string[0];
            if (postedTaxonomies == null) postedTaxonomies = new PostedTaxonomies();

            // if an array of posted city ids exists and is not empty,
            // save selected ids
            //if (terms != null && terms.Any())
            //{
            //    postedTermIDs = terms;
            //    postedTerms.TermIDs = terms;
            //}
            // if a view model array of posted city ids exists and is not empty,
            // save selected ids
            //if (postedTerms.TermIDs != null && postedTerms.TermIDs.Any())
            //{
            //    postedTermIDs = postedTerms.TermIDs;
            //    model.WasPosted = true;
            //}
            // if there are any selected ids saved, create a list of taxonomies
            if (postedTermIDs.Any())
                selectedTerms = db.GetMicaTerms(postedTaxonomies.TaxonomyIDs).ToList();

            //.GetAll().Where(x => postedTaxonomyIDs.Any(s => x.Id.ToString().Equals(s))).ToList();

            // setup a view model
            model.AvailableTerms = db.GetMicaTerms(postedTaxonomies.TaxonomyIDs).ToList();
            model.SelectedTerms = selectedTerms;
            //model.PostedTerms = postedTerms;

            return model;
        }



     
        #region old code

        // GET: Taxonomy
        public ActionResult Index()
        {
            return View();
        }

        // GET: Taxonomy/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Taxonomy/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Taxonomy/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Taxonomy/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Taxonomy/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Taxonomy/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Taxonomy/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        #endregion
    }
}
