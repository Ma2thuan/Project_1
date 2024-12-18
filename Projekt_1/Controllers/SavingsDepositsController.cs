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
    public class SavingsDepositsController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();

        // GET: SavingsDeposits
        public ActionResult Index(string searchString, string sortOrder)
        {
            var savingsDeposits = from s in db.SavingsDeposits.Include(s => s.passbook).Include(s => s.user)
                                  select s;

            // Lọc theo PassBook ID nếu có
            if (!String.IsNullOrEmpty(searchString))
            {
                savingsDeposits = savingsDeposits.Where(s => s.passbook.SavingsBookID.ToString().Contains(searchString));
            }

            // Sắp xếp theo PassBook ID nếu cần
            ViewBag.PassBookSortParm = String.IsNullOrEmpty(sortOrder) ? "passbook_desc" : "";
            switch (sortOrder)
            {
                case "passbook_desc":
                    savingsDeposits = savingsDeposits.OrderByDescending(s => s.passbook.SavingsBookID);
                    break;
                default:
                    savingsDeposits = savingsDeposits.OrderBy(s => s.passbook.SavingsBookID);
                    break;
            }

            return View(savingsDeposits.ToList());
        }


        // GET: SavingsDeposits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavingsDeposit savingsDeposit = db.SavingsDeposits.Include(s => s.user).Where(s => s.DepositID == id).FirstOrDefault();
            if (savingsDeposit == null)
            {
                return HttpNotFound();
            }
            return View(savingsDeposit);
        }
        // GET:
        public ActionResult Create()
        {
            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID");
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name");
            return View(new SavingsDeposit());
        }

        [HttpGet]
        public JsonResult GetCustomerName(int savingsBookID)
        {
            var customerName = db.passbooks
                                .Where(p => p.SavingsBookID == savingsBookID)
                                .Select(p => p.user.user_name)
                                .FirstOrDefault();
            return Json(customerName, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetInterestRate(int savingsBookID)
        {
            var interestRate = db.passbooks
                                .Where(p => p.SavingsBookID == savingsBookID)
                                .Select(p => p.InterestRate)
                                .FirstOrDefault();
            return Json(interestRate, JsonRequestBehavior.AllowGet);
        }



        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepositID,SavingsBookID,user_id,DepositAmount,InterestRate,DepositDate")] SavingsDeposit savingsDeposit)
        {
            // Kiểm tra số tiền gởi thêm có hợp lệ không
            var additionalAmountValid = db.SavingsAccountTypes.Where(p => p.SavingsTypeID == passbook).Select(p => p.MinDepositAmount).FirstOrDefault() <= savingsDeposit.DepositAmount && db.SavingsAccountTypes.Where(p => p.InterestRate == savingsDeposit.InterestRate).Select(p => p.MaxDepositAmount).FirstOrDefault() >= savingsDeposit.DepositAmount;
            savingsDeposit.DepositAmount >= ;

            // Nạp lãi suất từ SavingsBookID
            var interestRate = db.passbooks
                                .Where(p => p.SavingsBookID == savingsDeposit.SavingsBookID)
                                .Select(p => p.InterestRate)
                                .FirstOrDefault();

            // Nạp `user_id` từ SavingsBookID
            var userId = db.passbooks
                         .Where(p => p.SavingsBookID == savingsDeposit.SavingsBookID)
                         .Select(p => p.user_id)
                         .FirstOrDefault();

            // Nếu không hợp lệ hoặc số tiền gởi thêm < 100000, thêm thông báo lỗi
            if (!additionalAmountValid || savingsDeposit.DepositAmount < 100000)
            {
                ModelState.AddModelError("", "Số tiền gởi thêm tối thiểu là 100.000đ và chỉ được gửi thêm khi đến kỳ hạn.");
            }

            if (ModelState.IsValid)
            {
                // Ghi lãi suất và `user_id` vào SavingsDeposit
                savingsDeposit.InterestRate = interestRate;
                savingsDeposit.user_id = userId;

                // Kiểm tra nếu đã tồn tại ID rồi
                if (db.SavingsDeposits.Any(d => d.DepositID == savingsDeposit.DepositID))
                {
                    ModelState.AddModelError("", "Deposit ID đã tồn tại. Vui lòng nhập ID khác.");
                    ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID", savingsDeposit.SavingsBookID);
                    ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", savingsDeposit.user_id);
                    ViewBag.InterestRate = new SelectList(db.SavingsAccountTypes, "InterestRate", "InterestRate", savingsDeposit.InterestRate);
                    return View(savingsDeposit);
                }

                db.SavingsDeposits.Add(savingsDeposit);
                db.SaveChanges();
                db.Entry(savingsDeposit).Reference(s => s.user).Load();

                return RedirectToAction("Index");
            }

            return View(savingsDeposit);
        }



        // GET: SavingsDeposits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavingsDeposit savingsDeposit = db.SavingsDeposits.Include(s => s.user).Where(s => s.DepositID == id).FirstOrDefault();
            if (savingsDeposit == null)
            {
                return HttpNotFound();
            }
            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID", savingsDeposit.SavingsBookID);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", savingsDeposit.user_id);
            ViewBag.InterestRate = new SelectList(db.SavingsAccountTypes, "InterestRate", "InterestRate", savingsDeposit.InterestRate);
            return View(savingsDeposit);
        }

        // POST: SavingsDeposits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepositID,SavingsBookID,user_id,DepositAmount,InterestRate,DepositDate")] SavingsDeposit savingsDeposit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(savingsDeposit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SavingsBookID = new SelectList(db.passbooks, "SavingsBookID", "SavingsBookID", savingsDeposit.SavingsBookID);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", savingsDeposit.user_id);
            ViewBag.InterestRate = new SelectList(db.SavingsAccountTypes, "InterestRate", "InterestRate", savingsDeposit.InterestRate);
            return View(savingsDeposit);
        }

        // GET: SavingsDeposits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavingsDeposit savingsDeposit = db.SavingsDeposits.Include(s => s.user).Where(s => s.DepositID == id).FirstOrDefault();
            if (savingsDeposit == null)
            {
                return HttpNotFound();
            }
            return View(savingsDeposit);
        }

        // POST: SavingsDeposits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SavingsDeposit savingsDeposit = db.SavingsDeposits.Find(id);
            db.SavingsDeposits.Remove(savingsDeposit);
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
