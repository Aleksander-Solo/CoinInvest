using CoinInvest.Auth;
using CoinInvest.DataAccessLayer.Entity;
using CoinInvest.DataAccessLayer.Repositories.Interfaces;
using CoinInvest.DtoModels;
using CoinInvest.Exceptions;
using CoinInvest.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Xml.Linq;

namespace CoinInvest.Services
{
    public class CoinService : ICoinService
    {
        private readonly ICoinRepo _coinRepo;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CoinService(ICoinRepo coinRepo, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _coinRepo = coinRepo;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<int> AddCoins(CreateCoinDto coin)
        {
            string? userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Coin mappedCoin = new Coin
            {
                Name = coin.Name,
                Price = coin.Price,
                Quantity = coin.Quantity,
                MetalId = coin.MetalId,
                IsSold = coin.IsSold,
                SoldFor = coin.SoldFor,
                UserId = Int32.Parse(userId)
            };
            return await _coinRepo.AddCoins(mappedCoin);
        }

        public async Task DeleteCoin(int coinId)
        {
            var identity = _httpContextAccessor.HttpContext.User;
            var authResult = await _authorizationService.AuthorizeAsync(identity, _coinRepo.GetCoin(coinId), new ResourceOperationRequirement(ResourceOperation.Delete));
            if (!authResult.Succeeded)
                throw new ForbiddenException("Not enough privileges!");
            await _coinRepo.DeleteCoin(coinId);
        }

        public async Task<Coin> GetCoin(int id)
        {
            var identity = _httpContextAccessor.HttpContext.User;
            Coin coin = await _coinRepo.GetCoin(id);
            var authResult = await _authorizationService.AuthorizeAsync(identity, coin, new ResourceOperationRequirement(ResourceOperation.Read));
            if (!authResult.Succeeded)
                throw new ForbiddenException("Not enough privileges!");
            return await _coinRepo.GetCoin( id);
        }

        public async Task<CoinDtoData> GetCoins(string? metal)
        {
            string? userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            CoinDtoData coinDto = new CoinDtoData
            {
                Coins = Mapp(await _coinRepo.GetCoins(Int32.Parse(userId))),  
            };
            if (!String.IsNullOrWhiteSpace(metal))
            {
                coinDto.Price = await GetMetalPrice(metal);
            }
            return coinDto;
        }

        private async Task<float> GetMetalPrice(string? metal)
        {
            var options = new RestClientOptions("https://api.metalpriceapi.com/v1/latest?api_key=088294ec1953aa99217e7ff25439ccf7&base=PLN&currencies=XAG,XPD,XAU,XPT");
            RestClient client = new RestClient(options);
            RestRequest request = new RestRequest("", Method.Get);
            RestResponse response = await client.ExecuteAsync(request);
            var obj = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return (1 / (float)obj.rates[metal]);
        }
        private List<CoinDto> Mapp(IEnumerable<Coin> coins)
        {
            List<CoinDto> mappedCoins = new();
            foreach (Coin coin in coins)
            {
                mappedCoins.Add(new CoinDto
                {
                    Id = coin.Id,
                    Name = coin.Name,
                    Price = coin.Price,
                    Quantity = coin.Quantity,
                    Metal = coin.Metal,
                    IsSold = coin.IsSold,
                    SoldFor = coin.SoldFor
                });
            }
            return mappedCoins;
        }
    }
}
