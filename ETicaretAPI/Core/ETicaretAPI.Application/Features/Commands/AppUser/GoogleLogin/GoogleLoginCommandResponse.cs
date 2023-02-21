using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X = ETicaretAPI.Application.DTOs;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandResponse
    {
        public X.Token Token { get; set; }
    }
}
