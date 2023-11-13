using AutoMapper;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Models.DboModels;
using CSS_MagacinControl_App.ViewModels.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        public AppDbContext _dbContext;
        private IMapper _mapper;

        public AuthenticationRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }



        public async Task<(bool, bool)> AuthenticateToSytem(string username, string pass)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user != null) 
            {
                return (true, user.IsAdmin);
            }

            return (false, false);
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            var usersDbo = await _dbContext.Users.ToListAsync();

            var users = _mapper.Map<List<UserModel>>(usersDbo);

            return users;
        }
    }
}