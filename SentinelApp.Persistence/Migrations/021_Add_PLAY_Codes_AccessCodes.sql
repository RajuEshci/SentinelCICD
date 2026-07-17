-- INSERT Access codes--
DECLARE @i INT = 1;

WHILE @i <= 100
BEGIN
	INSERT INTO Access_Code(AccessCode,AccessCode_Status,IsDelete,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,AssignedTo,AccessCodeTypeId,LanguageId)
	VALUES
	(
		'PLAY' + RIGHT('000' + CAST(@i AS VARCHAR(3)), 3),
		0,
		0,
		NULL,
		GETDATE(),
		NULL,
		NULL,
		NULL,
		1,
		1
	);

	SET @i = @i + 1;
END;