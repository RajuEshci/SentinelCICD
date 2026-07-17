IF NOT EXISTS (SELECT 1 FROM sys.triggers WHERE name = 'TR_UserAnswers_History')
BEGIN
    EXEC('CREATE TRIGGER [dbo].[TR_UserAnswers_History]
    ON [dbo].[UserAnswers]
    AFTER INSERT, UPDATE, DELETE
    AS
    BEGIN
        SET NOCOUNT ON;

        ---------------------------------------------------
        -- INSERT
        ---------------------------------------------------
        IF EXISTS (SELECT 1 FROM inserted)
           AND NOT EXISTS (SELECT 1 FROM deleted)
        BEGIN

            INSERT INTO dbo.UserAnswerHistory
            (
                UserAnswerId,
                UserId,
                QuestionId,
                OptionId,
                Answer,
                AnswerDate,

                ActionType,
                ActionDate,

                IsActive,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn,
                FlagDeleted,
                DeletedOn
            )
            SELECT
                i.UserAnswerId,
                i.UserId,
                i.QuestionId,
                i.OptionId,
                i.Answer,
                i.AnswerDate,

                ''INSERT'',
                GETDATE(),

                i.IsActive,
                i.CreatedBy,
                i.CreatedOn,
                i.UpdatedBy,
                i.UpdatedOn,
                i.FlagDeleted,
                i.DeletedOn
            FROM inserted i;
        END

        ---------------------------------------------------
        -- UPDATE
        ---------------------------------------------------
        ELSE IF EXISTS (SELECT 1 FROM inserted)
            AND EXISTS (SELECT 1 FROM deleted)
        BEGIN

            INSERT INTO dbo.UserAnswerHistory
            (
                UserAnswerId,
                UserId,
                QuestionId,
                OptionId,
                Answer,
                AnswerDate,

                ActionType,
                ActionDate,

                IsActive,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn,
                FlagDeleted,
                DeletedOn
            )
            SELECT
                i.UserAnswerId,
                i.UserId,
                i.QuestionId,
                i.OptionId,
                i.Answer,
                i.AnswerDate,

                ''UPDATE'',
                GETDATE(),

                i.IsActive,
                i.CreatedBy,
                i.CreatedOn,
                i.UpdatedBy,
                i.UpdatedOn,
                i.FlagDeleted,
                i.DeletedOn
            FROM inserted i;
        END

        ---------------------------------------------------
        -- DELETE
        ---------------------------------------------------
        ELSE IF EXISTS (SELECT 1 FROM deleted)
            AND NOT EXISTS (SELECT 1 FROM inserted)
        BEGIN

            INSERT INTO dbo.UserAnswerHistory
            (
                UserAnswerId,
                UserId,
                QuestionId,
                OptionId,
                Answer,
                AnswerDate,

                ActionType,
                ActionDate,

                IsActive,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn,
                FlagDeleted,
                DeletedOn
            )
            SELECT
                d.UserAnswerId,
                d.UserId,
                d.QuestionId,
                d.OptionId,
                d.Answer,
                d.AnswerDate,

                ''DELETE'',
                GETDATE(),

                d.IsActive,
                d.CreatedBy,
                d.CreatedOn,
                d.UpdatedBy,
                d.UpdatedOn,
                d.FlagDeleted,
                d.DeletedOn
            FROM deleted d;
        END
    END')
END