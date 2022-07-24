using System.Security.Cryptography;
public class RefreshToken
{
    public string Token { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Expires { get; set; } = DateTime.Now.AddDays(1);
}