using EmployeeLeaveManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Admin") == null)
            {
                return RedirectToAction("Login");
            }
            var leaves = _context.LeaveRequests.ToList();

            return View(leaves);
        }
        public IActionResult Approve(int id)
        {
            var leave = _context.LeaveRequests.Find(id);

            if (leave == null)
                return RedirectToAction("Dashboard");

            // Prevent approving twice
            if (leave.Status != "Pending")
                return RedirectToAction("Dashboard");

            leave.Status = "Approved";

            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }
        public IActionResult Reject(int id)
        {
            var leave = _context.LeaveRequests.Find(id);

            if (leave == null)
                return RedirectToAction("Dashboard");

            // Prevent rejecting twice
            if (leave.Status != "Pending")
                return RedirectToAction("Dashboard");

            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == leave.EmployeeEmail);

            if (employee != null)
            {
                employee.AvailableLeaves++;
            }

            leave.Status = "Rejected";

            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (email == "admin@company.com" &&
                password == "admin123")
            {
                HttpContext.Session.SetString("Admin", "true");

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Admin Credentials";

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}