using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWTTokenIdentity.Data;
using JWTTokenIdentity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JWTTokenIdentity.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> userManager;

    public AuthController(UserManager<ApplicationUser> userManager)
    {
      this.userManager = userManager;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> InsertUser()
    {
      var user = new ApplicationUser
      {
        UserName = "test",
        Email = "test@example.com",
        SecurityStamp = Guid.NewGuid().ToString()
      };

      var result = await userManager.CreateAsync(user, "MyPassword1!");

      if (result.Succeeded)
      {
        await userManager.AddToRoleAsync(user, "Customer");

        // var mobileNoClaim = new Claim("MobileNo", "1111111111", ClaimValueTypes.String);

        return Ok(user);
      }
      else
      {
        return BadRequest(result);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(LoginDto data)
    {
      var user = await userManager.FindByNameAsync(data.UserName);

      if (user != null && await userManager.CheckPasswordAsync(user, data.Password))
      {
        var claim = new[]
        {
          new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
        };

        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecretKey"));

        var token = new JwtSecurityToken(
          issuer: "https://www.mysite.com",
          audience: "https://www.mysite.com",
          expires: DateTime.UtcNow.AddHours(1),
          signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
          Token = new JwtSecurityTokenHandler().WriteToken(token),
          Expiration = token.ValidTo
        });
      }

      return Unauthorized();
    }
  }
}