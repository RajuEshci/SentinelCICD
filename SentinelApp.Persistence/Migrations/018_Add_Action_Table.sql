IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Actions')
BEGIN
	CREATE TABLE [dbo].[Actions](
		[ActionId] [int] IDENTITY(1,1) NOT NULL,
		[DueDate] [datetime2](7) NULL,
		[Status] [nvarchar](255) NULL,
		[ColorCode] [nvarchar](255) NULL,
		[SortOrder] [int] NULL,
		[QuestionId] [int] NULL,
		[UserId] [int] NULL,
		[FlagDeleted] [bit] NULL,
		[DeletedOn] [datetime2](7) NULL,
		[CreatedBy] [int] NULL,
		[CreatedOn] [datetime2](7) NULL,
		[UpdatedBy] [int] NULL,
		[UpdatedOn] [datetime2](7) NULL,
	 CONSTRAINT [PK_Actions] PRIMARY KEY CLUSTERED 
	(
		[ActionId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Actions] ADD  CONSTRAINT [DF_Actions_FlagDeleted]  DEFAULT ((0)) FOR [FlagDeleted]
	
	ALTER TABLE [dbo].[Actions] ADD  CONSTRAINT [DF_Actions_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
	
END
