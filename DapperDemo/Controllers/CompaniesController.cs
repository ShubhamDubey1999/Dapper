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
using DapperDemo.Repository;

namespace Dapper.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompanyRepository _compRepo;
        private readonly IEmployeeRepository _EmpRepo;
        private readonly IBonusRepository _BonusRepo;

        public CompaniesController(ICompanyRepository compRepo , IBonusRepository BonusRepo , IEmployeeRepository EmpRepo)
        {
            _compRepo = compRepo;
            _EmpRepo = EmpRepo;
            _BonusRepo = BonusRepo;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(_compRepo.GetAll());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _compRepo.GetAll() == null)
            {
                return NotFound();
            }
            var company = _BonusRepo.GetCompanyWithEmployees(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (ModelState.IsValid)
            {
                _compRepo.Add(company);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var company = _compRepo.Find(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _compRepo.Update(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _compRepo.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }
    }
}
