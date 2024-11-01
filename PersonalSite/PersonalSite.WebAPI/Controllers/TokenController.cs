using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Application.Common.Interfaces;
using PersonalSite.Application.DTOs.Account;
using PersonalSite.Infrastructure.Identity;

namespace PersonalSite.WebAPI.Controllers;

[Route("api/token")]
[ApiController]
public class TokenController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAccountService _accountService;

    public TokenController(UserManager<ApplicationUser> userManager, IAccountService accountServicenService)
    {
        _userManager = userManager;
        _accountService = accountServicenService;
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto tokenDto)
    {
        if (tokenDto is null)
        {
            return BadRequest();
        }

        var principal = _accountService.GetPrincipalFromExpiredToken(tokenDto.Token);
        var username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username);
        if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return Unauthorized();

        var token = await _accountService.GenerateJWTokenString(user);
        user.RefreshToken = _accountService.GenerateRefreshToken();

        await _userManager.UpdateAsync(user);

        return Ok(new AuthenticationResponse { JWToken = token, RefreshToken = user.RefreshToken });
    }
}