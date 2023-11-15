using AutoMapper;
using CSS_MagacinControl_App.Dialog;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Models.DboModels;
using CSS_MagacinControl_App.ViewModels.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        public AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DialogHandler dialogHandler;

        public AuthenticationRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            dialogHandler = new DialogHandler();
        }

        public async Task AddNewUserAsync(UserModel userModel)
        {
            var userDbo = _mapper.Map<UserDbo>(userModel);

            _dbContext.Users.Add(userDbo);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(bool, bool)> AuthenticateToSytemAsync(string username, string pass)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user != null) 
            {
                return (true, user.IsAdmin);
            }

            return (false, false);
        }

        public async Task<UserModel> FindChangedUserAsync(List<UserModel> newState)
        {
            var users = await _dbContext.Users.ToListAsync();

            for (int i = 0; i < newState.Count; i++)
            {
                if (users[i].Username != newState[i].Username ||
                    users[i].Name != newState[i].Name ||
                    users[i].Surname != newState[i].Surname ||
                    users[i].IsAdmin != newState[i].IsAdmin)
                {
                    return _mapper.Map<UserModel>(newState[i]);
                }
            }

            return null;
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            var usersDbo = await _dbContext.Users.ToListAsync();

            var users = _mapper.Map<List<UserModel>>(usersDbo);

            return users;
        }

        public async Task SaveUserChangesAsync(UserModel userModel)
        {
            var userDbo = _mapper.Map<UserDbo>(userModel);

            _dbContext.ChangeTracker.Clear();
            _dbContext.Entry<UserDbo>(userDbo).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ValidateNewUserAsync(UserModel userModel, string repeatPassword)
        {
            if (string.IsNullOrEmpty(userModel.Username) || string.IsNullOrEmpty(userModel.Name) || string.IsNullOrEmpty(userModel.Surname) || 
                string.IsNullOrEmpty(userModel.Password) || string.IsNullOrEmpty(repeatPassword))
            {
                dialogHandler.GetNotAllFieldsFilled();
                return false;
            }

            if (userModel.Password != repeatPassword)
            {
                dialogHandler.GetRazliciteSifreDialog();
                return false;
            }

            if (await _dbContext.Users.Where(x => x.Username == userModel.Username).AnyAsync())
            {
                dialogHandler.GetUsernameAlreadyTakenDialog();
                return false;
            }

            return true;
        }
    }
}