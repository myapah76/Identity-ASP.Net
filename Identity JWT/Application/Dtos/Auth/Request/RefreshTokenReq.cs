using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth.Request;

public class RefreshTokenReq
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}

