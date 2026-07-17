IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserConsentAnswers')
BEGIN
	CREATE TABLE [dbo].[UserConsentAnswers](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[ConsentId] [int] NULL,
		[UserId] [int] NULL,
		[Answer] [nvarchar](100) NULL,
		[FlagDeleted] [bit] NULL,
		[CreatedOn] [datetime2](7) NULL,
		[CreatedBy] [int] NULL,
		[UpdatedBy] [int] NULL,
		[UpdatedOn] [datetime2](7) NULL,
	 CONSTRAINT [PK_UserConsentAnswers] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[UserConsentAnswers] ADD  CONSTRAINT [DF_UserConsentAnswers_FlagDeleted]  DEFAULT ((0)) FOR [FlagDeleted]

END