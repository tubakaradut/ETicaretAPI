using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Domain.Entities.Identiy;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ETicaretAPI.Infrastructure.Services.Token
{
    public class TokenHandler : ITokenHandler
    {
        readonly IConfiguration _configuration;

        public TokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Application.DTOs.Token CreateAccessToken(int minute, AppUser user)
        {
            //token oluşturma işlemleri
            Application.DTOs.Token token = new();

            //Security Keyin simetriğini alıyoruz.
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]));

            //şifrelenmiş kimliği oluşturyoruz
            SigningCredentials signingCredentials = new(securityKey,SecurityAlgorithms.HmacSha256);

            //oluşturulucak token ayarlarını veriyoruz.
            token.Expiration=DateTime.UtcNow.AddMinutes(minute);

            JwtSecurityToken securityToken = new(
                audience: _configuration["Token:Audince"],
                issuer: _configuration["Token:Issuer"],
                expires: token.Expiration,
                notBefore: DateTime.UtcNow , //token ne zaman devreye girsin demek
                signingCredentials:signingCredentials,
                claims:new List<Claim> { new(ClaimTypes.Name,user.UserName)}
                );


            //token oluşturucu sınıftan bir örnek alınır.
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken=tokenHandler.WriteToken(securityToken);

            //access token oluşturulurken aynı anda refresh token oluşturmak için methodumuz burada çağrılır ve token classında yazdığımız properte aktarılır.
            token.RefreshToken=CreateRefreshToken();

            return token;
        }

        public string CreateRefreshToken()
        {
            byte[] number=new byte[32];

            //RandomNumberGenerator disposable(imha edilebilir) olduğu için using ile kullanılır yani scoptan çıktığında işimiz bitip imha edileceği için böyle yazılır. 
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(number);
            };
            return Convert.ToBase64String(number);

        }
    }
}
