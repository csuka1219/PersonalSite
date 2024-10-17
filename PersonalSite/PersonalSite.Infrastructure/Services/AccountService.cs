using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PersonalSite.Application.Common.Interfaces;
using PersonalSite.Application.DTOs.Account;
using PersonalSite.Domain.Settings;
using PersonalSite.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonalSite.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JWTSettings _jwtSettings;
    public AccountService(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<JWTSettings> jwtSettings,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
        _signInManager = signInManager;
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            throw new Exception($"No Accounts Registered with {request.Email}.");
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            throw new Exception($"Invalid Credentials for '{request.Email}'.");
        }

        if (!user.EmailConfirmed)
        {
            throw new Exception($"Account Not Confirmed for '{request.Email}'.");
        }

        JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
        AuthenticationResponse response = new AuthenticationResponse()
        {
            Id = user.Id,
            JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email!,
            UserName = user.UserName!,
        };

        var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

        response.Roles = rolesList.ToList();
        response.IsVerified = user.EmailConfirmed;

        return response;
    }

    private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();

        for (int i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("roles", roles[i]));
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("uid", user.Id),
        }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }
}
