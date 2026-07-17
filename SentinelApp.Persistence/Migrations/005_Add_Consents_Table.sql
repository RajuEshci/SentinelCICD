IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Consents')
BEGIN
	CREATE TABLE [dbo].[Consents](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Description] [nvarchar](max) NULL,
		[Type] [nvarchar](max) NULL,
		[FlagDeleted] [bit] NULL,
		[CreatedBy] [int] NULL,
		[CreatedOn] [datetime2](7) NULL,
		[UpdatedBy] [int] NULL,
		[UpdatedOn] [datetime2](7) NULL,
		[LinkText] [nvarchar](max) NULL,
	 CONSTRAINT [PK_Consents] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[Consents] ADD  CONSTRAINT [DF_Consents_FlagDeleted]  DEFAULT ((0)) FOR [FlagDeleted]

	---INSERT---
	insert into consents(Description,createdOn,Linktext,[Type])
	values('I confirm that I have read and understand the App Information Sheet for the app and have had the opportunity to ask questions.',GETDATE(),'Click here to review App Information Sheet','BRCA'),
	('I understand that at all times the app will comply with the General Data Protection Regulations (GDPR, 2018) approved by the EU parliament and UK equivalent',GETDATE(),'Click here to review App GDPR Privacy Notice','BRCA'),
	('I understand that my participation in the use of the BRCA app is voluntary and that I am free to withdraw at any time, without giving reason.',GETDATE(),null,'BRCA'),
	('I grant consent for Instant Access Medical UK Limited to hold my personal data that I enter in this BRCA app on condition that my data will be confidential and will not be shared with anyone without my explicit consent.',GETDATE(),null,'BRCA'),
	('I agree that data collected about me, for this project may be stored by Instant Access Medical securely with Amazon Web Services (AWS) in the UK.',GETDATE(),null,'BRCA'),
	('I consent for the data in my personal dashboard to be used to audit the effectiveness of the app on condition that my data will be confidential and will not be shared with anyone without my explicit consent.',GETDATE(),null,'BRCA'),
	('I agree to being asked to complete surveys from time to time on my use of the app. I understand that completion of any surveys is voluntary and has no impact on my use of the app',GETDATE(),null,'BRCA'),
	('I consent to receive updates about national changes to BRCA management guidelines',GETDATE(),null,'BRCA'),
	('I consent to receive updates about BRCA related research that has been nationally approved by ethical committee',GETDATE(),null,'BRCA')
END