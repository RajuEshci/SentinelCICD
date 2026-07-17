IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserAccessCode')
BEGIN
	CREATE TABLE [dbo].[UserAccessCode](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Email] [nvarchar](255) NULL,
		[Otp] [nvarchar](255) NULL,
		[IsOtpVerified] [bit] NULL,
		[OtpSentOn] [datetime2](7) NULL,
		[OtpVerifiedOn] [datetime2](7) NULL,
		[CreatedOn] [datetime2](7) NULL,
		[OptionId] [int] NULL,
		[UpdatedOn] [datetime2](7) NULL,
		[Message] [nvarchar](max) NULL,
		[IsRejected] [bit] NULL,
		[RejectedOn] [datetime2](7) NULL,
		[LanguageId] [int] NULL,
		[FlagDeleted] [bit] NULL,
		[DeletedOn] [datetime2](7) NULL,
		[OptionText] [nvarchar](max) NULL,
	 CONSTRAINT [PK_UserAccessCode] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END