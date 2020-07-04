using api.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using web.Configuration;
using web.Models.auth;

namespace web.Controllers
{
    [ApiVersion("0")]
    [Route("/api/v0/authentication")]
    [ProducesErrorResponseType(typeof(void))]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtOptions _jwtOptions;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager,
            IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel model)
        {
            //try to sign in user bu username or email
            var appUser = await _userManager.Users.SingleOrDefaultAsync(r => r.UserName == model.UnameOrEmail);
            if (appUser is null)
            {
                appUser = await _userManager.Users.SingleOrDefaultAsync(r => r.Email == model.UnameOrEmail);
                if (appUser is null)
                    return Unauthorized(new { error = "You have entered an invalid username or password" });

                model.UnameOrEmail = appUser.UserName;
            }

            var result = await _signInManager.PasswordSignInAsync(model.UnameOrEmail, model.Password, true, false);
            if (!result.Succeeded)
                return Unauthorized(new { error = "You have entered an invalid username or password" });

            var res = await GenerateJwtTokenAsync(model.UnameOrEmail, appUser);
            return Ok(res);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }

        [HttpGet("provider/login")]
        public IActionResult ProviderLogin(string provider, string? returnUrl = null)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider,
                $"/api/v0/authentication/provider/callback?returnUrl={returnUrl}");

            var res = new ChallengeResult(provider, properties);
            return res;
        }

        [HttpPost("provider/logout")]
        public async Task<IActionResult> ProviderLogout(string provider)
        {
            var user = await _userManager.GetUserAsync(User);
            var userLoginInfos = await _userManager.GetLoginsAsync(user);

            var userLoginInfo = userLoginInfos.FirstOrDefault(item => item.LoginProvider == provider);

            if (userLoginInfo is null)
            {
                return Ok();
            }

            await _userManager.RemoveLoginAsync(user, userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);

            return Ok();
        }

        [HttpGet("provider/callback")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ProviderCallback(string? remoteError = null, string? returnUrl = null)
        {
            if (remoteError != null)
            {
                return Unauthorized(new { error = $"Error from external provider: {remoteError}" });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Login failed, typically because they cancelled.
                return Redirect(returnUrl);
            }

            if (!info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                return Unauthorized(new { error = "Provider did not return an e-mail address" });
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                false, true);

            if (result.Succeeded)
            {
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return Redirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return Unauthorized(new { error = "User is locked out" });
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            User user;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = await _userManager.FindByEmailAsync(email);
            }

            if (user is null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return Error("Error creating user", createResult.Errors);
                }
            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                return Error("Error adding login to user", addLoginResult.Errors);
            }

            await _signInManager.SignInAsync(user, true);
            return Redirect(returnUrl);
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterModel model)
        {
            var newUser = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Email
            };

            IdentityResult identityResult;
            if (model.Password is null)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is null)
                {
                    identityResult = await _userManager.CreateAsync(newUser);
                }
                else
                {
                    // We only let users that registered without password and
                    // also don't have any other provider linked to login this way
                    if (user.PasswordHash is null)
                    {
                        var logins = await _userManager.GetLoginsAsync(user);
                        if (!logins.Any())
                        {
                            await UpdateUser(user, model);

                            return await SignIn(user);
                        }
                    }

                    identityResult = IdentityResult.Failed(new IdentityErrorDescriber().DuplicateUserName(model.Email));
                }
            }
            else
            {
                identityResult = await _userManager.CreateAsync(newUser, model.Password);
            }

            if (!identityResult.Succeeded)
            {
                return BadRequest(identityResult.Errors);
            }

            return await SignIn(newUser);
        }

        [Authorize]
        [HttpGet("ping")]
        public async Task<ActionResult<List<PingResult>>> Ping()
        {
            var user = await _userManager.GetUserAsync(User);
            var logins = await _userManager.GetLoginsAsync(user);

            return Ok(new PingResult(user.Id, user.UserName, User.Identity.IsAuthenticated,
                logins.Select(item => item.LoginProvider).ToList()));
        }

        [HttpPost("forgotpasswordreset")]
        public async Task<ActionResult> ForgotPasswordReset(ForgotPasswordResetModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            return Ok();
        }

        private async Task<ActionResult<string>> SignIn(User user)
        {
            await _signInManager.SignInAsync(user, true);

            return Ok(await GenerateJwtTokenAsync(user.Email, user));
        }

        private Task UpdateUser(User user, RegisterModel model)
        {
            if (!string.IsNullOrEmpty(model.Name))
            {
                user.Name = model.Name;
            }

            return _userManager.UpdateAsync(user);
        }

        private async Task<string> GenerateJwtTokenAsync(string email, User user)
        {
            if (!_jwtOptions.IsValid())
            {
                throw new Exception("Missing JWT configurations.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_jwtOptions.ExpireDays);

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UnauthorizedObjectResult Error(string message, IEnumerable<IdentityError> identityErrors)
        {
            var errors = string.Join(",", identityErrors.Select(e => e.Description).ToList());

            return Unauthorized(new { error = $"{message}: {errors}" });
        }
    }
}
