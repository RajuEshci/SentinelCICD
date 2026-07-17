IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Access_Code')
BEGIN
	CREATE TABLE [dbo].[Access_Code](
		[AccessCode_Id] [int] IDENTITY(1,1) NOT NULL,
		[AccessCode] [varchar](50) NOT NULL,
		[AccessCode_Status] [int] NULL,
		[IsDelete] [bit] NOT NULL,
		[CreatedBy] [int] NULL,
		[CreatedOn] [datetime] NULL,
		[ModifiedBy] [int] NULL,
		[ModifiedOn] [datetime] NULL,
		[AssignedTo] [varchar](100) NULL,
		[AccessCodeTypeId] [bigint] NULL,
		[LanguageId] [int] NULL,
	 CONSTRAINT [PK_Access_Code] PRIMARY KEY CLUSTERED 
	(
		[AccessCode_Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Access_Code] ADD  CONSTRAINT [DF_Access_Code_IsDelete]  DEFAULT ((0)) FOR [IsDelete]

	ALTER TABLE [dbo].[Access_Code] ADD  CONSTRAINT [DF_Access_Code_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]

	ALTER TABLE [dbo].[Access_Code] ADD  CONSTRAINT [DF_Access_Code_LanguageId]  DEFAULT ((1)) FOR [LanguageId]

	-- INSERT Access codes--
	DECLARE @i INT = 1;

	WHILE @i <= 100
	BEGIN
		INSERT INTO Access_Code(AccessCode,AccessCode_Status,IsDelete,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,AssignedTo,AccessCodeTypeId,LanguageId)
		VALUES
		(
			'LS' + RIGHT('000' + CAST(@i AS VARCHAR(3)), 3),
			0,
			0,
			NULL,
			GETDATE(),
			NULL,
			NULL,
			NULL,
			2,
			1
		);

		SET @i = @i + 1;
	END;
END