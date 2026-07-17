IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'QuestionnaireId')
BEGIN
	alter table users add QuestionnaireId int
END
GO
update users set QuestionnaireId=1

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Consents' AND COLUMN_NAME = 'QuestionnaireId')
BEGIN
	alter table consents add QuestionnaireId int
	alter table consents drop column [Type]
END
GO
update Consents set QuestionnaireId=1