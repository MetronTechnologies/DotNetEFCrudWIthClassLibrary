using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EFDataManagementLibrary.Models.DataTransferObject.Requests;
using EFDataManagementLibrary.Models.DataTransferObject.Responses;

namespace EfCrudAuthorizationAuthentication.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase {

	private readonly UserManager<IdentityUser> userManager;
	private readonly IConfiguration configuration;

	public AuthController(IConfiguration config, UserManager<IdentityUser> user) {
		userManager = user;
		configuration = config;
	}

	[HttpPost]
	[Route("register")]
	public async Task<IActionResult> registerUser([FromBody] RegisterUser registerUser) {
		var existUser = await userManager.FindByNameAsync(registerUser?.name);
		if (existUser != null) {
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				new GenericResponse {
					status = "Error",
					message = "User already exists"
				}
			);
		}
		Console.WriteLine("Forming identity user");
		IdentityUser user = new IdentityUser() {
			NormalizedUserName = registerUser.name,
			UserName = registerUser.name,
		};
		Console.WriteLine("Identity user formed. Creating User ...");
		IdentityResult identityResult = await userManager.CreateAsync(user, registerUser.password);
		if (!identityResult.Succeeded) {
			Console.WriteLine("Could not create user");
			return StatusCode(
				StatusCodes.Status500InternalServerError,
				new GenericResponse {
					status = "Error",
					message = "User was not saved "
				}
			);
		}
		return Ok(new GenericResponse {
			status = "Success",
			message = "User was successfully saved"
		});
	}

	[HttpPost]
	[Route("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequest login) {
		IdentityUser? identityUser = await userManager.FindByNameAsync(login.username!);
		bool passwordCheck = await userManager.CheckPasswordAsync(identityUser!, login.password!);
		if (identityUser != null && passwordCheck) {
			IList<string> userRoles = await userManager.GetRolesAsync(identityUser);
			List<Claim> authClaims = new List<Claim> {
				new Claim(ClaimTypes.Name,identityUser.UserName!),
				new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
			};
			foreach (string userRole in userRoles) {
				authClaims.Add(new Claim(ClaimTypes.Role, userRole));
			}
			SymmetricSecurityKey authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
			JwtSecurityToken token = new JwtSecurityToken (
				issuer: configuration["JWT:ValidIssuer"],
				audience: configuration["JWT:ValidAudience"],
				expires: System.DateTime.Now.AddHours(1),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
			);
			return Ok(
				new {
					token = new JwtSecurityTokenHandler().WriteToken(token)
				}
			);
		}
		return Unauthorized();
	}


}

