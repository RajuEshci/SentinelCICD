CREATE TABLE [dbo].[Versions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VersionNo] [varchar](50) NOT NULL,
	[IsForcefullyUpdate] [bit] NOT NULL,
 CONSTRAINT [PK_Versions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[VersionMessages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VersionId] [int] NOT NULL,
	[LanguageId] [int] NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_VersionMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VersionMessages] WITH CHECK ADD CONSTRAINT [FK_VersionMessages_Versions] FOREIGN KEY([VersionId])
REFERENCES [dbo].[Versions] ([Id])
GO

ALTER TABLE [dbo].[VersionMessages] CHECK CONSTRAINT [FK_VersionMessages_Versions]
GO

ALTER TABLE [dbo].[VersionMessages] WITH CHECK ADD CONSTRAINT [FK_VersionMessages_Languages] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Languages] ([LanguageId])
GO

ALTER TABLE [dbo].[VersionMessages] CHECK CONSTRAINT [FK_VersionMessages_Languages]
GO