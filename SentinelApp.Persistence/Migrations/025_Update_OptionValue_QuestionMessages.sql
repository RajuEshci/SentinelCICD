IF EXISTS (SELECT 1 FROM QuestionMessages WHERE QuestionCode = 'TAMOXIFEN_OUTCOME_NO')
BEGIN
	update QuestionMessages set OptionValue='NOT_CONSIDERED',UpdatedOn=GETDATE() 
	where MessageId = (select MessageId from QuestionMessages where QuestionCode='TAMOXIFEN_OUTCOME_NO')
END