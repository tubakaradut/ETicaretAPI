using System.Runtime.Serialization;

namespace ETicaretAPI.Application.Exceptions
{
    public class UserCreaterFailedException : Exception
    {
        public UserCreaterFailedException()
        {
        }

        public UserCreaterFailedException(string? message) : base(("Kullanıcı oluşturulurken beklenmeyen bir hatayla karşılaşıldı!"))
        {

        }

        public UserCreaterFailedException(string? message, Exception? innerException) : base(message, innerException)
        {


        }

      
    }
}
