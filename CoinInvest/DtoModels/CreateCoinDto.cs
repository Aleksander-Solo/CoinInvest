namespace CoinInvest.DtoModels
{
    public class CreateCoinDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int MetalId { get; set; }
        public bool IsSold { get; set; }
        public decimal SoldFor { get; set; }
    }
}
