using CoinInvest.DataAccessLayer.Entity;
using CoinInvest.DtoModels;

namespace CoinInvest.Services.Interfaces
{
    public interface ICoinService
    {
        public Task<CoinDtoData> GetCoins(string? metal);
        public Task<Coin> GetCoin(int id);
        public Task<int> AddCoins(CreateCoinDto coin);
        public Task DeleteCoin(int coinId);
    }
}
