IF NOT EXISTS (
    SELECT 1
    FROM sys.tables
    WHERE name = 'QuestionLabelTranslations'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE [dbo].[QuestionLabelTranslations]
    (
        [QuestionLabelTranslationId] INT IDENTITY(1,1) NOT NULL,
        [QuestionId] INT NOT NULL,
        [LanguageId] INT NOT NULL,
        [LabelType] NVARCHAR(100) NOT NULL,
        [LabelText] NVARCHAR(MAX) NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_QuestionLabelTranslations_CreatedAt DEFAULT(GETUTCDATE()),
        [UpdatedAt] DATETIME2(7) NOT NULL CONSTRAINT DF_QuestionLabelTranslations_UpdatedAt DEFAULT(GETUTCDATE()),
        [CreatedBy] NVARCHAR(200) NOT NULL,
        [UpdatedBy] NVARCHAR(200) NULL,
        [flagDeleted] BIT NOT NULL CONSTRAINT DF_QuestionLabelTranslations_flagDeleted DEFAULT(0),
        [DeletedOn] DATETIME2(7) NULL,

        CONSTRAINT PK_QuestionLabelTranslations
            PRIMARY KEY CLUSTERED (QuestionLabelTranslationId),

        CONSTRAINT UQ_QuestionLabelTranslations
            UNIQUE (QuestionId, LanguageId, LabelType),

        CONSTRAINT FK_QuestionLabelTranslations_Questions
            FOREIGN KEY (QuestionId)
            REFERENCES Questions(QuestionId),

        CONSTRAINT FK_QuestionLabelTranslations_Languages
            FOREIGN KEY (LanguageId)
            REFERENCES Languages(LanguageId)
    );
END;
-- Insert Question Options
IF NOT EXISTS (
    SELECT 1
    FROM QuestionOptions
    WHERE QuestionCode = 'MRI_HISTORY'
      AND OptionValue = 'Normal'
)
BEGIN
    INSERT INTO QuestionOptions
    (QuestionCode, OptionValue, DisplayOrder, CreatedOn, CreatedBy, flagDeleted)
    VALUES ('MRI_HISTORY', 'Normal', 1, GETDATE(), '', 0);
END

IF NOT EXISTS (
    SELECT 1
    FROM QuestionOptions
    WHERE QuestionCode = 'MRI_HISTORY'
      AND OptionValue = 'Abnormal'
)
BEGIN
    INSERT INTO QuestionOptions
    (QuestionCode, OptionValue, DisplayOrder, CreatedOn, CreatedBy, flagDeleted)
    VALUES ('MRI_HISTORY', 'Abnormal', 2, GETDATE(), '', 0);
END

-- Insert Option Translations
INSERT INTO QuestionOptionTranslations
(
    OptionId,
    LanguageId,
    OptionText,
    CreatedAt,
    UpdatedAt,
    CreatedBy,
    flagDeleted
)
SELECT
    qo.OptionId,
    1,
    qo.OptionValue,
    GETDATE(),
    GETDATE(),
    '',
    0
FROM QuestionOptions qo
WHERE qo.QuestionCode = 'MRI_HISTORY'
  AND qo.OptionValue IN ('Normal', 'Abnormal')
  AND NOT EXISTS
  (
      SELECT 1
      FROM QuestionOptionTranslations qt
      WHERE qt.OptionId = qo.OptionId
        AND qt.LanguageId = 1
  );

-- Insert Question Label Translations
INSERT INTO QuestionLabelTranslations
(
    QuestionId,
    LanguageId,
    LabelType,
    LabelText,
    CreatedAt,
    UpdatedAt,
    CreatedBy,
    flagDeleted
)
SELECT
    q.QuestionId,
    1,
    v.LabelType,
    v.LabelText,
    GETUTCDATE(),
    GETUTCDATE(),
    '',
    0
FROM Questions q
CROSS APPLY
(
    VALUES
        ('monthyear', 'Please enter date of MRI'),
        ('dropdown', 'Please select result of MRI')
) v(LabelType, LabelText)
WHERE q.QuestionCode = 'MRI_HISTORY'
AND NOT EXISTS
(
    SELECT 1
    FROM QuestionLabelTranslations qt
    WHERE qt.QuestionId = q.QuestionId
      AND qt.LanguageId = 1
      AND qt.LabelType = v.LabelType
);

  INSERT INTO QuestionLabelTranslations
(
    QuestionId,
    LanguageId,
    LabelType,
    LabelText,
    CreatedAt,
    UpdatedAt,
    CreatedBy,
    flagDeleted
)
SELECT
    q.QuestionId,
    1,
    v.LabelType,
    v.LabelText,
    GETUTCDATE(),
    GETUTCDATE(),
    '',
    0
FROM Questions q
CROSS APPLY
(
    VALUES
        ('text', 'Name of cancer'),
        ('monthyear', 'Date of diagnosis')
) v(LabelType, LabelText)
WHERE q.QuestionCode = 'CANCER_HISTORY'
AND NOT EXISTS
(
    SELECT 1
    FROM QuestionLabelTranslations qt
    WHERE qt.QuestionId = q.QuestionId
      AND qt.LanguageId = 1
      AND qt.LabelType = v.LabelType
);

-- Update Question Type
UPDATE Questions
SET QuestionType = 'repeater_monthyear_dropdown'
WHERE QuestionCode = 'MRI_HISTORY'
  AND QuestionType <> 'repeater_monthyear_dropdown';
 
UPDATE Questions
SET QuestionType = 'repeater_text_monthyear'
WHERE QuestionCode = 'CANCER_HISTORY'
  AND QuestionType <> 'repeater_text_monthyear';


