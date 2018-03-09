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
using System.Data.Entity.Infrastructure;

namespace CECHarmonization.Controllers
{
    public class field_data_field_sva_statusController : Controller
    {
        private Entities db = new Entities();

        // GET: field_data_field_sva_status
        public async Task<ActionResult> Index()
        {
            return View(await db.field_data_field_sva_status.ToListAsync());
        }

        // GET: field_data_field_sva_status/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            field_data_field_sva_status field_data_field_sva_status = await db.field_data_field_sva_status.FindAsync(id);
            if (field_data_field_sva_status == null)
            {
                return HttpNotFound();
            }
            return View(field_data_field_sva_status);
        }

        // GET: field_data_field_sva_status/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: field_data_field_sva_status/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "entity_type,bundle,deleted,entity_id,revision_id,language,delta,field_sva_status_value")] field_data_field_sva_status field_data_field_sva_status)
        {
            if (ModelState.IsValid)
            {
                db.field_data_field_sva_status.Add(field_data_field_sva_status);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(field_data_field_sva_status);
        }

        // GET: field_data_field_sva_status/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //            field_data_field_sva_status field_data_field_sva_status = await db.field_data_field_sva_status.FindAsync(id);
            field_data_field_sva_status field_data_field_sva_status = db.field_data_field_sva_status.Where(o => o.entity_id.ToString() == id).First();
            if (field_data_field_sva_status == null)
            {
                return HttpNotFound();
            }
            return View(field_data_field_sva_status);
        }

        // POST: field_data_field_sva_status/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "entity_type,bundle,deleted,entity_id,revision_id,language,delta,field_sva_status_value")] field_data_field_sva_status field_data_field_sva_status)
        {
            if (ModelState.IsValid)
            {

                //                db.Entry(field_data_field_sva_status).State = EntityState.Modified;
                //                await db.SaveChangesAsync();

                field_data_field_sva_status recToUpdate = db.field_data_field_sva_status.Where(o => o.entity_id == field_data_field_sva_status.entity_id).First();

                recToUpdate.field_sva_status_value = field_data_field_sva_status.field_sva_status_value;

                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }


                return RedirectToAction("Index");
            }

            return View(field_data_field_sva_status);
        }

        // GET: field_data_field_sva_status/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            field_data_field_sva_status field_data_field_sva_status = await db.field_data_field_sva_status.FindAsync(id);
            if (field_data_field_sva_status == null)
            {
                return HttpNotFound();
            }
            return View(field_data_field_sva_status);
        }

        // POST: field_data_field_sva_status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            field_data_field_sva_status field_data_field_sva_status = await db.field_data_field_sva_status.FindAsync(id);
            db.field_data_field_sva_status.Remove(field_data_field_sva_status);
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
