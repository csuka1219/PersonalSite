using PersonalSite.Application.DTOs.Account;

namespace PersonalSite.Application.Common.Interfaces;

public interface IAccountService
{
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
    //Task ForgotPassword(ForgotPasswordRequest model, string origin);
    //Task<Response<string>> ResetPassword(ResetPasswordRequest model);
}

