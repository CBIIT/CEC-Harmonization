using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CECHarmonization.Controllers
{
    public class Option2Controller : Controller
    {
        // GET: Option2
        public ActionResult Index()
        {
            return View();
        }

        // GET: Option2/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Option2/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Option2/Create
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

        // GET: Option2/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Option2/Edit/5
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

        // GET: Option2/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Option2/Delete/5
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
    }
}
