using Dapper;
using SV21T1020037.DomainModels;

namespace SV21T1020037.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select  EmployeeID as UserId,
		                            Email as UserName,
		                            FullName as DisplayName,
		                            Photo,
		                            RoleNames
                            from Employees where Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        public bool ChangePassword(string username, string newpassword)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Employees
                    SET Password = @NewPassword
                    WHERE Email = @UserName";
                var parameters = new
                {
                    UserName = username,
                    NewPassword = newpassword,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

    }
}
