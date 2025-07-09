using authapi.DTOs;

namespace authapi.IServices
{
    public interface IAuthService
    {
        Task<(bool success, string message)> RegisterUserAsync(UserSignupDto request);

        Task<(bool success, string message, string token)>LoginUserAsync(UserLoginDto request);


    }
}
