using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Helper
{

    public static class CustomEncoders
    {
        public static string UrlEncode(this string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value); //refrehtoken şifreleme işlemi

            return WebEncoders.Base64UrlEncode(bytes); //url detaşınabilir hale getirilir.
        }


        public static string UrlDecode(this string value)
        {
            byte[] bytes = WebEncoders.Base64UrlDecode(value); // //refrehtoken şifreleme decode işlemi
            return Encoding.UTF8.GetString(bytes);
        }

    }

}
