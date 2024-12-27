using SV21T1020037.BusinessLayer;
using SV21T1020037.DataLayers;
using SV21T1020037.DataLayers.SQLServer;
using SV21T1020037.DomainModels;
namespace SV21T1020037.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;

        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;

            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }
        public static UserAccount? Authorize(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
                return employeeAccountDB.Authorize(username, password);
            else
                return customerAccountDB.Authorize(username, password);
        }
        public static bool ChangedPassword(string username, string newpassword)
        {
            return employeeAccountDB.ChangePassword(username, newpassword);
        }
    }
    public enum UserTypes
    {
        Employee,
        Customer
    }



}
