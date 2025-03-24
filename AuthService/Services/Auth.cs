namespace AuthService.Services;

public class Auth
{
    public async Task<bool> Login(string username, string password)
    {
        return await Task.FromResult(true);
    }

    public async Task<bool> Logout(string username)
    {
        return await Task.FromResult(true);
    }
}