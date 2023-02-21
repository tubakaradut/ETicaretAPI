using ETicaretAPI.Domain.Entities.Identiy;
using X = ETicaretAPI.Application.DTOs;

namespace ETicaretAPI.Application.Abstractions.Token
{
    public interface ITokenHandler
    {
        X.Token CreateAccessToken(int minute, AppUser user);

        string CreateRefreshToken();
    }
}
