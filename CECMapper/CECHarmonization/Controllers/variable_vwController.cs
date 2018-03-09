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

namespace CECHarmonization.Controllers
{
    public class variable_vwController : Controller
    {
        private Entities db = new Entities();

        // GET: variable_vw
        public async Task<ActionResult> Index()
        {
            return View(await db.variable_vw.ToListAsync());
        }

        // GET: variable_vw/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            variable_vw variable_vw = await db.variable_vw.FindAsync(id);
            if (variable_vw == null)
            {
                return HttpNotFound();
            }
            return View(variable_vw);
        }

        // GET: variable_vw/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: variable_vw/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "nid,vid,title,status,field_label_value,dataset_name,entity_id,entity_type,delta,field_variable_categories_name,field_variable_categories_label,field_variable_categories_missing")] variable_vw variable_vw)
        {
            if (ModelState.IsValid)
            {
                db.variable_vw.Add(variable_vw);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(variable_vw);
        }

        // GET: variable_vw/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            variable_vw variable_vw = await db.variable_vw.FindAsync(id);
            if (variable_vw == null)
            {
                return HttpNotFound();
            }
            return View(variable_vw);
        }

        // POST: variable_vw/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "nid,vid,title,status,field_label_value,dataset_name,entity_id,entity_type,delta,field_variable_categories_name,field_variable_categories_label,field_variable_categories_missing")] variable_vw variable_vw)
        {
            if (ModelState.IsValid)
            {
                db.Entry(variable_vw).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(variable_vw);
        }

        // GET: variable_vw/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            variable_vw variable_vw = await db.variable_vw.FindAsync(id);
            if (variable_vw == null)
            {
                return HttpNotFound();
            }
            return View(variable_vw);
        }

        // POST: variable_vw/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            variable_vw variable_vw = await db.variable_vw.FindAsync(id);
            db.variable_vw.Remove(variable_vw);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
