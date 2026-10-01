using Repositories.Models;

namespace Repositories;

public interface IUserInterface
{
    Task<int> Register(t_user user);
    Task<t_user> Login(vm_Login login);
    Task<t_user> GetUser(int user_id);
    Task<int> UpdateProfile(t_user user);
    Task<int> ChangePassword(int user_id, vm_ChangePassword password);
    Task<List<t_user>> GetUsers();

    Task<int> GetTotalUsers();

    Task<List<t_user>> SearchUsers(
    string username,
    string email,
    string mobile,
    string gender,
    string city);
}
