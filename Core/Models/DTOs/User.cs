namespace Core.Models.DTOs;

public class User
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime RegisteredAt { get; set; }
}