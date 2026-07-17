IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
BEGIN
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](250) NULL,
	[Gender] [varchar](20) NOT NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[Phonenumber] [nvarchar](30) NULL,
	[Mobilenumber] [nvarchar](30) NULL,
	[EmailId] [nvarchar](250) NOT NULL,
	[Address] [nvarchar](2000) NULL,
	[PostCode] [nvarchar](300) NULL,
	[Country] [varchar](50) NULL,
	[Password] [nvarchar](300) NULL,
	[Activationcode] [nvarchar](300) NULL,
	[Is2FactorAuthentication] [bit] NULL,
	[AuthenticationId] [int] NULL,
	[IsVerify] [bit] NULL,
	[IsEmailVerify] [bit] NULL,
	[IsMobileVerify] [bit] NULL,
	[IsActive] [bit] NULL,
	[IsSuspend] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedOn] [datetime] NULL,
	[IsTnCChecked] [bit] NULL,
	[ActivationCodeGenOn] [datetime] NULL,
	[IsConsent] [bit] NULL,
	[ConsentLabelId] [nvarchar](300) NULL,
	[SubscriptionId] [varchar](50) NULL,
	[SubscriptionUpdatedDate] [datetime] NULL,
	[Otp] [varchar](50) NULL,
	[OtpGenOn] [datetime] NULL,
	[OtpVerifiedOn] [datetime] NULL,
	[IsOtpVerify] [bit] NULL,
	[OtpAttempts] [int] NOT NULL,
	[AccessCode] [varchar](50) NULL,
	[IsAdmin] [bit] NULL,
	[flagdeleted] [bit] NULL,
	[deletedon] [datetime] NULL,
	[Age]  AS (case when [DateOfBirth] IS NULL then NULL else datediff(year,[DateOfBirth],getdate())-case when dateadd(year,datediff(year,[DateOfBirth],getdate()),[DateOfBirth])>getdate() then (1) else (0) end end),
	[IsConsentUpdated] [bit] NULL,
	[PreferredLanguageCode] [varchar](10) NULL,
	[PreferredLanguageId] [int] NULL,
	[IsExported] [bit] NULL,
	[Version] [nvarchar](250) NULL,
	[Model] [nvarchar](500) NULL,
 CONSTRAINT [PK_tbl_PublicUser] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsConsent]  DEFAULT ((0)) FOR [IsConsent]

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsActive]  DEFAULT ((1)) FOR [IsActive]

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_OtpAttempts]  DEFAULT ((3)) FOR [OtpAttempts]

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsAdmin]  DEFAULT ((0)) FOR [IsAdmin]

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsExported]  DEFAULT ((0)) FOR [IsExported]

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_flagdeleted]  DEFAULT ((0)) FOR [flagdeleted]

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_CreatedOn]  DEFAULT ((GETDATE())) FOR [CreatedOn]
END