namespace Service.Services.Abstractions
{
    public interface IEmailService
    {
        void SendNewPassword(string email, string password, string fullname, string username, string company = "FPT Company");

        void SendForgotPassword(string email, string password, string fullname, bool isEmployee, string company = "FPT Company");
    }
}