using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020037.BusinessLayers;
using SV21T1020037.DomainModels;
using SV21T1020037.Web.AppCodes;
using SV21T1020037.Web.Models;
namespace SV21T1020037.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}, {WebUserRoles.EMPLOYEE}")]


    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";

        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };
            }
            return View(condition);
        }

        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(
                out rowCount,
                condition.Page,
                condition.PageSize,
                condition.SearchValue ?? "",
                condition.CategoryID,
                condition.SupplierID,
                condition.MinPrice,
                condition.MaxPrice
            );

            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var data = new Product()
            {
                ProductID = 0,
                IsSelling = true,
                Photo = "nophoto.png"
            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Product data, IFormFile? _Photo)
        {

            //Xử lí ảnh
            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật mặt hàng";

            // Validate input data
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (data.SupplierID == 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Tên mặt hàng không được để trống");
            if (data.Price == 0)
                ModelState.AddModelError(nameof(data.Price), "Giá tiền không được để trống");
            if (string.IsNullOrWhiteSpace(data.ProductDescription))
                ModelState.AddModelError(nameof(data.ProductDescription), "Mô tả không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }
            int id = 0;
            try
            {

                if (data.ProductID == 0)
                {
                    // Add
                    id = ProductDataService.AddProduct(data);

                    if (id < 0)
                    {
                        ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng đã tồn tại");
                        return View("Edit", data);
                    }
                }
                else
                {
                    // Update
                    bool result = ProductDataService.UpdateProduct(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng đã tồn tại");
                        return View("Edit", data);
                    }
                }

                return RedirectToAction("Edit", new { id = id });
            }
            catch
            {
                ModelState.AddModelError("Error", "Lỗi hệ thống.");
                return View("Edit", data);
            }
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }

            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        public IActionResult Attribute(int productId)
        {
            var attributes = ProductDataService.ListAttributes(productId);
            ViewBag.ProductID = productId;
            return View(attributes);
        }

        public IActionResult Photo(int productId)
        {
            var photos = ProductDataService.ListPhotos(productId);
            ViewBag.ProductID = productId;
            return View(photos);
        }

        public IActionResult EditPhoto(int ProductID, int PhotoID)
        {
            ViewBag.Title = "Cập nhật ảnh mặt hàng";
            var data = ProductDataService.GetPhoto(ProductID, PhotoID);
            if (data == null)
                return RedirectToAction("Index");
            return View("Photo", data);
        }


        public IActionResult CreatePhoto(int ProductID)
        {
            ViewBag.Title = "Bổ sung ảnh mặt hàng";
            var data = new ProductPhoto()
            {
                PhotoID = 0,
                IsHidden = true,
                ProductID = ProductID // Gán ProductID ở đây
            };
            return View("Photo", data);
        }


        [HttpPost]
        public IActionResult UploadPhoto(ProductPhoto data, IFormFile? _Photo)
        {
            ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh mới" : "Cập nhật thông tin ảnh mới";

            // Kiểm tra xem có ảnh được tải lên không
            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\productPhoto", fileName);

                try
                {
                    // Lưu ảnh vào thư mục
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        _Photo.CopyTo(stream);
                    }
                    data.Photo = fileName; // Lưu tên ảnh vào thuộc tính của đối tượng
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", "Lưu ảnh thất bại");
                    return View("Photo", data); // Quay lại view và hiển thị lỗi nếu lưu ảnh thất bại
                }
            }


            // Thêm hoặc cập nhật ảnh
            try
            {
                if (data.PhotoID == 0)
                {
                    // Thêm ảnh mới
                    long id = ProductDataService.AddPhoto(data);
                    if (id < 0)
                    {
                        ModelState.AddModelError("Error", "Không thể thêm ảnh sản phẩm.");
                        return View("Photo", data);
                    }
                }
                else
                {
                    // Cập nhật ảnh
                    bool result = ProductDataService.UpdatePhoto(data);
                    if (!result)
                    {
                        ModelState.AddModelError("Error", "Không thể cập nhật ảnh sản phẩm.");
                        return View("Photo", data);
                    }
                }

                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Vui lòng điền đầy đủ thông tin của mặt hàng trên");
                return View("Photo", data); // Nếu có lỗi hệ thống, quay lại view và hiển thị thông báo lỗi
            }
        }

        public IActionResult DeletePhoto(int ProductID, int PhotoID)
        {
            ProductDataService.DeletePhoto(PhotoID);
            return RedirectToAction("Edit", new { id = ProductID });
        }



        public IActionResult DeleteAttribute(int ProductID, int AttributeID)
        {
            ProductDataService.DeleteAttribute(AttributeID);
            return RedirectToAction("Edit", new { id = ProductID });
        }
        public IActionResult EditAttribute(int ProductID, int AttributeID)
        {
            ViewBag.Title = "Cập nhật thuộc tính";
            var data = ProductDataService.ListAttribute(ProductID, AttributeID);
            if (data == null)
                return RedirectToAction("Index");
            return View("Attribute", data);
        }


        public IActionResult CreateAttribute(int ProductID)
        {
            ViewBag.Title = "Bổ sung thuộc tính";
            var data = new ProductAttribute()
            {
                AttributeID = 0,
                ProductID = ProductID
            };

            return View("Attribute", data);
        }


        [HttpPost]
        public IActionResult UploadAttribute(ProductAttribute data)
        {
            ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính mới" : "Cập nhật thông tin thuộc tính";

            // Thêm hoặc cập nhật ảnh
            try
            {
                if (data.AttributeID == 0)
                {
                    // Thêm ảnh mới
                    long id = ProductDataService.AddAttribute(data);
                    if (id < 0)
                    {
                        ModelState.AddModelError("Error", "Không thể thêm thuộc tính sản phẩm.");
                        return View("Attribute", data);
                    }
                }
                else
                {
                    // Cập nhật
                    bool result = ProductDataService.UpdateAttribute(data);
                    if (!result)
                    {
                        ModelState.AddModelError("Error", "Không thể cập nhật thuộc tính sản phẩm.");
                        return View("Attribute", data);
                    }
                }
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Vui lòng điền đầy đủ thông tin thuộc tính");
                return View("Attribute", data); // Nếu có lỗi hệ thống, quay lại view và hiển thị thông báo lỗi
            }
        }


    }
}
