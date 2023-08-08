using Dapper.Data;
using Dapper.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Dapper.Repository
{
    public class CompanyRepositorySP : ICompanyRepository
    {

        private IDbConnection db;
        public CompanyRepositorySP(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public Company Add(Company company)
        {
            try
            {
                var param = new DynamicParameters();  // DyanamicParameters exists in DapperClass.
                param.Add("@CompanyId" , company.CompanyId , DbType.Int32 , direction:ParameterDirection.Output);
                param.Add("@Name",company.Name);
                param.Add("@Address",company.Address);
                param.Add("@City",company.City);
                param.Add("@State",company.State);
                param.Add("@PostalCode",company.PostalCode);
                this.db.Execute("usp_AddCompany", param, commandType: CommandType.StoredProcedure);
                company.CompanyId = param.Get<int>("CompanyId");
                return company;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Company Find(int id)
        {
            return db.Query<Company>("usp_GetCompany", new { @CompanyId = id }, commandType: CommandType.StoredProcedure).Single();
        }

        public List<Company> GetAll()
        {
            return db.Query<Company>("usp_GetALLCompany", commandType: CommandType.StoredProcedure).ToList();
        }

        public void Remove(int id)
        {
            db.Execute("usp_RemoveCompany", new { @CompanyId = id } , commandType: CommandType.StoredProcedure);
        }

        public Company Update(Company company)
        {
            var param = new DynamicParameters();  // DyanamicParameters exists in DapperClass.
            param.Add("@CompanyId", company.CompanyId, DbType.Int32);
            param.Add("@Name", company.Name);
            param.Add("@Address", company.Address);
            param.Add("@City", company.City);
            param.Add("@State", company.State);
            param.Add("@PostalCode", company.PostalCode);
            this.db.Execute("usp_UpdateCompany", param, commandType: CommandType.StoredProcedure);
            company.CompanyId = param.Get<int>("CompanyId");
            return company;
        }
    }
}
