using CoinInvest.Auth;
using CoinInvest.DataAccessLayer.Entity;
using CoinInvest.DataAccessLayer.Repositories.Interfaces;
using CoinInvest.DtoModels;
using CoinInvest.Exceptions;
using CoinInvest.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoinInvest.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthSetting _authSetting;
        public UserService(IUserRepo userRepo, IPasswordHasher<User> passwordHasher, AuthSetting authSetting)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _authSetting = authSetting;
        }
        public async Task<string> Login(string username, string password)
        {
            User user = await _userRepo.GetUser(username);
            if (user is null)
                throw new BadRequestException("Invalid username or password!");

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

            if(result is PasswordVerificationResult.Failed)
                throw new BadRequestException("Invalid username or password!");

            return GenerateJwt(user);
        }
        private string GenerateJwt(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSetting.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authSetting.JwtExpireDay);

            JwtSecurityToken token = new JwtSecurityToken(_authSetting.JwtIssuer,
                _authSetting.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public async Task Register(UserDto user)
        {
            User mappedUser = new User { Name = user.Name, Password = user.Password };
            mappedUser.Password = _passwordHasher.HashPassword(mappedUser, mappedUser.Password);
            await _userRepo.Register(mappedUser);
        }
    }
}
