using Microsoft.AspNetCore.Mvc;
using SV21T1020037.BusinessLayers;
using SV21T1020037.DomainModels;
using SV21T1020037.Web.AppCodes;
using SV21T1020037.Web.Models;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
namespace SV21T1020037.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}, {WebUserRoles.EMPLOYEE}")]

    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";


        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";
        private const string SHOPPING_CART = "ShoppingCart";


        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
                };
            }
            return View(condition);
        }

        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
            var model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View(model);
        }

        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }

        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }

        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }

        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);


            return Json("");


        }

        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }

        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống. vui lòng chọn mặt hàng cần bán");

            if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");

            var userData = User.GetUserData();
            int employeeID = int.Parse(userData.UserId);


            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }

        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var data = OrderDataService.GetOrderDetail(id, productId);
            if (data == null)
                return RedirectToAction("Details", new { id = id });
            return View(data);
        }
        public IActionResult UpdateDetail(OrderDetail data, int quantity = 0, decimal salePrice = 0)
        {
            if (data.OrderID != 0 && data.ProductID != 0)
            {
                bool result = OrderDataService.SaveOrderDetail(data.OrderID, data.ProductID, quantity, salePrice);
                if (result == false)
                    return RedirectToAction("Details", new { id = data.OrderID });
            }
            return RedirectToAction("Details", new { id = data.OrderID });
        }
        public IActionResult DeleteDetail(int id = 0, int productId = 0)
        {
            OrderDataService.DeleteOrderDetail(id, productId);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Delete(int id = 0)
        {
            var data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            OrderDataService.DeleteOrder(id);

            return RedirectToAction("Index");

        }
        public IActionResult Accept(int id = 0)
        {
            OrderDataService.AcceptOrder(id);
            return RedirectToAction("Details", new { id = id });
        }

        //public IActionResult Shipping(int id = 0, int ShipperID = 0)
        //{
        //    if (Request.Method == "GET")
        //    {
        //        var order = OrderDataService.GetOrder(id);
        //        if (order == null)
        //            return RedirectToAction("Index");
        //        var details = OrderDataService.ListOrderDetails(id);
        //        var model = new OrderDetailModel()
        //        {
        //            Order = order,
        //            Details = details
        //        };
        //        return View("Shipping", model);
        //    }
        //    int shipperId = int.Parse(Request.Form["ShipperID"]);
        //    if (shipperId <= 0)
        //    {
        //        return BadRequest("Vui lòng chọn người giao hàng.");
        //    }

        //    try
        //    {
        //        // Gọi phương thức cập nhật thông tin giao hàng
        //        OrderDataService.ShipOrder(id, shipperId);
        //        return RedirectToAction("Details", new { id = id });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Lỗi: {ex.Message}");
        //        return StatusCode(500, "Không thể xử lý yêu cầu.");
        //    }
        //}


        public IActionResult Shipping(int id = 0)
        {
            if (id <= 0)
            {
                return RedirectToAction("Index");
            }

            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Shipping(int id, int shipperID)
        {
            if (id <= 0 || shipperID <= 0)
            {
                return RedirectToAction("Details", new { id = id });
            }
            OrderDataService.ShipOrder(id, shipperID);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Cancel(int id = 0)
        {
            OrderDataService.CancelOrder(id);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Finish(int id = 0)
        {
            OrderDataService.FinishOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Reject(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                // Gọi dịch vụ từ chối đơn hàng
                OrderDataService.RejectOrder(id);

                // Điều hướng về trang chi tiết đơn hàng hoặc danh sách
                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi từ chối đơn hàng: {ex.Message}");
                return StatusCode(500, "Không thể xử lý yêu cầu từ chối đơn hàng.");
            }
        }

    }
}
