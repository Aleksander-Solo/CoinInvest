using CoinInvest.DataAccessLayer.Entity;

namespace CoinInvest.DtoModels
{
    public class CoinDtoData
    {
        public List<CoinDto> Coins { get; set; }
        public float Price { get; set; }
    }
}
