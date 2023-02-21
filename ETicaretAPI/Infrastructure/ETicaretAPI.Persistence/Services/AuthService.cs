using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identiy;
using ETicaretAPI.Persistence.Helper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        readonly UserManager<AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly IConfiguration _configuration;
        readonly SignInManager<AppUser> _signInManager;
        readonly IUserService _userService;
        readonly IMailService _mailService;

        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, IConfiguration configuration, SignInManager<AppUser> signInManager, IUserService userService, IMailService mailService)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _signInManager = signInManager;
            _userService = userService;
            _mailService = mailService;
        }

        public async Task<Token> GoogleLoginAsync(string IdToken, int accessTokenLifeTime)
        {
            var settigs = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:App_Id"] }     
                
                //console cloud googledan aldığımız clientId                                
            };

            //kullanıcıdan gelen tokenla settigsi doğrulamak için
            var payload = await GoogleJsonWebSignature.ValidateAsync(IdToken, settigs);

            //doğrulama sonucu userinfo nesne oluşturulur.
            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");

            //info ile oluşturulan nesne identityden gelen appuserlogin tablosunda varmı diye aranır. 
            Domain.Entities.Identiy.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            //eğer user boş ise kaydedilecek varsa login olacak

            bool result = user != null;

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        UserName = payload.Email,
                        NameSurname = payload.Name
                    };
                    IdentityResult createResult = await _userManager.CreateAsync(user);
                    result = createResult.Succeeded;
                }
            }
            if (result)
            {
                //kullanıcı oluşturulduktan sonra appuserlogin tablosuna da eklenir
                await _userManager.AddLoginAsync(user, info);
            }
            else
            {
                throw new Exception("Invalid external authentication");
            }
            Application.DTOs.Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime,user);
             await _userService.UpdateRefreshToken(token.RefreshToken,user,token.Expiration,10);

            return token;
        }

        public async Task<Token> LoginAsync(string usernameoremail, string password,int accessTokenLifeTime)
        {
            Domain.Entities.Identiy.AppUser user = await _userManager.FindByNameAsync(usernameoremail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(usernameoremail);
            }
            if (user == null)
            {
                throw new NotFoundUserException();
            }

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user,password, false);

            if (result.Succeeded)
            {
                Application.DTOs.Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime,user);

                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 10);

                return token;
            }
            else
            {
                throw new AuthenticationErrorException();
            }

        }


        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
            AppUser? user=await _userManager.Users.FirstOrDefaultAsync(x=>x.RefreshToken== refreshToken);

            if (user!=null && user?.RefreshTokenEndDate> DateTime.UtcNow)
            {
                Token token = _tokenHandler.CreateAccessToken(15,user);
                await _userService.UpdateRefreshToken(token.RefreshToken,user, token.Expiration, 10);
                return token;
            }
            else
            {
                throw new  NotFoundUserException();
            }

        }




        //şifreyi yenileme işlemi için
        public async Task PasswordResetAsync(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                resetToken = resetToken.UrlEncode();

                await _mailService.SendPasswordResetMailAsync(email, user.Id, resetToken);

            }
        }

        //reset token doğrulama işlemi için 
        public async Task<bool> VerifyResetTokenAsync(string resetToken, string userId)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                resetToken = resetToken.UrlDecode();

                return await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);

            }
            return false;
        }
    }
}
