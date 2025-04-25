using Microsoft.AspNetCore.Mvc;
using PersonalSite.Application.Common.Interfaces;
using PersonalSite.Application.DTOs.Account;

namespace PersonalSite.WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
    {
        return Ok(await _accountService.AuthenticateAsync(request));
    }
    [HttpGet]
    public async Task<string> Test()
    {
        return "OK";
    }
}
