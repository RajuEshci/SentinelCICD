IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InformativePages')
BEGIN
	CREATE TABLE [dbo].[InformativePages](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](255) NULL,
		[Description] [nvarchar](max) NULL,
		[Image] [nvarchar](max) NULL,
		[QuestionnaireId] [int] NULL,
		[AndriodUrl] [nvarchar](255) NULL,
		[IosUrl] [nvarchar](255) NULL,
		[SortOrder] [int] NULL,
		[ParentPageId] [int] NULL,
		[IsApplication] [bit] NULL,
		[IsActive] [bit] NULL,
		[FlagDeleted] [bit] NULL,
		[DeletedOn] [datetime] NULL,
		[CreatedBy] [int] NULL,
		[CreatedOn] [datetime2](7) NULL,
		[UpdatedBy] [int] NULL,
		[UpdatedOn] [datetime2](7) NULL,
		[PageCode] [varchar](50) NULL,
		[LanguageId] [int] NULL,
		[IsMenu] [int] NULL,
	 CONSTRAINT [PK_InformativePages] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
	 CONSTRAINT [IX_InformativePages] UNIQUE NONCLUSTERED 
	(
		[PageCode] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[InformativePages] ADD  CONSTRAINT [DF_flagdeleted_InformativePages]  DEFAULT ((0)) FOR [FlagDeleted]

	ALTER TABLE [dbo].[InformativePages] ADD  CONSTRAINT [DF_InformativePages_IsMenu]  DEFAULT ((0)) FOR [IsMenu]

END
