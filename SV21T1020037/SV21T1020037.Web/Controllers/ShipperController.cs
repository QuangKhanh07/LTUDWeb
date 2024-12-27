using Microsoft.AspNetCore.Mvc;
using SV21T1020037.BusinessLayer;
using SV21T1020037.BusinessLayers;
using SV21T1020037.DomainModels;
using SV21T1020037.Web.AppCodes;
using SV21T1020037.Web.Models;
using Microsoft.AspNetCore.Authorization;
namespace SV21T1020037.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}, {WebUserRoles.EMPLOYEE}")]


    public class ShipperController : Controller
    {
        public const int PAGE_SIZE = 10;
        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";


        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }

        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfShippers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, model);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Shipper mới";
            var data = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin Shipper";
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung Shipper mới" : "Cập nhật thông tin Shipper";

            //Kiểm tra dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu vào ModelState
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên Shipper không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            try
            {
                if (data.ShipperID == 0)
                {
                    //Add
                    int id = CommonDataService.AddShipper(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Phone bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    //Update
                    bool result = CommonDataService.UpdateShipper(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Phone bị trùng");
                        return View("Edit", data);
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống bị lỗi");
                return View("Edit", data);
            }

        }

        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
