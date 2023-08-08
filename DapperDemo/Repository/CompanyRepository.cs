using Dapper.Data;
using Dapper.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Dapper.Repository
{
    public class CompanyRepository : ICompanyRepository
    {

        private IDbConnection db;
        public CompanyRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public Company Add(Company company)
        {
            try
            {
                company.CompanyId = db.Query<int>("INSERT INTO Companies(Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                + "SELECT CAST(SCOPE_IDENTITY() as int);", new
                {
                    company.Name,
                    company.Address,
                    company.City,
                    company.State,
                    company.PostalCode
                }).Single();
                // company.CompanyId = db.Query<int>("INSERT INTO Companies(Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                //+ "SELECT CAST(SCOPE_IDENTITY() as int);", new
                //{ company }).Single();
                return company;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Company Find(int id)
        {
            return db.Query<Company>("select * from companies where CompanyId = @CompanyId", new { @CompanyId = id }).Single();
        }

        public List<Company> GetAll()
        {
            return db.Query<Company>("select * from companies").ToList();
        }

        public void Remove(int id)
        {
            db.Execute("delete from companies where companyId = @Id", new { id });
        }

        public Company Update(Company company)
        {
            db.Execute("UPDATE Companies SET Name = @Name, Address = @Address, City = @City, State = @State, PostalCode = @PostalCode WHERE CompanyId = @CompanyId",company);
            return company;
        }
    }
}
