using CoinInvest.DataAccessLayer.Entity;
using CoinInvest.DataAccessLayer.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using CoinInvest.DtoModels;
using System.Globalization;

namespace CoinInvest.DataAccessLayer.Repositories
{
    public class CoinRepo : ICoinRepo
    {
        private readonly IConfiguration _configuration;

        public CoinRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddCoins(Coin coin)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            return await connection.QueryFirstAsync<int>($"Insert INTO Coins([Name], [Price], [Quantity], [MetalId], [IsSold], [SoldFor], [UserId]) VALUES('{coin.Name}', {coin.Price.ToString(CultureInfo.InvariantCulture.NumberFormat)},{coin.Quantity},{coin.MetalId},'{coin.IsSold}',{coin.SoldFor.ToString(CultureInfo.InvariantCulture.NumberFormat)},{coin.UserId}); SELECT SCOPE_IDENTITY();");
        }

        public async Task DeleteCoin(int coinId)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await connection.ExecuteAsync($"DELETE FROM Coins WHERE Id = {coinId}");
        }

        public async Task<Coin> GetCoin(int id)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            return await connection.QueryFirstOrDefaultAsync<Coin>($"SELECT [Coins].[Id], [Coins].[Name], [Coins].[Price], [Coins].[Quantity] ,[Coins].[UserId], [Metal].[Name] as [Metal] FROM Coins INNER JOIN Metal ON Coins.MetalId=Metal.Id WHERE [Coins].[Id] = {id};");
        }

        public async Task<IEnumerable<Coin>> GetCoins(int userId)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            return await connection.QueryAsync<Coin>($"SELECT [Coins].[Id], [Coins].[Name], [Coins].[Price], [Coins].[Quantity], [Metal].[Name] as [Metal] FROM Coins INNER JOIN Metal ON Coins.MetalId=Metal.Id WHERE [UserId] = {userId};");
        } 
    }
}
