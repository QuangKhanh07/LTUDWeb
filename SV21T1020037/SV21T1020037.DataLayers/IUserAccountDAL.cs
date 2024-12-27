using SV21T1020037.DomainModels;

namespace SV21T1020037.DataLayers
{
    public interface IUserAccountDAL
    {
        UserAccount? Authorize(string username, string password);
        bool ChangePassword(string username, string newpassword);

    }
}
