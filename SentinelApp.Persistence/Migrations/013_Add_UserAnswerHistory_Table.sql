IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserAnswerHistory')
BEGIN
	CREATE TABLE [dbo].[UserAnswerHistory](
		[UserAnswerHistoryId] [int] IDENTITY(1,1) NOT NULL,
		[UserAnswerId] [int] NULL,
		[UserId] [int] NULL,
		[QuestionId] [int] NULL,
		[OptionId] [int] NULL,
		[Answer] [nvarchar](max) NULL,
		[AnswerDate] [datetime2](7) NULL,
		[ActionType] [nvarchar](255) NULL,
		[ActionDate] [datetime2](7) NULL,
		[IsActive] [bit] NULL,
		[CreatedBy] [int] NULL,
		[CreatedOn] [datetime2](7) NULL,
		[UpdatedBy] [int] NULL,
		[UpdatedOn] [datetime2](7) NULL,
		[FlagDeleted] [bit] NULL,
		[DeletedOn] [datetime2](7) NULL,
	 CONSTRAINT [PK_UserAnswerHistory] PRIMARY KEY CLUSTERED 
	(
		[UserAnswerHistoryId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	
	ALTER TABLE [dbo].[UserAnswerHistory] ADD  CONSTRAINT [DF_UserAnswerHistory_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
	
	ALTER TABLE [dbo].[UserAnswerHistory] ADD  CONSTRAINT [DF_UserAnswerHistory_FlagDeleted]  DEFAULT ((0)) FOR [FlagDeleted]
	
END