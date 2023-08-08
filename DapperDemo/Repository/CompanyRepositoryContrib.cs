using Dapper.Contrib.Extensions;
using Dapper.Data;
using Dapper.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Dapper.Repository
{
    public class CompanyRepositoryContrib : ICompanyRepository
    {

        private IDbConnection db;
        public CompanyRepositoryContrib(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public Company Add(Company company)
        {
            try
            {
                var id = db.Insert<Company>(company);
                company.CompanyId = (int)id;
                return company;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Company Find(int id)
        {
            return db.Get<Company>(id);
        }

        public List<Company> GetAll()
        {
            return db.GetAll<Company>().ToList();
        }

        public void Remove(int id)
        {
            db.Delete(new Company { CompanyId = id });
        }

        public Company Update(Company company)
        {

            db.Update<Company>(company);
            return company;
        }
    }
}
