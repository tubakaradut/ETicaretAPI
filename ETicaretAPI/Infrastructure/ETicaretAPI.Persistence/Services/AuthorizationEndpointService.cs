using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.DTOs.Configuration;
using ETicaretAPI.Application.Repositories.EndpointRepo;
using ETicaretAPI.Application.Repositories.MenuuRepo;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identiy;
using ETicaretAPI.Persistence.Repositories.MenuuRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthorizationEndpointService : IAuthorizationEndpointService
    {
        readonly IEndpointWriteRepository _endpointWriteRepository;
        readonly IEndpointReadRepository _endpointReadRepository;
        readonly IApplicationService _applicationService;
        readonly IMenuuReadRepository _menuuReadRepository;
        readonly IMenuuWriteRepository _menuuWriteRepository;
        readonly RoleManager<AppRole> _roleManager;

        public AuthorizationEndpointService(IEndpointWriteRepository endpointWriteRepository, IEndpointReadRepository endpointReadRepository, IApplicationService applicationService, IMenuuReadRepository menuuReadRepository, IMenuuWriteRepository menuuWriteRepository, RoleManager<AppRole> roleManager)
        {
            _endpointWriteRepository = endpointWriteRepository;
            _endpointReadRepository = endpointReadRepository;
            _applicationService = applicationService;
            _menuuReadRepository = menuuReadRepository;
            _menuuWriteRepository = menuuWriteRepository;
            _roleManager = roleManager;
        }

        public async Task AssignRoleEndpointAsync(string[] roles, string endpointCode, string menu, Type type) //type diye yazdığımız assemblyde authorize olarak işaretlenmiş olarak yakaladığımız type o yüzden bunu apidan almamaız gerektiği için type olarak verildi
        {
            Menuu _menu = await _menuuReadRepository.GetSingleAsync(m => m.MenuName == menu);
            if (_menu == null)
            {
                _menu = new()
                {
                    Id = Guid.NewGuid(),
                    MenuName = menu
                };
                await _menuuWriteRepository.AddAsync(_menu);

                await _menuuWriteRepository.SaveAsync();
            }

            Endpoint? endpoint = await _endpointReadRepository.Table.Include(e => e.Menuu).Include(e => e.AppRoles).FirstOrDefaultAsync(e => e.EndpointCode == endpointCode && e.Menuu.MenuName == menu);

            if (endpoint == null)
            {
                var action = _applicationService.GetAuthorizeDefinitionEndpoints(type)
                        .FirstOrDefault(m => m.MenuName == menu)
                        ?.Actions.FirstOrDefault(e => e.EndpointCode == endpointCode);

                endpoint = new()
                {
                    EndpointCode = action.EndpointCode,
                    ActionType = action.ActionType,
                    HttpType = action.HttpType,
                    Definition = action.Definition,
                    Id = Guid.NewGuid(),
                    Menuu = _menu
                };

                await _endpointWriteRepository.AddAsync(endpoint);
                await _endpointWriteRepository.SaveAsync();

            }

            foreach (var role in endpoint.AppRoles) //varolanları silip hepsini yeniden secilenleri vermek için 
                endpoint.AppRoles.Remove(role);

            var appRoles = await _roleManager.Roles.Where(r => roles.Contains(r.Name)).ToListAsync();

            foreach (var role in appRoles)
                endpoint.AppRoles.Add(role);

            await _endpointWriteRepository.SaveAsync();
        }

        public async Task<List<string>> GetRolesToEndpointAsync(string endpointCode, string menu)
        {
            Endpoint? endpoint = await _endpointReadRepository.Table.Include(x => x.AppRoles).Include(m => m.Menuu).FirstOrDefaultAsync(e => e.EndpointCode == endpointCode && e.Menuu.MenuName == menu);
            if (endpoint != null)
            {
                return endpoint.AppRoles.Select(r => r.Name).ToList();
            }
            return null;


        }
    }
}
