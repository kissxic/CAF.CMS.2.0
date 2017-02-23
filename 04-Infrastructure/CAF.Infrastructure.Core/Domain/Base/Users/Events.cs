
namespace CAF.Infrastructure.Core.Domain.Users
{
    public class UserRegisterEvent
    {
        private readonly string _phone;
        private readonly string _message;
        public UserRegisterEvent(string phone, string message)
        {
            this._phone = phone;
            this._message = message;
        }
        public string Phone
        {
            get { return _phone; }
        }
        public string Message
        {
            get { return _message; }
        }
    }

}