using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Projekt_1.Model;

namespace Projekt_1.Controllers
{
    public class passbooksController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();

        // GET: passbooks
        public ActionResult Index()
        {
            var passbooks = db.passbooks.Include(p => p.user);
            return View(passbooks.ToList());
        }

        // GET: passbooks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            passbook passbook = db.passbooks.Find(id);
            if (passbook == null)
            {
                return HttpNotFound();
            }
            return View(passbook);
        }

        // GET: passbooks/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address");
            return View();
        }

        // POST: passbooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,passbook_id,balance,startDate,endDate,term")] passbook passbook)
        {
            if (ModelState.IsValid)
            {
                db.passbooks.Add(passbook);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", passbook.user_id);
            return View(passbook);
        }

        // GET: passbooks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            passbook passbook = db.passbooks.Find(id);
            if (passbook == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", passbook.user_id);
            return View(passbook);
        }

        // POST: passbooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,passbook_id,balance,startDate,endDate,term")] passbook passbook)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passbook).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", passbook.user_id);
            return View(passbook);
        }

        // GET: passbooks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            passbook passbook = db.passbooks.Find(id);
            if (passbook == null)
            {
                return HttpNotFound();
            }
            return View(passbook);
        }

        // POST: passbooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            passbook passbook = db.passbooks.Find(id);
            db.passbooks.Remove(passbook);
            db.SaveChanges();
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
