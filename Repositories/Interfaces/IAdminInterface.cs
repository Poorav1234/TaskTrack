using Repositories.Models;

namespace Repositories;

public interface IAdminInterface
{
    Task<t_admin> Login(vm_Login login);
}
