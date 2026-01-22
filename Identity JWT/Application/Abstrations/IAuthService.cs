using Application.Dtos.Auth.Request;
using Application.Dtos.Auth.Response;

namespace Application.Abstrations;

public interface IAuthService
{
    Task<LoginRep> LoginAsync(LoginReq request);
    Task<bool> LogoutAsync(string refreshToken, string? accessToken = null);
    Task<RefreshTokenRep> RefreshTokenAsync(string refreshToken);

    Task<RegisterRep> RegisterAsync(RegisterReq request);
    Task ConfirmOtpAsync(ConfirmOtpReq request);
}

