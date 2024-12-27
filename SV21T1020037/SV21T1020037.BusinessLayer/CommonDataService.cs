using SV21T1020037.BusinessLayer;
using SV21T1020037.DataLayers;
using SV21T1020037.DataLayers.SQLServer;
using SV21T1020037.DomainModels;

namespace SV21T1020037.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ISimpleQueryDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ISimpleQueryDAL<OrderStatus> orderstatusDB;
        private static readonly IOrderDAL orderDB;
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;



        /// <summary>
        /// Ctor
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;
            provinceDB = new ProvinceDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
            orderstatusDB = new OrderStatusDAL(connectionString);
            orderDB = new OrderDAL(connectionString);
            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }

        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List();
        }

        public static List<Shipper> ListOfShippers()
        {
            return shipperDB.List();
        }

        public static List<OrderStatus> ListOfOrderStatus()
        {
            return orderstatusDB.List();
        }
        public static List<Category> ListAllCategory()
        {
            return categoryDB.List();
        }

        public static List<Customer> ListAllCustomer()
        {
            return customerDB.List();
        }

        public static List<Order> ListAllOrder()
        {
            return orderDB.List();
        }

        public static List<Supplier> ListAllSupplier()
        {
            return supplierDB.List();
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi dòng</param>
        /// <param name="searchValue">Tên khách hàng hoặc tên giao dịch cần tìm</param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);

        }
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }

        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        public static bool DeleteCustomer(int id)
        {
            if (customerDB.InUsed(id))
                return false;
            return customerDB.Delete(id);
        }
        public static bool InUserdCustomer(int id)
        {
            return customerDB.InUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách shipper dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue);
        }
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }

        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        public static bool DeleteShipper(int id)
        {
            if (shipperDB.InUsed(id))
                return false;
            return shipperDB.Delete(id);
        }
        public static bool InUserdShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách nha cung cap dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue);
        }
        public static Supplier? GetSupperlier(int id)
        {
            return supplierDB.Get(id);
        }
        public static int AddSupperlier(Supplier data)
        {
            return supplierDB.Add(data);
        }

        public static bool UpdateSupperlier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        public static bool DeleteSupperlier(int id)
        {
            if (supplierDB.InUsed(id))
                return false;
            return supplierDB.Delete(id);
        }
        public static bool InUserdSupperlier(int id)
        {
            return supplierDB.InUsed(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = (employeeDB.Count(searchValue));
            return employeeDB.List(page, pageSize, searchValue);
        }
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }

        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.InUsed(id))
                return false;
            return employeeDB.Delete(id);
        }
        public static bool InUserdEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = (categoryDB.Count(searchValue));
            return categoryDB.List(page, pageSize, searchValue);
        }

        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }

        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        public static bool DeleteCategory(int id)
        {
            if (categoryDB.InUsed(id))
                return false;
            return categoryDB.Delete(id);
        }
        public static bool InUserdCategory(int id)
        {
            return categoryDB.InUsed(id);
        }

    }
}
