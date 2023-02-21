using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Repositories.EndpointRepo;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identiy;
using ETicaretAPI.Persistence.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<AppUser> _userManager;
        readonly IEndpointReadRepository _endpointReadRepository;

        public UserService(UserManager<AppUser> userManager, IEndpointReadRepository endpointReadRepository)
        {
            _userManager = userManager;
            _endpointReadRepository = endpointReadRepository;
        }

        public int TotalUsersCount => _userManager.Users.Count();

        public async Task AssignRoleToUser(string[] roles, string userId)
        {
            AppUser user=await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);
                await _userManager.AddToRolesAsync(user, roles);
            }
            
        }

        public async Task<CreateUserResponseDTO> CreateAsync(CreateUserDTO model)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                Email = model.Email,
                NameSurname = model.NameSurname,
                UserName = model.UserName
            }, model.Password);


            CreateUserResponseDTO response = new() { Succeded = result.Succeeded };

            if (result.Succeeded) response.Message = "Kullanıcı oluşturuldu";
            else
            {
                foreach (var error in result.Errors)
                {
                    response.Message += $"{error.Code} - {error.Description} <br>";
                }
            }
            return response;
        }

        public async Task<List<UserListDTO>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(user => new UserListDTO

            {
                Id = user.Id,
                Email = user.Email,
                NameSurname = user.NameSurname,
                TwoFactorEnabled = user.TwoFactorEnabled,
                UserName = user.UserName

            }).ToList();
        }

        public async Task<string[]> GetRolesToUserAsync(string userIdOrUsername)
        {
           AppUser user =await _userManager.FindByIdAsync(userIdOrUsername);
            if (user==null)
            {
                user = await _userManager.FindByNameAsync(userIdOrUsername);
            }

            if (user!=null)
            {
                var userRoles=await _userManager.GetRolesAsync(user);
               return  userRoles.ToArray();
            }

            return null; //hata mesajı verilebilir

        }

        public async Task<bool> HasRolePermissionToEndpointAsync(string userName, string endpointCode)
        {
            var userRoles = await GetRolesToUserAsync(userName);
            if (!userRoles.Any()) return false;

           Endpoint? endpoint=await _endpointReadRepository.Table.Include(r => r.AppRoles).FirstOrDefaultAsync(e => e.EndpointCode == endpointCode);
            if (endpoint == null) return false;

            var hasRole = false;
            var endpointRoles = endpoint.AppRoles.Select(r => r.Name);

            foreach (var userRole in userRoles)
            {
                if (!hasRole)
                {
                    foreach (var endpointRole in endpointRoles)
                    {
                        if (userRole == endpointRole)
                        {
                            hasRole = true;
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return hasRole;

        }

        public async Task UpdateRefreshToken(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccesssTokenDate)

        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = accessTokenDate.AddMinutes(addOnAccesssTokenDate);
                await _userManager.UpdateAsync(user);
            }
            else
            {
                throw new NotFoundUserException();
            }

        }

        public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                resetToken = resetToken.UrlDecode();
                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                if (result.Succeeded)
                    await _userManager.UpdateSecurityStampAsync(user);
                else
                    throw new PasswordChangeFailedException();
            }
        }
    }
}
