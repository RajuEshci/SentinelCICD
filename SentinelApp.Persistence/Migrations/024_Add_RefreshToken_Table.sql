IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RefreshTokens')
BEGIN
	CREATE TABLE [dbo].[RefreshTokens](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[UserId] [int] NOT NULL,
		[Token] [nvarchar](450) NOT NULL,
		[ExpiresAt] [datetime] NOT NULL,
		[RevokedAt] [datetime] NULL,
		[CreatedAt] [datetime] NOT NULL,
		[CreatedByIp] [nvarchar](45) NOT NULL,
		[RevokedByIp] [nvarchar](45) NULL,
		[ReplacedByToken] [nvarchar](450) NULL,
		[IsRevoked] [bit] NOT NULL,
		[IsUsed] [bit] NOT NULL,
	 CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]	

	ALTER TABLE [dbo].[RefreshTokens] ADD  CONSTRAINT [DF_RefreshTokens_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]

	ALTER TABLE [dbo].[RefreshTokens] ADD  CONSTRAINT [DF_RefreshTokens_CreatedByIp]  DEFAULT ('Unknown') FOR [CreatedByIp]

	ALTER TABLE [dbo].[RefreshTokens] ADD  CONSTRAINT [DF_RefreshTokens_RevokedByIp]  DEFAULT ('Unknown') FOR [RevokedByIp]

	ALTER TABLE [dbo].[RefreshTokens] ADD  CONSTRAINT [DF_RefreshTokens_IsRevoked]  DEFAULT ((0)) FOR [IsRevoked]

	ALTER TABLE [dbo].[RefreshTokens] ADD  CONSTRAINT [DF_RefreshTokens_IsUsed]  DEFAULT ((0)) FOR [IsUsed]

END
