using Api.Dtos;

namespace Api.Modules;

public static class TokenResponseExtension
{
    public static TokenResponse ToTokenResponse(this (string AccessToken, string RefreshToken) values)
    {
        return new TokenResponse(values.AccessToken, values.RefreshToken);
    }
}