using System.Net.Http.Json;
using ArquiteturaBaseWeb.Models;

namespace ArquiteturaBaseWeb.Services;

public sealed class AuthService
{
    private readonly ApiService api;
    private readonly SessionService session;

    public AuthService(ApiService api, SessionService session)
    {
        this.api = api;
        this.session = session;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await api.Client.PostAsJsonAsync("api/Auth/login", new LoginRequest(username, password));
        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (result is null || string.IsNullOrWhiteSpace(result.Token) || result.ExpiresAt <= DateTimeOffset.UtcNow)
            return false;

        await session.SetAsync(new SessionData(result.Token, result.ExpiresAt, result.Username, result.UserId));
        return true;
    }
}