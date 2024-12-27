using SV21T1020037.DomainModels;

namespace SV21T1020037.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
