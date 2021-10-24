using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RefreshTokenAuth.Models;
using RefreshTokenAuth.Repositories;
using RefreshTokenAuth.Services;
using System.Threading.Tasks;

namespace RefreshTokenAuth.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            var user = UserRepository.Get(model.UserName, model.Password);

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos." });

            var token = TokenService.GenerateToken(user);
            var refreshToken = TokenService.GenerateRefreshToken();
            TokenService.SaveRefreshtoken(user.UserName, refreshToken);

            user.Password = "";

            return new
            {
                user = user,
                token = token,
                refreshToken = refreshToken
            };
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(string token, string refreshToken)
        {
            var principal = TokenService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            var savedRefreshToken = TokenService.GetRefreshToken(username);

            if (savedRefreshToken != refreshToken)
                throw new SecurityTokenException(message: "Invalid refresh token");

            var newJwtToken = TokenService.GenerateToken(principal.Claims);
            var newRefreshToken = TokenService.GenerateRefreshToken();

            TokenService.DeleteRefreshToken(username, refreshToken);
            TokenService.SaveRefreshtoken(username, refreshToken);

            return new ObjectResult(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }
    }
}
