IF NOT EXISTS (SELECT 1 FROM sys.triggers WHERE name = 'TR_Actions_History')
BEGIN
    EXEC('CREATE TRIGGER [dbo].[TR_Actions_History]
    ON [dbo].[Actions]
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
            INSERT INTO dbo.ActionHistory
            (
                ActionId,
                DueDate,
                Status,
                ColorCode,
                SortOrder,
                QuestionId,
                UserId,

                ActionType,
                ActionDate,

                FlagDeleted,
                DeletedOn,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn
            )
            SELECT
                i.ActionId,
                i.DueDate,
                i.Status,
                i.ColorCode,
                i.SortOrder,
                i.QuestionId,
                i.UserId,

                ''INSERT'',
                GETDATE(),

                i.FlagDeleted,
                i.DeletedOn,
                i.CreatedBy,
                i.CreatedOn,
                i.UpdatedBy,
                i.UpdatedOn
            FROM inserted i;
        END

        ---------------------------------------------------
        -- UPDATE
        ---------------------------------------------------
        ELSE IF EXISTS (SELECT 1 FROM inserted)
            AND EXISTS (SELECT 1 FROM deleted)
        BEGIN
            INSERT INTO dbo.ActionHistory
            (
                ActionId,
                DueDate,
                Status,
                ColorCode,
                SortOrder,
                QuestionId,
                UserId,

                ActionType,
                ActionDate,

                FlagDeleted,
                DeletedOn,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn
            )
            SELECT
                i.ActionId,
                i.DueDate,
                i.Status,
                i.ColorCode,
                i.SortOrder,
                i.QuestionId,
                i.UserId,

                CASE
                    WHEN ISNULL(d.FlagDeleted, 0) = 0
                     AND ISNULL(i.FlagDeleted, 0) = 1
                        THEN ''DELETE''
                    ELSE ''UPDATE''
                END,
                GETDATE(),

                i.FlagDeleted,
                i.DeletedOn,
                i.CreatedBy,
                i.CreatedOn,
                i.UpdatedBy,
                i.UpdatedOn
            FROM inserted i
            INNER JOIN deleted d
                ON i.ActionId = d.ActionId;
        END

        ---------------------------------------------------
        -- DELETE
        ---------------------------------------------------
        ELSE IF EXISTS (SELECT 1 FROM deleted)
            AND NOT EXISTS (SELECT 1 FROM inserted)
        BEGIN
            INSERT INTO dbo.ActionHistory
            (
                ActionId,
                DueDate,
                Status,
                ColorCode,
                SortOrder,
                QuestionId,
                UserId,

                ActionType,
                ActionDate,

                FlagDeleted,
                DeletedOn,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn
            )
            SELECT
                d.ActionId,
                d.DueDate,
                d.Status,
                d.ColorCode,
                d.SortOrder,
                d.QuestionId,
                d.UserId,

                ''DELETE'',
                GETDATE(),

                d.FlagDeleted,
                d.DeletedOn,
                d.CreatedBy,
                d.CreatedOn,
                d.UpdatedBy,
                d.UpdatedOn
            FROM deleted d;
        END
    END')
END