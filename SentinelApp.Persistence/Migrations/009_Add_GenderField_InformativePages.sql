IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'InformativePages' AND COLUMN_NAME = 'Gender')
BEGIN
	alter table InformativePages add Gender nvarchar(20) NULL
END
GO