namespace CoinInvest.DtoModels
{
    public class CoinDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Metal { get; set; }
        public bool IsSold { get; set; }
        public decimal SoldFor { get; set; }
    }
}
