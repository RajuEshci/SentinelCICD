IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserAnswers')
BEGIN
	CREATE TABLE [dbo].[UserAnswers](
	[UserAnswerId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[QuestionId] [int] NULL,
	[OptionId] [int] NULL,
	[Answer] [nvarchar](max) NULL,
	[AnswerDate] [datetime2](7) NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedOn] [datetime2](7) NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedOn] [datetime2](7) NULL,
	[FlagDeleted] [bit] NULL,
	[DeletedOn] [datetime2](7) NULL,
	 CONSTRAINT [PK_UserAnswers] PRIMARY KEY CLUSTERED 
	(
		[UserAnswerId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]	

	ALTER TABLE [dbo].[UserAnswers] ADD  CONSTRAINT [DF_UserAnswers_IsActive]  DEFAULT ((1)) FOR [IsActive]	

	ALTER TABLE [dbo].[UserAnswers] ADD  CONSTRAINT [DF_UserAnswers_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]	

	ALTER TABLE [dbo].[UserAnswers] ADD  CONSTRAINT [DF_UserAnswers_FlagDeleted]  DEFAULT ((0)) FOR [FlagDeleted]
	
	ALTER TABLE [dbo].[UserAnswers]  WITH CHECK ADD  CONSTRAINT [FK_UserAnswers_QuestionOptions] FOREIGN KEY([OptionId]) REFERENCES [dbo].[QuestionOptions] ([OptionId])
	
	ALTER TABLE [dbo].[UserAnswers] CHECK CONSTRAINT [FK_UserAnswers_QuestionOptions]	

	ALTER TABLE [dbo].[UserAnswers]  WITH CHECK ADD  CONSTRAINT [FK_UserAnswers_Questions] FOREIGN KEY([QuestionId])
	REFERENCES [dbo].[Questions] ([QuestionId])
	
	ALTER TABLE [dbo].[UserAnswers] CHECK CONSTRAINT [FK_UserAnswers_Questions]	

	ALTER TABLE [dbo].[UserAnswers]  WITH CHECK ADD  CONSTRAINT [FK_UserAnswers_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[Users] ([UserId])
	
	ALTER TABLE [dbo].[UserAnswers] CHECK CONSTRAINT [FK_UserAnswers_Users]
	
END