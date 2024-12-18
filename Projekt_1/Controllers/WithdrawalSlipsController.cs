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
    public class WithdrawalSlipsController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();

        // GET: WithdrawalSlips
        public ActionResult Index()
        {
            var withdrawalSlips = db.WithdrawalSlips.Include(w => w.passbook).Include(w => w.user);
            return View(withdrawalSlips.ToList());
        }

        // GET: WithdrawalSlips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WithdrawalSlip withdrawalSlip = db.WithdrawalSlips.Find(id);
            if (withdrawalSlip == null)
            {
                return HttpNotFound();
            }
            return View(withdrawalSlip);
        }

        // GET: WithdrawalSlips/Create
        public ActionResult Create()
        {
            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID");
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address");
            return View();
        }

        [HttpGet]
        public JsonResult GetCustomerDetails(int savingsBookID)
        {
            var customerDetails = db.passbooks
                                    .Where(p => p.SavingsBookID == savingsBookID)
                                    .Select(p => new { user_id = p.user_id, user_name = p.user.user_name })
                                    .FirstOrDefault();
            return Json(customerDetails, JsonRequestBehavior.AllowGet);
        }


        // POST: WithdrawalSlips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: WithdrawalSlips/Create
        // POST: WithdrawalSlips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WithdrawalID,SavingsBookID,user_id,WithdrawalAmount,WithdrawalDate")] WithdrawalSlip withdrawalSlip)
        {
            if (ModelState.IsValid)
            {
                // Nhận thông tin `user_id` từ SavingsBookID
                var userId = db.passbooks
                             .Where(p => p.SavingsBookID == withdrawalSlip.SavingsBookID)
                             .Select(p => p.user_id)
                             .FirstOrDefault();

                // Cập nhật `user_id` trong WithdrawalSlip
                withdrawalSlip.user_id = userId;

                db.WithdrawalSlips.Add(withdrawalSlip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID", withdrawalSlip.SavingsBookID);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", withdrawalSlip.user_id);
            return View(withdrawalSlip);
        }



        // GET: WithdrawalSlips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WithdrawalSlip withdrawalSlip = db.WithdrawalSlips.Find(id);
            if (withdrawalSlip == null)
            {
                return HttpNotFound();
            }
            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID", withdrawalSlip.SavingsBookID);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", withdrawalSlip.user_id);
            return View(withdrawalSlip);
        }

        // POST: WithdrawalSlips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WithdrawalID,SavingsBookID,user_id,WithdrawalAmount,WithdrawalDate")] WithdrawalSlip withdrawalSlip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(withdrawalSlip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID", withdrawalSlip.SavingsBookID);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_address", withdrawalSlip.user_id);
            return View(withdrawalSlip);
        }

        // GET: WithdrawalSlips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WithdrawalSlip withdrawalSlip = db.WithdrawalSlips.Find(id);
            if (withdrawalSlip == null)
            {
                return HttpNotFound();
            }
            return View(withdrawalSlip);
        }

        // POST: WithdrawalSlips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WithdrawalSlip withdrawalSlip = db.WithdrawalSlips.Find(id);
            db.WithdrawalSlips.Remove(withdrawalSlip);
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
