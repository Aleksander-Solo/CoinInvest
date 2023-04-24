using CoinInvest.DataAccessLayer.Entity;
using CoinInvest.DtoModels;

namespace CoinInvest.DataAccessLayer.Repositories.Interfaces
{
    public interface IUserRepo
    {
        public Task Register(User user);
        public Task<User> GetUser(string name);
    }
}
