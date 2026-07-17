using Dapper;
using System.Data;
using System.Transactions;
using SentinelApp.Application.DTO;
using SentinelApp.Application.Interfaces;
using SentinelApp.Domain.Entities;
using SentinelApp.Persistence.Data;
using static Dapper.SqlMapper;
using SentinelApp.Application.Core.CommonExtension;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Net;

namespace SentinelApp.Persistence.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly IDbContext _dbContext;
		public UserRepository(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}
        public async Task<bool> DBHealthy()
        {
            try
            {
                using var connection = _dbContext.CreateConnection();

                if (connection is DbConnection dbConnection)
                {
                    await dbConnection.OpenAsync();
                }
                else
                {
                    connection.Open();
                }

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                command.ExecuteScalar();
                throw new ServiceResponseException(HttpStatusCode.ServiceUnavailable, "Check CICD");
                return true;
            }
            catch (Exception ex)
            {
				throw new ServiceResponseException(HttpStatusCode.ServiceUnavailable, ex.Message);
                return false;
            }
        }
        public async Task<User?> GetByEmailAsync(string email, int Id = 0)
		{
			using var _connection = _dbContext.CreateConnection();

			// Get user first
			const string userSql = @"
				SELECT * FROM Users
				WHERE EmailId = @EmailId 
				AND IsActive = 1 
				AND UserId != @UserId 
				AND FlagDeleted = 0";

			var user = await _connection.QueryFirstOrDefaultAsync<User>(userSql, new { EmailId = email, UserId = Id });
			if (user == null) return null;

			return user;
		}
		public async Task<User?> GetByIdAsync(int Id)
		{
			using var _connection = _dbContext.CreateConnection();

			const string userSql = @"SELECT * FROM Users WHERE UserId = @UserId AND FlagDeleted = 0 AND IsActive=1";
			var user = await _connection.QueryFirstOrDefaultAsync<User>(userSql, new { UserId = Id });
			if (user == null) return null;

			return user;
		}
		public async Task<User?> GetByUsernameAsync(string username, int Id = 0)
		{
			using var _connection = _dbContext.CreateConnection();
			const string sql = "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1 AND UserId != @UserId and FlagDeleted=0";
			return await _connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username, UserId = Id });
		}
		public async Task<int> CreateAsync(User user)
		{
			using var _connection = _dbContext.CreateConnection();
			_connection.Open();
			using var transaction = _connection.BeginTransaction();
			const string sql = @"INSERT INTO Users (UserName,Gender,DateOfBirth,Phonenumber,Mobilenumber,EmailId,Address,PostCode,Country,Password,CreatedBy,IsTnCChecked,AccessCode,OtpAttempts,Otp,OtpGenOn,IsOtpVerify,IsVerify,QuestionnaireId) 
			OUTPUT INSERTED.UserId
            VALUES (@UserName,@Gender,@DateOfBirth,@Phonenumber,@Mobilenumber,@EmailId,@Address,@PostCode,@Country,@Password,@CreatedBy,@IsTnCChecked,@AccessCode,@OtpAttempts,@Otp,@OtpGenOn,@IsOtpVerify,@IsVerify,@QuestionnaireId)";

			int userId = await _connection.ExecuteScalarAsync<int>(sql, user, transaction);

			const string updateAccessCode = @"UPDATE Access_Code SET AccessCode_Status = 1, ModifiedOn = GETDATE() WHERE AccessCode = @AccessCode";

			await _connection.ExecuteAsync(updateAccessCode, new { user.AccessCode }, transaction);

			transaction.Commit();

			return userId;
		}
		public async Task UpdateAsync(User user)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"UPDATE Users SET UserName=@UserName, Address=@Address, PostCode=@PostCode, Country=@Country, Phonenumber=@Phonenumber, Mobilenumber=@Mobilenumber, IsActive=@IsActive, UpdatedBy=@UpdatedBy, QuestionnaireId=@QuestionnaireId, UpdatedOn=GETDATE()";
			if (!string.IsNullOrEmpty(user.Password))
			{
				sql += ", Password=@Password";
			}
			sql += @" WHERE userId=@UserId";
			await _connection.ExecuteAsync(sql, user);
		}

		public async Task DeleteAsync(int userId)
		{
			using var _connection = _dbContext.CreateConnection();

			const string sql = "Update Users set FlagDeleted=1, DeletedOn=GETDATE() WHERE userId=@UserId";
			await _connection.ExecuteAsync(sql, new { UserId = userId });
		}

		public async Task<bool> DisableUserAsync(int userId)
		{
			using var _connection = _dbContext.CreateConnection();

			const string sql = "Update Users set IsActive=0, FlagDeleted=1, DeletedOn=GETDATE(),UpdatedBy=@UpdatedBy,UpdatedOn=GETDATE() WHERE UserId=@UserId";
			var rowsAffected = await _connection.ExecuteAsync(sql, new { UserId = userId, UpdatedBy = userId });
			return rowsAffected > 0;
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync(UserFilter userFilter)
		{
			using var _connection = _dbContext.CreateConnection();

			var sql = @"
        SELECT 
            um.UserID,
            um.FirstName,
            um.LastName,
            um.Email,
            um.DateJoined,
            um.IsActive,
            um.CreatedOn,
            um.UpdatedOn,
            STRING_AGG(rm.RoleName, ', ') AS RoleNames,
            STRING_AGG(CAST(urm.RoleId AS NVARCHAR(MAX)), ',') AS RoleIds
        FROM UserMaster um
        LEFT JOIN UserRoleMaster urm ON um.UserId = urm.UserId
        LEFT JOIN RoleMaster rm ON urm.RoleId = rm.RoleID
        WHERE um.FlagDeleted = 0";

			// 🔹 Apply filters BEFORE GROUP BY
			if (!string.IsNullOrEmpty(userFilter.RoleName))
			{
				sql += " AND rm.RoleName LIKE '%' + @RoleName + '%'";
			}

			if (!string.IsNullOrEmpty(userFilter.SearchText))
			{
				sql += @" AND (
                    um.FirstName LIKE '%' + @SearchText + '%' OR
                    um.LastName LIKE '%' + @SearchText + '%' OR
                    um.Email LIKE '%' + @SearchText + '%'
                )";
			}

			if (!string.IsNullOrEmpty(userFilter.Status))
			{
				userFilter.IsActive = userFilter.Status.ToLower() == "active";
				sql += " AND um.IsActive = @IsActive";
			}

			// 🔹 Now group the results (after filtering)
			sql += @"
        GROUP BY 
            um.UserID, 
            um.FirstName, 
            um.LastName, 
            um.Email, 
            um.DateJoined, 
            um.IsActive,
            um.CreatedOn,
            um.UpdatedOn";

			// 🔹 Sorting
			var sortCol = !string.IsNullOrEmpty(userFilter.SortColumn) ? userFilter.SortColumn : "um.CreatedOn";
			var sortDirection = !string.IsNullOrEmpty(userFilter.SortDirection) ? userFilter.SortDirection : "DESC";
			sql += $" ORDER BY {sortCol} {sortDirection}";

			// 🔹 Pagination
			sql += " OFFSET @Offset ROWS FETCH NEXT @PerPage ROWS ONLY";
			userFilter.Offset = (userFilter.PageNo - 1) * userFilter.PerPage;

			var users = (await _connection.QueryAsync<User>(sql, userFilter)).ToList();

			// 🔹 Optional: You can map roles as a list if needed (instead of aggregated string)
			// For that, you would need a separate query per user — but this current form is more efficient.
			var rolesSql = @"
        SELECT 
            urm.UserId,
            urm.UserRoleId,
            urm.RoleId,
            rm.RoleName RoleName
        FROM UserRoleMaster urm
        INNER JOIN RoleMaster rm ON urm.RoleId = rm.RoleID
        WHERE urm.UserId IN @UserIds AND urm.FlagDeleted=0";

			var userIds = users.Select(u => u.UserId).ToList();
			var allRoles = await _connection.QueryAsync<UserRoleMaster>(rolesSql, new { UserIds = userIds });

			// 3️⃣ Map roles to each user
			//foreach (var user in users)
			//{
			//    user.Roles = allRoles
			//        .Where(r => r.UserId == user.UserID)
			//        .ToList();
			//}
			return users;
		}
		public async Task<User?> ValidateLogin(string emailId)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"SELECT   
                  um.UserId as 'UserId',
		          um.UserName as 'Name',
		          um.EmailId as 'UserName',  
		          um.Gender,
		          um.DateOfBirth,
		          um.Mobilenumber,
		          um.Phonenumber,       
                  um.EmailId,    
		          um.Address, 
		          um.PostCode,  
		          um.Country, 
		          um.IsVerify,
	              um.IsActive,
		          um.IsTnCChecked,
		          um.UpdatedOn,
		          um.PreferredLanguageCode,
		          um.IsConsentUpdated,
		          um.IsExported,
                  um.Password,
				  um.IsOtpVerify,
				  um.QuestionnaireId,
				  um.PreferredLanguageId,
				  um.CreatedOn
		        FROM [dbo].[Users]  um
		        WHERE [EmailId]=@EmailId
		        order by CreatedOn desc";
			return await _connection.QueryFirstOrDefaultAsync<User?>(sql, new { EmailId = emailId });
		}
		public async Task UpdateOtpAsync(User user)
		{
			using var _connection = _dbContext.CreateConnection();

			const string sql = "Update Users set Otp=@Otp, OtpGenOn=GETDATE(), OtpAttempts = 3, UpdatedBy=@UpdatedBy, UpdatedOn=GETDATE() WHERE userId=@UserId";
			await _connection.ExecuteAsync(sql, user);
		}
		public async Task VerifyOtpAsync(User user)
		{
			using var _connection = _dbContext.CreateConnection();

			const string sql = "Update Users set IsOtpVerify=@IsOtpVerify, OtpVerifiedOn=GETDATE(), IsActive = @IsActive, IsVerify=@IsVerify, UpdatedBy=@UpdatedBy, UpdatedOn=GETDATE() WHERE userId=@UserId";
			await _connection.ExecuteAsync(sql, user);
		}
		public async Task<bool> ValidateAccessCodeAsync(string accessCode)
		{
			using var _connection = _dbContext.CreateConnection();
			var sql = @"select * from Access_Code where AccessCode=@AccessCode and IsDelete=0 and AccessCode_Status=0 and (AssignedTo is not null or AssignedTo != '')";
			var isValid = await _connection.QueryFirstOrDefaultAsync<bool>(sql, new { accessCode });
			return isValid;
		}
		public async Task<(UserAccessCode userAccessCode, string msg)> AccessCodeEmail(UserAccessCode userAccessCode)
		{
			using var _connection = _dbContext.CreateConnection();
			var userSql = @"select count(*) from Users where EmailId=@EmailId and IsActive=1 and IsVerify=1 and flagdeleted=0";
			var userData = await _connection.QueryFirstOrDefaultAsync<int>(userSql, new { EmailId = userAccessCode.Email });
			if (userData > 0)
			{
				return (null, "UserExist");
			}
			var userAccessCodeSql = @"select count(*) from Access_Code where AssignedTo=@EmailId and AccessCode_Status=1 and IsDelete=0";
			var userAccessCodeData = await _connection.QueryFirstOrDefaultAsync<int>(userAccessCodeSql, new { EmailId = userAccessCode.Email });
			if (userAccessCodeData > 0)
			{
				return (null, "UserExist");
			}
			userAccessCodeSql = @"select count(*) from Access_Code where AssignedTo=@EmailId and IsDelete=0";
			userAccessCodeData = await _connection.QueryFirstOrDefaultAsync<int>(userAccessCodeSql, new { EmailId = userAccessCode.Email });
			if (userAccessCodeData > 0)
			{
				return (null, "AccessCodeAssign");
			}
			string otp = new Random().Next(100000, 999999).ToString();
			var userAccessSql = @"select * from UserAccessCode where Email=@Email and ISNULL(FlagDeleted, 0) = 0";
			var userAData = await _connection.QuerySingleOrDefaultAsync<UserAccessCode>(userAccessSql, new { Email = userAccessCode.Email });
			if (userAData == null)
			{
				var insertUserAccessSql = @"insert UserAccessCode(Email,Otp,IsOtpVerified,OtpSentOn,CreatedOn,OptionId,Message,FlagDeleted)values(@Email,@Otp,0,GETDATE(),GETDATE(),@OptionId,@Message,0)";
				var inserData = await _connection.ExecuteAsync(insertUserAccessSql, new { Email = userAccessCode.Email, Otp = otp, OptionId = userAccessCode.OptionId, Message = userAccessCode.Message });

				if (inserData > 0)
				{
					userAData = new UserAccessCode { Otp = otp, IsOtpVerified = true };
				}
				return (userAData, "");
			}
			else if (userAccessCode.ResendOtp)
			{
				var updateSql = @"update UserAccessCode set Otp=@Otp,OtpSentOn=GETDATE(),UpdatedOn=GETDATE(), Message=@Message,OptionId=@OptionId where Email=@Email and ISNULL(FlagDeleted, 0) = 0";
				var iData = await _connection.ExecuteAsync(updateSql, new { Otp = otp, Email = userAccessCode.Email, Message = userAccessCode.Message, OptionId = userAccessCode.OptionId });
				if (iData > 0)
				{
					userAData.Otp = otp;
					userAData.IsOtpVerified = true;
				}
				return (userAData, "");
			}
			else
			{
				if (!userAData.IsOtpVerified && userAData.OptionId != userAccessCode.OptionId)
				{
					var updateSql = @"update UserAccessCode set Otp=@Otp, OptionId=@OptionId,OptionText=null,Message=null,OtpSentOn=GETDATE(),UpdatedOn=GETDATE() where Email=@Email and ISNULL(FlagDeleted, 0) = 0";
					var iData = await _connection.ExecuteAsync(updateSql, new { Otp = otp, Email = userAccessCode.Email, OptionId = userAccessCode.OptionId });
					if (iData > 0)
					{
						userAData.Otp = otp;
					}
				}
				return (userAData, "");
			}
		}
		public async Task<UserAccessCode?> AccessCodeCheck(string email, int optionId)
		{
			using var _connection = _dbContext.CreateConnection();
			var userAccessSql = @"select * from UserAccessCode where Email=@Email and ISNULL(FlagDeleted, 0) = 0";
			var userAData = await _connection.QuerySingleOrDefaultAsync<UserAccessCode?>(userAccessSql, new { Email = email });
			return userAData;
		}
		public async Task<(UserAccessCode userAccessCode, string error)> VerifyAccessCodeOtp(string email, string otp, int languageId)
		{
			using var _connection = _dbContext.CreateConnection();
			string sql = @"select * from UserAccessCode where Email=@Email and Otp=@Otp and ISNULL(FlagDeleted, 0) = 0";
			var userAccessCodeData = await _connection.QuerySingleOrDefaultAsync<UserAccessCode>(sql, new { Email = email, Otp = otp });
			if (userAccessCodeData == null)
			{
				return (null, "Invalid Otp");
			}
			var updateUserSql = @"update UserAccessCode set IsOtpVerified=1, OtpVerifiedOn=GETDATE(),UpdatedOn=GETDATE() where Email=@Email and ISNULL(FlagDeleted, 0) = 0";
			var uAData = await _connection.ExecuteAsync(updateUserSql, new { Email = email });
			if (uAData > 0)
			{
				//var langId = UserExtension.GetCurrentUserLanguageId();
				var langId = languageId;
				var accessSql = @"select * from Access_Code where AssignedTo=@AssignedTo and IsDelete=0";
				var acData = await _connection.QuerySingleOrDefaultAsync<AccessCodeMaster>(accessSql, new { AssignedTo = email });
				if (acData != null)
				{
					userAccessCodeData.AccessCode = acData.AccessCode;
					return (userAccessCodeData, "");
				}
				string aSql = @"select top 1 * from Access_Code where AccessCode like 'LS%' and AssignedTo is null and AccessCode_Status=0 order by AccessCode asc";
				var accessData = await _connection.QuerySingleOrDefaultAsync<AccessCodeMaster>(aSql);
				if (accessData == null)
					return (userAccessCodeData, "NoCode");

				var updateSql = @"update Access_Code set LanguageId=@LanguageId, AssignedTo=@AssignedTo where AccessCode_Id=@AccessCode_Id and IsDelete=0";

				var iData = await _connection.ExecuteAsync(updateSql, new { LanguageId = langId, AssignedTo = email, AccessCode_Id = accessData.AccessCode_Id });
				if (iData > 0)
				{
					userAccessCodeData.AccessCode = accessData.AccessCode;
				}
				return (userAccessCodeData, "");
			}
			else
				return (userAccessCodeData, "noUpdate");
		}
		public async Task<(UserAccessCode userAccessCode, string msg)> AccessCodeEmailOpt2(UserAccessCode model)
		{
			using var _connection = _dbContext.CreateConnection();
			var userSql = @"select count(*) from Users where EmailId=@EmailId and IsActive=1 and IsVerify=1 and flagdeleted=0";
			var userData = await _connection.QueryFirstOrDefaultAsync<int>(userSql, new { EmailId = model.Email });
			if (userData > 0)
			{
				return (null, "UserExist");
			}
			var userAccessCodeSql = @"select count(*) from Access_Code where AssignedTo=@EmailId and IsDelete=0";
			var userAccessCodeData = await _connection.QueryFirstOrDefaultAsync<int>(userAccessCodeSql, new { EmailId = model.Email });
			if (userAccessCodeData > 0)
			{
				return (null, "AccessCodeAssignORApproval");
			}
			string otp = new Random().Next(100000, 999999).ToString();
			model.Otp = otp;
			//var langId = UserExtension.GetCurrentUserLanguageId();
			var langId = model.LanguageId;
			var userAccessSql = @"select * from UserAccessCode where Email=@Email and ISNULL(FlagDeleted, 0) = 0";
			var userAData = await _connection.QuerySingleOrDefaultAsync<UserAccessCode>(userAccessSql, new { Email = model.Email });
			if (userAData == null)
			{
				var insertUserAccessSql = @"insert UserAccessCode(Email,Otp,IsOtpVerified,OtpSentOn,CreatedOn,OptionId,Message,LanguageId,FlagDeleted,OptionText)values(@Email,@Otp,0,GETDATE(),GETDATE(),@OptionId,@Message,@LanguageId,0,@OptionText)";
				var inserData = await _connection.ExecuteAsync(insertUserAccessSql, model);

				if (inserData > 0)
				{
					userAData = new UserAccessCode { Otp = otp };
				}
				return (userAData, "");
			}
			else if (userAData.IsRejected == true)
			{
				return (userAData, "requestRejected");
			}
			else if (userAData.OptionId != model.OptionId && !userAData.IsOtpVerified)
			{
				model.Id = userAData.Id;
				var updateUserAccessSql = @"update UserAccessCode set Otp=@Otp,IsOtpVerified=0,OtpSentOn=GETDATE(),CreatedOn=GETDATE(),OptionId=@OptionId,Message=@Message,LanguageId=@LanguageId,OptionText=@OptionText where Id=@Id and ISNULL(FlagDeleted, 0) = 0";
				var updateData = await _connection.ExecuteAsync(updateUserAccessSql, model);

				if (updateData > 0)
				{
					userAData = new UserAccessCode { Otp = otp, OptionId = model.OptionId };
				}
				return (userAData, "");
			}
			else if (model.ResendOtp)
			{
				var updateUserAccessSql = @"update UserAccessCode set Otp=@Otp,IsOtpVerified=0,OtpSentOn=GETDATE() where Id=@Id and ISNULL(FlagDeleted, 0) = 0";
				var updateData = await _connection.ExecuteAsync(updateUserAccessSql, new { Otp = otp, Id = userAData.Id });

				if (updateData > 0)
				{
					userAData = new UserAccessCode { Otp = otp };
				}
				return (userAData, "");
			}


			if (!userAData.IsOtpVerified && (userAData.OptionId == 2 || userAData.OptionId == 3 || userAData.OptionId == 1))
			{
				return (userAData, "verifyOtp");
			}
			else if (userAData.IsOtpVerified && (userAData.OptionId == 2 || userAData.OptionId == 3))
			{
				return (userAData, "requestedCode");
			}
			else if (userAData.IsOtpVerified && userAData.OptionId == 1)
			{
				return (userAData, "alreadyAssigned");
			}
			else
			{
				return (userAData, "");
			}
		}
		public async Task<(UserAccessCode userAccessCode, string error, string optionText)> VerifyRequestAccessCodeOtp(string email, string otp)
		{
			using var _connection = _dbContext.CreateConnection();
			string sql = @"select * from UserAccessCode where Email=@Email and Otp=@Otp and ISNULL(FlagDeleted, 0) = 0";
			var userAccessCodeData = await _connection.QuerySingleOrDefaultAsync<UserAccessCode>(sql, new { Email = email, Otp = otp });
			if (userAccessCodeData == null)
			{
				return (null, "InvalidOtp", "");
			}
			var updateUserSql = @"update UserAccessCode set IsOtpVerified=1, OtpVerifiedOn=GETDATE(),UpdatedOn=GETDATE() where Email=@Email and ISNULL(FlagDeleted, 0) = 0";
			var uAData = await _connection.ExecuteAsync(updateUserSql, new { Email = email });
			if (uAData > 0)
			{
				return (userAccessCodeData, userAccessCodeData.Message, userAccessCodeData.OptionText);
			}
			else
				return (userAccessCodeData, "noUpdate", "");
		}
		public async Task<(AccessCodeMaster? accessCodeMaster, string error)> AssignAccessCodeFromEmail(AccessCodeRequest accessCodeRequest)
		{
			using var _connection = _dbContext.CreateConnection();
			var userAccessSql = @"select * from UserAccessCode where Email=@Email and ISNULL(FlagDeleted, 0) = 0";
			UserAccessCode? usrAccessCodeData = await _connection.QueryFirstOrDefaultAsync<UserAccessCode>(userAccessSql, new { Email = accessCodeRequest.Email });

			//int expiryDays = 3;
			//if (!int.TryParse(ConfigurationManager.AppSettings["ExpiryDays"], out expiryDays))
			//{
			//	expiryDays = 3;
			//}

			if (usrAccessCodeData != null && usrAccessCodeData.OtpVerifiedOn != null)
			{
				var daysSinceVerification = (DateTime.Now - (DateTime)usrAccessCodeData.OtpVerifiedOn).TotalDays;
				if (daysSinceVerification > accessCodeRequest.ExpireDays)
				{
					return (null, "expired");
				}
				if (usrAccessCodeData.IsRejected == true)
				{
					//Enum.TryParse(Convert.ToString(usrAccessCodeData.LanguageId), out DisplayLanguage Alang);
					return (null, "alreadyRejected");
				}
			}

			string sql = @"select * from Access_Code where AssignedTo=@AssignedTo and IsDelete=0";
			var userAccessCodeData = await _connection.QuerySingleOrDefaultAsync<AccessCodeMaster>(sql, new { AssignedTo = accessCodeRequest.Email });
			if (userAccessCodeData != null)
			{
				//Enum.TryParse(Convert.ToString(userAccessCodeData.LanguageId), out DisplayLanguage Aslang);
				return (null, "alreadyAssigned");
			}

			//var langId = UserExtension.GetCurrentUserLanguageId();
			var langId = usrAccessCodeData?.LanguageId;
			//Enum.TryParse(Convert.ToString(usrAccessCodeData.LanguageId), out DisplayLanguage lang);
			if (accessCodeRequest.Type.Contains("RJ"))
			{
				if (usrAccessCodeData != null)
				{
					var updateUserAccessSql = @"update UserAccessCode set IsRejected=1,RejectedOn=GETDATE(),UpdatedOn=GETDATE() where Id=@Id";
					var updateUserAccessData = await _connection.ExecuteAsync(updateUserAccessSql, new { Id = usrAccessCodeData.Id });
					if (updateUserAccessData > 0)
					{
						return (null, "Rejected");
					}
				}
				return (null, "NotFound");
			}
			else
			{
				var aSql = @"select top 1 * from Access_Code where AccessCode like @Type and AssignedTo is null and AccessCode_Status=0 and IsDelete=0 order by AccessCode asc";
				var accessData = await _connection.QuerySingleOrDefaultAsync<AccessCodeMaster>(aSql, new { Type = $"{accessCodeRequest.Type}%" });
				if (accessData == null)
					return (null, "NoCode");

				var updateSql = @"update Access_Code set LanguageId=@LanguageId, AssignedTo=@AssignedTo where AccessCode_Id=@AccessCode_Id and IsDelete=0";

				var iData = await _connection.ExecuteAsync(updateSql, new { LanguageId = langId, AssignedTo = accessCodeRequest.Email, AccessCode_Id = accessData.AccessCode_Id });
				if (iData > 0)
				{
					userAccessCodeData = new AccessCodeMaster { AccessCode = accessData.AccessCode };
				}
				return (userAccessCodeData, "");
			}
		}
		public async Task<bool> UpdateUserPasswordAsync(ChangePassword changePassword)
		{
			using var _connection = _dbContext.CreateConnection();

			const string sql = "Update Users set Password=@Password, UpdatedBy=@UpdatedBy, UpdatedOn=GETDATE() WHERE userId=@UserId";
			var rowsAffected = await _connection.ExecuteAsync(sql, changePassword);
			return rowsAffected > 0;
		}
		public async Task<AccessCodeMaster?> GetAccessCodeAsync(string accessCode)
		{
			using var _connection = _dbContext.CreateConnection();

			const string sql = @"SELECT * FROM Access_Code WHERE AccessCode = @AccessCode AND IsDelete = 0 AND AccessCode_Status=0";
			var accessCodeData = await _connection.QueryFirstOrDefaultAsync<AccessCodeMaster>(sql, new { AccessCode = accessCode });
			if (accessCodeData == null) return null;
			return accessCodeData;
		}
		public async Task<LanguageMaster?> GetLanguageFromAccessCodeAsync(string accessCode)
		{
			using var _connection = _dbContext.CreateConnection();

			string sql = @"SELECT ln.LanguageId, ln.LanguageName, ln.LanguageCode 
							FROM Access_Code ac
							INNER JOIN Languages ln ON ac.LanguageId = ln.LanguageID AND ln.FlagDeleted=0
							WHERE AccessCode=@AccessCode AND ac.IsDelete=0";
			return (await _connection.QueryAsync<LanguageMaster>(sql, new { AccessCode = accessCode })).FirstOrDefault();
		}
		public async Task<bool> DeactivateUserAsync(int userId, string email)
		{
			using var _connection = _dbContext.CreateConnection();
			_connection.Open();
			using var transaction = _connection.BeginTransaction();
			try
			{
				const string sql = "Update Users set IsActive=0, FlagDeleted=1, DeletedOn=GETDATE(),UpdatedBy=@UpdatedBy,UpdatedOn=GETDATE(),IsExported=1 WHERE UserId=@UserId";

				const string userAccesssql = "Update UserAccessCode set FlagDeleted=1, DeletedOn=GETDATE() WHERE Email=@Email";

				const string accessCodesql = "Update Access_Code set IsDelete=1 WHERE AssignedTo=@AssignedTo";

				var rowsAffected = await _connection.ExecuteAsync(sql, new { UserId = userId, UpdatedBy = userId },transaction);

				await _connection.ExecuteAsync(userAccesssql, new { Email = email }, transaction);

				await _connection.ExecuteAsync(accessCodesql, new { AssignedTo = email }, transaction);

				transaction.Commit();
				return rowsAffected > 0;
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				return false;
			}
		}
		public async Task GenerateRefreshToken(RefreshTokenMaster refreshTokenMaster)
		{
			using var _connection = _dbContext.CreateConnection();
			var query = "UPDATE RefreshTokens set IsRevoked=1,RevokedAt=@RevokedAt WHERE UserId=@UserId AND IsUsed != 1;" +
				"INSERT INTO RefreshTokens (UserId, Token, CreatedByIp, ExpiresAt) VALUES (@UserId, @Token, @CreatedByIp, @ExpiresAt);";

			await _connection.ExecuteAsync(query, refreshTokenMaster);
		}
		public async Task<RefreshTokenMaster> GetRefreshToken(string refreshToken)
		{
			using var _connection = _dbContext.CreateConnection();
			DateTime curreTime = DateTime.UtcNow;
			var query = "SELECT * FROM RefreshTokens WHERE IsUsed=0 AND Token = @Token AND ExpiresAt > @CurreTime AND IsUsed=0";
			var parameters = new { Token = refreshToken, CurreTime = curreTime };
			return await _connection.QueryFirstOrDefaultAsync<RefreshTokenMaster>(query, parameters);
		}
		public async Task DeativeRefreshToken(string refreshToken)
		{
			using var _connection = _dbContext.CreateConnection();
			var query = "UPDATE RefreshTokens  set IsUsed=1 where Token = @Token";
			var parameters = new { Token = refreshToken };
			await _connection.ExecuteAsync(query, parameters);
		}
	}
}
