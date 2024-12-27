using SV21T1020037.DomainModels;

namespace SV21T1020037.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}
