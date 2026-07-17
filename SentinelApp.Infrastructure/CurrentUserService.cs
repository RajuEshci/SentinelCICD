using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SentinelApp.Application.Interfaces;
using System.Text.Json;

namespace SentinelApp.Infrastructure
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? UserId => User?.FindFirstValue(ClaimTypes.Sid);
        public string? UserName => User?.FindFirstValue(ClaimTypes.Name);
        public string? Email => User?.FindFirstValue(ClaimTypes.Email);
        public string? Role => User?.FindFirstValue(ClaimTypes.Role);
        public int RoleId
        {
            get
            {
                var value = User?.FindFirstValue("RoleId");
                if (int.TryParse(value, out var id))
                    return id;
                return 0;
            }
        }
        public string? AccessLevel => User?.FindFirstValue("AccessLevel");
		public int QuestionnaireId
		{
			get
			{
				var value = User?.FindFirstValue("UserObject");
				if (string.IsNullOrWhiteSpace(value))
					return 0;

				var user = JsonSerializer.Deserialize<UserClaim>(value);

				return user?.QuestionnaireId ?? 0;
			}
		}
		public string Gender
		{
			get
			{
				var value = User?.FindFirstValue("UserObject");
				if (string.IsNullOrWhiteSpace(value))
					return string.Empty;

				var user = JsonSerializer.Deserialize<UserClaim>(value);

				return user?.Gender ?? string.Empty;
			}
		}
		public int PreferredLanguageId
		{
			get
			{
				var value = User?.FindFirstValue("UserObject");
				if (string.IsNullOrWhiteSpace(value))
					return 0;

				var user = JsonSerializer.Deserialize<UserClaim>(value);

				return user?.PreferredLanguageId ?? 0;
			}
		}
		public DateTime? CreatedOn
		{
			get
			{
				var value = User?.FindFirstValue("UserObject");
				if (string.IsNullOrWhiteSpace(value))
					return null;

				var user = JsonSerializer.Deserialize<UserClaim>(value);

				return user?.CreatedOn ?? null;
			}
		}
		public DateTime? DateOfBirth
		{
			get
			{
				var value = User?.FindFirstValue("UserObject");
				if (string.IsNullOrWhiteSpace(value))
					return null;

				var user = JsonSerializer.Deserialize<UserClaim>(value);

				return user?.DateOfBirth ?? null;
			}
		}
		public class UserClaim
        {
            public int QuestionnaireId { get; set; }
            public string? Gender { get; set; }
			public int PreferredLanguageId { get; set; }
			public DateTime? CreatedOn { get; set; }
			public DateTime DateOfBirth { get; set; }
        }
	}
}
