using Dapper.Data;
using Dapper.Models;

namespace Dapper.Repository
{
    public class CompanyRepositoryEF : ICompanyRepository
    {
        private readonly ApplicationDBContext _db;

        public CompanyRepositoryEF(ApplicationDBContext db)
        {
            _db = db;
        }
        public Company Add(Company company)
        {
            _db.Companies.Add(company);
            _db.SaveChanges();
            return company;
        }

        public Company Find(int id)
        {
            return _db.Companies.FirstOrDefault(u => u.CompanyId == id);
        }

        public List<Company> GetAll()
        {
            return _db.Companies.ToList();
        }

        public void Remove(int id)
        {
            Company company= _db.Companies.FirstOrDefault(u=>u.CompanyId == id);
            _db.Companies.Remove(company);
            _db.SaveChanges();
            return;
        }

        public Company Update(Company company)
        {
            _db.Update(company);
            _db.SaveChanges();
            return company;
        }
    }
}
