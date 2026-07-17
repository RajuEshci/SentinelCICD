UPDATE qt
SET QuestionText =
    CASE
        WHEN q.QuestionCode IN (
            'REPRODUCTIVE_NO_REASON',
            'MRI_HISTORY',
            'MASTECTOMY_OUTCOME',
            'MASTECTOMY_OUTCOME_NO',
            'SALPINGO_OUTCOME',
            'SALPINGO_OUTCOME_NO',
            'TAMOXIFEN_OUTCOME',
            'TAMOXIFEN_OUTCOME_NO',
            'BREAST_SELF_EXAM_ACTION'
        )
        THEN ''

        WHEN q.QuestionCode = 'SURGERY_TYPE'
        THEN 'Surgery you had'
    END
FROM Questions q
INNER JOIN QuestionTranslations qt
    ON qt.QuestionId = q.QuestionId
WHERE q.QuestionCode IN (
    'REPRODUCTIVE_NO_REASON',
    'MRI_HISTORY',
    'MASTECTOMY_OUTCOME',
    'MASTECTOMY_OUTCOME_NO',
    'SALPINGO_OUTCOME',
    'SALPINGO_OUTCOME_NO',
    'TAMOXIFEN_OUTCOME',
    'TAMOXIFEN_OUTCOME_NO',
    'BREAST_SELF_EXAM_ACTION',
    'SURGERY_TYPE'
);