using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dapper.Data;
using Dapper.Models;
using Dapper.Repository;
using DapperDemo.Models;
using DapperDemo.Repository;

namespace Dapper.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ICompanyRepository _compRepo;
        private readonly IEmployeeRepository _EmpRepo;
        private readonly IBonusRepository _BonusRepo;
        [BindProperty]
        public Employee employee { get; set; }

        public EmployeesController(IEmployeeRepository EmpRepo , ICompanyRepository compRepo , IBonusRepository BonusRepo)
        {
            _compRepo = compRepo;
            _EmpRepo = EmpRepo;
            _BonusRepo = BonusRepo;
        }

        // GET: Companies
        public async Task<IActionResult> Index(int companyId = 0)
        {
            //List<Employee> employees = _EmpRepo.GetAll();
            //foreach(Employee obj in employees)
            //{
            //    obj.Company = _compRepo.Find(obj.CompanyId);
            //}
            List<Employee> employees = _BonusRepo.GetEmployeesWithCompany(companyId);
            return View(employees);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> companyList = _compRepo.GetAll().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.CompanyId.ToString(),
            });
            ViewBag.CompanyList = companyList;
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST()
        {
            if (ModelState.IsValid)
            {
                _EmpRepo.Add(employee);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            employee = _EmpRepo.Find(id.GetValueOrDefault());
            IEnumerable<SelectListItem> companyList = _compRepo.GetAll().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.CompanyId.ToString(),
            });
            ViewBag.CompanyList = companyList;
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                _EmpRepo.Update(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _EmpRepo.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }
    }
}
