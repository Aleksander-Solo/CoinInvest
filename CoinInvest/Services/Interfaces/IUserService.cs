using CoinInvest.DtoModels;

namespace CoinInvest.Services.Interfaces
{
    public interface IUserService
    {
        public Task<string> Login(string username, string password);
        public Task Register(UserDto user);
    }
}
