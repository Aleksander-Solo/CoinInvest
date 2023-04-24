using CoinInvest.DataAccessLayer.Repositories.Interfaces;
using CoinInvest.DtoModels;
using System.Data.SqlClient;
using System.Data;
using CoinInvest.DataAccessLayer.Entity;
using Dapper;
using Microsoft.AspNetCore.Identity;
using CoinInvest.Exceptions;

namespace CoinInvest.DataAccessLayer.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly IConfiguration _configuration;


        public UserRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<User> GetUser(string name)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            User user = await connection.QuerySingleOrDefaultAsync<User>($"SELECT * FROM Users WHERE Name = '{name}';");
            if (user is null)
                throw new NotFoundException("User not found!");

            return user;
        }

        public async Task Register(User user)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await connection.ExecuteAsync($"Insert INTO Users([Name], [Password]) VALUES('{user.Name}', '{user.Password}');");
        }
    }
}
