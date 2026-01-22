namespace Application.Dtos.Auth.Request;

internal class RegistrationData
{
    public string Email { get; set; } = null!;
    public string HashedPassword { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
}
