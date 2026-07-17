using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelApp.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        string? Role { get; }
        string? AccessLevel { get; }
        int RoleId { get; }
        int QuestionnaireId { get; }
        string Gender { get; }
		int PreferredLanguageId { get; }
        DateTime? CreatedOn { get; }
        DateTime? DateOfBirth { get; }
	}
}
