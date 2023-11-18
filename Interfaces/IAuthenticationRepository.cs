using CSS_MagacinControl_App.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Interfaces
{
    public interface IAuthenticationRepository
    {
        // Return value of this function is Tuple<bool, bool>
        // First bool value indicates if authentication succeeded.
        // Second bool value indicated if authenticated user is admin.
        Task<(bool, bool)> AuthenticateToSytemAsync(string username, string pass);

        Task<List<UserModel>> GetUsersAsync();
        Task<bool> ValidateNewUserAsync(UserModel userModel, string repeatPassword);
        Task AddNewUserAsync(UserModel userModel);
        Task<UserModel> FindChangedUserAsync(List<UserModel> newState);
        Task SaveUserChangesAsync(UserModel userModel);
        Task ChangeUserPasswordAsync(Guid userIdentifier, string newPassword);
        string EncryptPassword(string plainTextPassword);
    }
}