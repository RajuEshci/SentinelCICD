
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SentinelApp.Application.Auth
{
	public class TokenService
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IDbConnection _dbConnection;

		public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, IDbConnection dbConnection)
		{
			_configuration = configuration;
			_userManager = userManager;
			_dbConnection = dbConnection;
		}
		//public async Task<(string accessToken, DateTime expiresAt)> GenerateAccessToken(UserViewModel user)
		//{
		//	var authClaims = new List<Claim>
		//	{
		//		new Claim("test","test"),
		//		new Claim(ClaimTypes.Name.ToString(), user.EmailId),
		//		new Claim(ClaimTypes.Sid,user.Id.ToString()),
		//		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		//		new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
		//		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
		//		new Claim("UserData",JsonConvert.SerializeObject(user))
		//	};

		//	//var userRoles = await _userManager.GetRolesAsync(user);
		//	//foreach (var role in userRoles)
		//	//{
		//	//	authClaims.Add(new Claim(ClaimTypes.Role, role));
		//	//}

		//	// Get user claims and add them
		//	//var userClaims = await _userManager.GetClaimsAsync(user);
		//	//authClaims.AddRange(userClaims);

		//	var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
		//	var Minutes = Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]);
		//	DateTime AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(Minutes);
		//	var token = new JwtSecurityToken(
		//		issuer: _configuration["Jwt:Issuer"],
		//		audience: _configuration["Jwt:Audience"],
		//		expires: AccessTokenExpiresAt,
		//		claims: authClaims,
		//		signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
		//	);

		//	return (new JwtSecurityTokenHandler().WriteToken(token), AccessTokenExpiresAt);
		//}
	}
}
