IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QuestionTranslations' AND COLUMN_NAME = 'ActionName')
BEGIN
	alter table QuestionTranslations
	add ActionName nvarchar(255)
END
GO

update QuestionTranslations set ActionName='Breast self-examination' where TranslationId=29
update QuestionTranslations set ActionName='Magnetic Resonance Imaging (MRI) scan of your breasts' where TranslationId=5
update QuestionTranslations set ActionName='Mammogram as well as MRI' where TranslationId=7
update QuestionTranslations set ActionName='MRI as well as Mammogram' where TranslationId=8
update QuestionTranslations set ActionName='Discuss risk-reducing mastectomy with a specialist surgeon' where TranslationId=16
update QuestionTranslations set ActionName='Discuss risk-reducing salpingo-oophorectomy with a gynaecologist' where TranslationId=20
update QuestionTranslations set ActionName='Discuss taking tamoxifen with a specialist' where TranslationId=24