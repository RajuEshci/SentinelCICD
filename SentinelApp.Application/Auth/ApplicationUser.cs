using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Auth
{
	public class ApplicationUser:IdentityUser
	{
		// Additional properties if needed
		public ICollection<IdentityUserClaim<string>> Claims { get; set; }
	}

	public class ApplicationRole : IdentityRole
	{
		// Additional properties if needed
	}

	public class LoginModel
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class TokenModel
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}

	public class RoleAssignmentModel
	{
		public string UserId { get; set; }
		public string RoleName { get; set; }
	}

	public class ClaimModel
	{
		public string UserId { get; set; }
		public string ClaimType { get; set; }
		public string ClaimValue { get; set; }
	}

	public class RegisterModel
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string Role { get; set; }  // Optional: You can assign a role at registration
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string CountryCode { get; set; }
		public string AccessCode { get; set; }
		public List<int> CarePathPlanList { get; set; }
		//public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}
