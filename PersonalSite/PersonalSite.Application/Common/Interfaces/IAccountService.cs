using PersonalSite.Application.DTOs.Account;
using System.Security.Claims;

namespace PersonalSite.Application.Common.Interfaces;

public interface IAccountService
{
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
    Task<string> GenerateJWTokenString(dynamic user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task Generateuser();
    //Task ForgotPassword(ForgotPasswordRequest model, string origin);
    //Task<Response<string>> ResetPassword(ResetPasswordRequest model);
}

