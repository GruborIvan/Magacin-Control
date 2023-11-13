using CSS_MagacinControl_App.ViewModels.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Interfaces
{
    public interface IAuthenticationRepository
    {
        // Return value of this function is Tuple<bool, bool>
        // First bool value indicates if authentication succeeded.
        // Second bool value indicated if authenticated user is admin.
        Task<(bool, bool)> AuthenticateToSytem(string username, string pass);

        Task<List<UserModel>> GetUsersAsync(); 
    }
}