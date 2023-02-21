using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.DTOs.Configuration;
using ETicaretAPI.Application.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Configuration
{
    public class ApplicationService : IApplicationService
    {
        public List<Menu> GetAuthorizeDefinitionEndpoints(Type type)
        {
            //programda yani apida çalışan assembly bulmak için 
            Assembly assembly = Assembly.GetAssembly(type);
            var controllers = assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(ControllerBase)));

            List<Menu> menus = new();


            if (controllers != null)
            {
                foreach (var controller in controllers) //controller içinde authorizedefinitionattribute ile işaretlenmiş actionları getirmek için
                {
                    var actions = controller.GetMethods().Where(m => m.IsDefined(typeof(AuthorizeDefinitionAttribute)));

                    if (actions != null)
                    {
                        foreach (var action in actions)
                        {
                            var attributes = action.GetCustomAttributes(true);

                            if (attributes != null)
                            {
                                Menu menu = null;

                                var authorizeDefinitionAttribue = attributes.FirstOrDefault(x => x.GetType() == typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;

                                if (!menus.Any(x => x.MenuName == authorizeDefinitionAttribue.Menu))
                                {
                                    menu = new() { MenuName = authorizeDefinitionAttribue.Menu };
                                    menus.Add(menu);
                                }
                                else
                                {
                                    menu = menus.FirstOrDefault(x => x.MenuName == authorizeDefinitionAttribue.Menu);
                                }

                                Application.DTOs.Configuration.Action _action = new()
                                { //enumdaki değeri string olarak getirmek için
                                    ActionType = Enum.GetName(typeof(ActionType), authorizeDefinitionAttribue.ActionType),

                                    Definition = authorizeDefinitionAttribue.Definition

                                };

                                //attributeler içinden httpmethodattribute vasıtasıyla hangi http methodu olduğunu bulacağız.
                                var httpAttribute = attributes.FirstOrDefault(x => x.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) as HttpMethodAttribute; ;

                                if (httpAttribute != null)
                                {
                                    _action.HttpType = httpAttribute.HttpMethods.First();

                                }
                                else // htttpattrıbute gelmediyse default olarak get methodunu ver
                                {
                                    _action.HttpType = HttpMethods.Get;
                                }

                                //actionlara özgü uniq code  yapmak için
                                _action.EndpointCode=$"{_action.HttpType}.{_action.ActionType}.{_action.Definition.Replace(" ","")}";

                                menu.Actions.Add(_action);
                            }



                        }
                    }

                }
            }

            return menus;


        }
    }
}
