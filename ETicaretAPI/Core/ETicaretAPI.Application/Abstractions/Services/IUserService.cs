using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Domain.Entities.Identiy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<CreateUserResponseDTO> CreateAsync(CreateUserDTO model);

        Task UpdatePasswordAsync(string userId, string resetToken, string newPassword);

        Task UpdateRefreshToken(string refreshToken,AppUser user,DateTime accessTokenDate, int addOnAccesssTokenDate);

        Task<List<UserListDTO>> GetAllUsers();

        int TotalUsersCount { get; }

        Task AssignRoleToUser(string[] roles,string userId);

        Task<string[]> GetRolesToUserAsync(string userIdOrUsername);

        Task<bool> HasRolePermissionToEndpointAsync(string userName, string endpointCode);  //kullanıcının endpoint için yetkisi varmı diye
    }
}
