using SV21T1020037.DomainModels;

namespace SV21T1020037.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}
