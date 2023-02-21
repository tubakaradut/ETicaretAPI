using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace ETicaretAPI.API.Filters
{
    public class CustomAuthorizeFilterRole : IAsyncActionFilter
    {
        readonly IUserService _userService;

        public CustomAuthorizeFilterRole(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var name= context.HttpContext.User.Identity?.Name; //isteği atan kişiyi yakamamız gerekir jwt token ile giriş yapıldığında claimtypes.name bize tokenhandler clasında tanımladığımız  username verecek.

          
            if (!string.IsNullOrEmpty(name) && name != "tuba") //default olarak adı bu olan admin olsun yani asagıdakilerden muaf olsun
            {
                //hangi actiona istek gönderildiyse onu yakalacagız.descriptor ile hangi actiona istek gönderiliyorsa onunla ilgili yüzeysel bilgileri yakalıyor  ancak adına dirakt vermez bundan contorlleractionDescriptor referans edersek alabiliriz.
                var descriptor = context.ActionDescriptor as ControllerActionDescriptor;


                //methodinfo ile Reflection içindeki yani ilgili actionın işaretlenmiş  olan kısmını authorizedefiniton kısmını yakalayabiliyoruz.
                var attribute = descriptor.MethodInfo.GetCustomAttribute(typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;


                //htttp type almak için httpmethodattribute(get,post vb. base classı) den faydalanırız.
                var httpAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(HttpMethodAttribute)) as HttpMethodAttribute;

                //özel olarak yazmış oluşturmuş olduğumuz endpointcode ile karşılaştırma yapmak için attributenden endpointcode yakalıyoruz. 
                var code = $"{(httpAttribute != null ? httpAttribute.HttpMethods.First() : HttpMethods.Get)}.{attribute.ActionType}.{attribute.Definition.Replace(" ", "")}";


                //code bilgisini veritabanındaki code ile kontrol etmek için

                var hasRole = await _userService.HasRolePermissionToEndpointAsync(name, code);

                if (!hasRole)
                    context.Result = new UnauthorizedResult();
                else
                    await next();
            }
            else
                await next();

        }
    }
}
