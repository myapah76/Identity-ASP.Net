using Application.Dtos.User.Respone;

namespace Application.Dtos.Auth.Response;

public class LoginRep
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public UserRep User { get; set; } = null!;
}

