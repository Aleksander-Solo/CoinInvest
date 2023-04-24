using CoinInvest.DataAccessLayer.Entity;
using CoinInvest.DtoModels;

namespace CoinInvest.DataAccessLayer.Repositories.Interfaces
{
    public interface ICoinRepo
    {
        public Task<IEnumerable<Coin>> GetCoins(int userId);
        public Task<Coin> GetCoin(int id);
        public Task<int> AddCoins(Coin coin);
        public Task DeleteCoin(int coinId);
    }
}
