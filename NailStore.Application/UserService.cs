using NailStore.Core.Interfaces;
using NailStore.Core.Models;

namespace NailStore.Application;

public class UserService : IUserService
{
    public Task<ResponseModelCore> ConfirmedEmailUser(UserConfirmitedEmail userConfirmited)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseModelCore> GetUserByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseModelCore> LoginUserAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseModelCore> RegisterUserAsync(string url, string userName, string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserNameIsFreeAsync(string username)
    {
        throw new NotImplementedException();
    }
}
