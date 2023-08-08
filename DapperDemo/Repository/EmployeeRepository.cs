using Dapper.Data;
using Dapper.Models;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Dapper.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {

        private IDbConnection db;
        public EmployeeRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public Employee Add(Employee employee)
        {
            //employee.CompanyId = db.Query<int>("INSERT INTO Companies(Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
            //+ "SELECT CAST(SCOPE_IDENTITY() as int);", new
            //{
            //    employee.Name,
            //    employee.Address,
            //    employee.City,
            //    employee.State,
            //    employee.PostalCode
            //}).Single();
            employee.EmployeeId = db.Query<int>("INSERT INTO Employees(Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                + "SELECT CAST(SCOPE_IDENTITY() as int);", employee).Single();
            return employee;

        }

        public Employee Find(int id)
        {
            return db.Query<Employee>("SELECT * FROM Employees WHERE EmployeeId = @Id", new { id }).Single();
        }

        public List<Employee> GetAll()
        {
            return db.Query<Employee>("select * from employees").ToList();
        }

        public void Remove(int id)
        {
            db.Execute("delete from employees where employeeId = @Id", new { id });
        }

        public Employee Update(Employee employee)
        {
            db.Execute("UPDATE Employees SET Name = @Name, Title = @Title, Email = @Email, Phone = @Phone, CompanyId = @CompanyId WHERE EmployeeId = @EmployeeId", employee);
            return employee;
        }
    }
}
