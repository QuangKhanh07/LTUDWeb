using SV21T1020037.DomainModels;
using SV21T1020037.Web.Models;

namespace SV21T1020037.Web.Models
{
    /// <summary>
    /// Lớp chứa thông tin đầu vào cho chức năng tìm kiếm và phân trang sản phẩm
    /// </summary>
    public class ProductSearchInput : PaginationSearchInput
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
    }

}

