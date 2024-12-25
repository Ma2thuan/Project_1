using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Projekt_1.Model;

namespace Projekt_1.Controllers
{
    public class ReportsController : Controller
    {
        private Project1DBEntities db = new Project1DBEntities();

        // Định nghĩa lớp ReportData
        public class ReportData
        {
            public string SavingsType { get; set; }
            public decimal TotalIncome { get; set; }
            // Add other necessary properties if needed
        }

        // GET: Reports/DailyReport
        public ActionResult DailyReport(DateTime? reportDate)
        {
            if (reportDate.HasValue)
            {
                var reportData = db.SavingsDeposits
                    .Where(d => d.DepositDate == reportDate.Value)
                    .Join(db.passbooks, d => d.SavingsBookID, p => p.SavingsBookID, (d, p) => new { d.DepositAmount, p.SavingsType })
                    .GroupBy(dp => dp.SavingsType)
                    .Select(g => new ReportData
                    {
                        SavingsType = g.Key.ToString(),
                        TotalIncome = g.Sum(dp => dp.DepositAmount ?? 0)
                    })
                    .ToList();

                ViewBag.ReportData = reportData;
            }

            return View();
        }

        // GET: Reports/MonthlyReport
        public ActionResult MonthlyReport(DateTime? reportMonth)
        {
            if (reportMonth.HasValue)
            {
                var reportData = db.SavingsDeposits
                    .Where(d => d.DepositDate.Value.Year == reportMonth.Value.Year && d.DepositDate.Value.Month == reportMonth.Value.Month)
                    .Join(db.passbooks, d => d.SavingsBookID, p => p.SavingsBookID, (d, p) => new { d.DepositAmount, p.SavingsType })
                    .GroupBy(dp => dp.SavingsType)
                    .Select(g => new ReportData
                    {
                        SavingsType = g.Key.ToString(),
                        TotalIncome = g.Sum(dp => (decimal)(dp.DepositAmount ?? 0))
                    })
                    .ToList();

                ViewBag.ReportData = reportData;
            }

            return View();
        }
    }
}
