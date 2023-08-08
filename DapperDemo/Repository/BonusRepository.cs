using Dapper;
using Dapper.Models;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DapperDemo.Repository
{
    public class BonusRepository : IBonusRepository
    {
        private IDbConnection db;
        public BonusRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void AddTestCompanyWithEmployees(Company objComp)
        {

            // We are using Transaction here  , if there any exception then rollback all changes.
            using (var transaction = new TransactionScope())
            {
                try
                {
                    objComp.CompanyId = db.Query<int>("INSERT INTO Companies(Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                + "SELECT CAST(SCOPE_IDENTITY() as int);", new
                {
                    objComp.Name,
                    objComp.Address,
                    objComp.City,
                    objComp.State,
                    objComp.PostalCode
                }).Single();

                    //One by one Insert
                    #region singleInsert

                    if (false)  // This method will never execute
                    {
                        foreach (var employee in objComp.Employees)
                        {
                            employee.CompanyId = objComp.CompanyId;
                            employee.EmployeeId = db.Query<int>("INSERT INTO Employees(Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                           + "SELECT CAST(SCOPE_IDENTITY() as int);", employee).Single();
                        }
                    }
                    #endregion

                    #region BulkInsert
                    //Bulk Insert
                    objComp.Employees.Select(c => { c.CompanyId = objComp.CompanyId; return c; }).ToList();
                    var sql = "INSERT INTO Employees(Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);" + "SELECT CAST(SCOPE_IDENTITY() as int);";
                    db.Execute(sql, objComp.Employees);
                    #endregion
                    transaction.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public List<Company> GetAllCompaniesWithEmployees()
        {
            var sql = "select  c.* , e.*  from Employees as e inner join Companies as c on e.CompanyId = c.CompanyId";
            var companyDic = new Dictionary<int, Company>();
            var company = db.Query<Company, Employee, Company>(sql, (c, e) =>
            {
                if (!companyDic.TryGetValue(c.CompanyId, out var currentCompany))
                {
                    currentCompany = c;
                    companyDic.Add(currentCompany.CompanyId, currentCompany);
                }
                currentCompany.Employees.Add(e);
                return currentCompany;
            }, splitOn: "EmployeeId");
            return company.Distinct().ToList();
        }

        public Company GetCompanyWithEmployees(int id)
        {
            var p = new
            {
                CompanyId = id,
            };
            var sql = "select * from companies where CompanyId = @CompanyId;" + "select * from employees where CompanyId = @CompanyId;";
            Company company;
            using (var list = db.QueryMultiple(sql, p))
            {
                company = list.Read<Company>().ToList().FirstOrDefault();
                company.Employees = list.Read<Employee>().ToList();
            }
            return company;
        }

        public List<Employee> GetEmployeesWithCompany(int id)
        {
            var sql = "select e.* , c.* from Employees as e inner join Companies as c on e.CompanyId = c.CompanyId";
            if (id != 0)
                sql += "where e.CompanyId = @Id";
            return db.Query<Employee, Company, Employee>(sql, (e, c) =>
            {
                e.Company = c;
                return e;
            }, new { id }, splitOn: "CompanyId").ToList();
        }

        public void RemoveRange(int[] CompanyId)
        {
            db.Query("Delete from Companies where CompanyId in @CompanyId", new { CompanyId });
        }

        List<Company> IBonusRepository.FilterCompanyByName(string CompanyName)
        {
            return db.Query<Company>("Select * from companies where Name like '%' +@CompanyName + '%'", new { CompanyName }).ToList();
        }
    }
}
