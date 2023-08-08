using Dapper.Models;
using DapperDemo.Models;

namespace DapperDemo.Repository
{
    public interface IBonusRepository
    {
        List<Employee> GetEmployeesWithCompany(int id);
        Company GetCompanyWithEmployees(int id);
        List<Company> GetAllCompaniesWithEmployees();
        void AddTestCompanyWithEmployees(Company objComp);
        void RemoveRange(int[] CompanyId);
        List<Company> FilterCompanyByName(string CompanyName);
    }
}
