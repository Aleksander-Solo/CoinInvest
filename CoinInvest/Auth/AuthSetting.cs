namespace CoinInvest.Auth
{
    public class AuthSetting
    {
        public string JwtKey { get; set; }
        public int JwtExpireDay { get; set; }
        public string JwtIssuer { get; set; }
    }
}
