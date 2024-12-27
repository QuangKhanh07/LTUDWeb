using SV21T1020037.DomainModels;

namespace SV21T1020037.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}
