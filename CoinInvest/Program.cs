using CoinInvest;
using CoinInvest.Auth;
using CoinInvest.DataAccessLayer;
using CoinInvest.DataAccessLayer.Entity;
using CoinInvest.DataAccessLayer.Repositories;
using CoinInvest.DataAccessLayer.Repositories.Interfaces;
using CoinInvest.DtoModels;
using CoinInvest.Middleware;
using CoinInvest.Services;
using CoinInvest.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var authSettings = new AuthSetting();

builder.Configuration.GetSection("Authentication").Bind(authSettings);
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = true;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = authSettings.JwtIssuer,
        ValidAudience = authSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey))
    };
});
builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(authSettings);
builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ICoinRepo, CoinRepo>();
builder.Services.AddTransient<IUserRepo, UserRepo>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ICoinService, CoinService>();
builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddTransient<IAuthorizationHandler, ResourceOperationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionMiddleware();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

//Api
app.MapPost("/login",async ([FromBody] UserDto user, IUserService service) => 
{
    return Results.Ok(await service.Login(user.Name, user.Password));
})
.WithName("Login")
.WithOpenApi();

app.MapPost("/register", async ([FromBody] UserDto user, IUserService service) =>
{
    await service.Register(user);
    return Results.Ok();
})
.WithName("Register")
.WithOpenApi();


app.MapPost("/coins", [Authorize] async ([FromBody] CreateCoinDto coin, ICoinService service) =>
{
    int id = await service.AddCoins(coin);
    return Results.Created($"/coins/{id}",await service.GetCoin(id));
})
.WithName("AddCoins")
.WithOpenApi();

app.MapGet("/coins/{id}", [Authorize] async (int id, ICoinService service) =>
{
    return Results.Ok(await service.GetCoin(id));
})
.WithName("GetCoin")
.WithOpenApi();

app.MapGet("/coins", [Authorize] async (ICoinService service, string? metal) =>
{
    return Results.Ok(await service.GetCoins(metal));
})
.WithName("GetCoins")
.WithOpenApi();

app.Run();