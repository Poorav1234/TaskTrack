using Repositories.Models;

namespace Repositories;

public interface IUserInterface
{
    Task<int> Register(t_user user);
    Task<t_user> Login(vm_Login login);
    Task<t_user> GetUser(string user_id);
}
