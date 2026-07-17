IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Languages')
BEGIN
    CREATE TABLE Languages(
        LanguageId INT IDENTITY(1,1) PRIMARY KEY,
        LanguageCode VARCHAR(10) NOT NULL,
        LanguageName NVARCHAR(100) NOT NULL,
        CreatedOn DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedOn DATETIME2 NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL,
        CONSTRAINT UQ_Languages_LanguageCode UNIQUE (LanguageCode)
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Questionnaires')
BEGIN
    CREATE TABLE Questionnaires(
        QuestionnaireId INT IDENTITY(1,1) PRIMARY KEY,
        QuestionnaireName NVARCHAR(200) NOT NULL,
        CreatedOn DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedOn DATETIME2 NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'QuestionGroups')
BEGIN
    CREATE TABLE QuestionGroups(
        GroupId INT IDENTITY(1,1) PRIMARY KEY,
        QuestionnaireId INT NOT NULL,
        GroupName NVARCHAR(200) NOT NULL,
        DisplayOrder INT NOT NULL,
        Description NVARCHAR(MAX) NULL,
        CreatedOn DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedOn DATETIME2 NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Questions')
BEGIN
    CREATE TABLE Questions(
        QuestionId INT IDENTITY(1,1) PRIMARY KEY,
        QuestionnaireId INT NOT NULL,
        GroupId INT NOT NULL,
        QuestionCode VARCHAR(100) NOT NULL,
        QuestionType VARCHAR(50) NOT NULL,
        IsRequired BIT NOT NULL DEFAULT(0),
        IsMain BIT NOT NULL DEFAULT(1),
        DisplayOrder INT NOT NULL,
        CreatedOn DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedOn DATETIME2 NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'QuestionOptions')
BEGIN
    CREATE TABLE QuestionOptions(
        OptionId INT IDENTITY(1,1) PRIMARY KEY,
        QuestionCode VARCHAR(100) NOT NULL,
        OptionValue VARCHAR(100) NOT NULL,
        DisplayOrder INT NOT NULL,
        CreatedOn DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedOn DATETIME2 NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'QuestionDependencies')
BEGIN
    CREATE TABLE QuestionDependencies(
        DependencyId INT IDENTITY(1,1) PRIMARY KEY,
        ParentQuestionCode VARCHAR(100) NOT NULL,
        ParentOptionValue VARCHAR(100) NOT NULL,
        ChildQuestionCode VARCHAR(100) NOT NULL,
        CreatedOn DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedOn DATETIME2 NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'QuestionMessages')
BEGIN
    CREATE TABLE QuestionMessages(
        MessageId INT IDENTITY(1,1) PRIMARY KEY,
        QuestionCode VARCHAR(100) NOT NULL,
        OptionValue VARCHAR(100) NULL,
        CreatedOn DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedOn DATETIME2 NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'QuestionTranslations')
BEGIN
    CREATE TABLE QuestionTranslations(
        TranslationId INT IDENTITY(1,1) PRIMARY KEY,
        QuestionId INT NOT NULL,
        LanguageId INT NOT NULL,
        QuestionText NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL,
        FOREIGN KEY (QuestionId) REFERENCES Questions(QuestionId),
        FOREIGN KEY (LanguageId) REFERENCES Languages(LanguageId),
        CONSTRAINT UQ_QuestionTranslations UNIQUE (QuestionId, LanguageId)
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'QuestionOptionTranslations')
BEGIN
    CREATE TABLE QuestionOptionTranslations(
        OptionTranslationId INT IDENTITY(1,1) PRIMARY KEY,
        OptionId INT NOT NULL,
        LanguageId INT NOT NULL,
        OptionText NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL,
        FOREIGN KEY (OptionId) REFERENCES QuestionOptions(OptionId),
        FOREIGN KEY (LanguageId) REFERENCES Languages(LanguageId),
        CONSTRAINT UQ_QuestionOptionTranslations UNIQUE (OptionId, LanguageId)
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'QuestionMessageTranslations')
BEGIN
    CREATE TABLE QuestionMessageTranslations(
        MessageTranslationId INT IDENTITY(1,1) PRIMARY KEY,
        MessageId INT NOT NULL,
        LanguageId INT NOT NULL,
        MessageText NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(200) NOT NULL,
        UpdatedBy NVARCHAR(200) NULL,
        flagDeleted BIT NOT NULL DEFAULT(0),
        DeletedOn DATETIME2 NULL,
        FOREIGN KEY (MessageId) REFERENCES QuestionMessages(MessageId),
        FOREIGN KEY (LanguageId) REFERENCES Languages(LanguageId),
        CONSTRAINT UQ_QuestionMessageTranslations UNIQUE (MessageId, LanguageId)
    );
END

/* ===================== DATA ===================== */

/* LANGUAGES (LanguageId 1 = English) */
INSERT INTO Languages(LanguageCode, LanguageName,CreatedBy) VALUES ('en', 'English',0);

/* QUESTIONNAIRE */
INSERT INTO Questionnaires(QuestionnaireName,CreatedBy) VALUES ('BRCA Assessment',0);

/* GROUPS */
INSERT INTO QuestionGroups(QuestionnaireId, GroupName, DisplayOrder, Description,CreatedBy) VALUES
(1, 'Diagnosis', 1, NULL,0),
(1, 'Screening', 2, 'Part of your program of management of your condition is to have regular breast screening.',0);

/* ==================== DIAGNOSIS GROUP (GroupId = 1) ==================== */

INSERT INTO Questions(QuestionnaireId,GroupId,QuestionCode,QuestionType,IsRequired,IsMain,DisplayOrder,CreatedBy) VALUES
(1,1,'DIAGNOSIS_DATE','monthyear',1,1,1,0),
(1,1,'DIAGNOSIS_METHOD','radio',1,1,2,0),
(1,1,'CANCER_HISTORY','repeater',0,1,3,0),
(1,1,'GENETIC_TYPE','radio',1,1,4,0);

/* ==================== SCREENING GROUP (GroupId = 2) ==================== */

INSERT INTO Questions(QuestionnaireId,GroupId,QuestionCode,QuestionType,IsRequired,IsMain,DisplayOrder,CreatedBy) VALUES
(1,2,'MRI_SCAN','radio',1,1,5,0),
(1,2,'MRI_HISTORY','repeater',0,0,6,0),
(1,2,'MAMMOGRAM_WITH_MRI','radio',0,1,7,0),
(1,2,'MRI_WITH_MAMMOGRAM','radio',0,1,8,0),
(1,2,'RED_FLAGS','radio',0,1,9,0),
(1,2,'SUPPORT_ACCESS','radio',0,1,10,0),
(1,2,'REPRODUCTIVE_OPTIONS','radio',0,1,11,0),
(1,2,'REPRODUCTIVE_NO_REASON','radio',0,0,12,0),
(1,2,'PREVENTIVE_SURGERY','radio',0,1,13,0),
(1,2,'SURGERY_TYPE','dropdown',0,0,14,0),
(1,2,'SURGERY_DATE','monthyear',0,0,15,0),
(1,2,'MASTECTOMY_DISCUSSION','radio',0,1,16,0),
(1,2,'MASTECTOMY_OUTCOME','radio',0,0,17,0),
(1,2,'MASTECTOMY_OUTCOME_NO','radio',0,0,18,0),
(1,2,'MASTECTOMY_DATE','monthyear',0,0,19,0),
(1,2,'SALPINGO_DISCUSSION','radio',0,1,20,0),
(1,2,'SALPINGO_OUTCOME','radio',0,0,21,0),
(1,2,'SALPINGO_OUTCOME_NO','radio',0,0,22,0),
(1,2,'SALPINGO_DATE','monthyear',0,0,23,0),
(1,2,'TAMOXIFEN_DISCUSSION','radio',0,1,24,0),
(1,2,'TAMOXIFEN_OUTCOME','radio',0,0,25,0),
(1,2,'TAMOXIFEN_OUTCOME_NO','radio',0,0,26,0),
(1,2,'TAMOXIFEN_START','monthyear',0,0,27,0),
(1,2,'TAMOXIFEN_STOP','monthyear',0,0,28,0),
(1,2,'BREAST_SELF_EXAM','radio',0,1,29,0),
(1,2,'BREAST_SELF_EXAM_ACTION','radio',0,0,30,0),
(1,2,'FAMILY_NOTIFICATION','radio',0,1,31,0);

/* ==================== OPTIONS ==================== */

INSERT INTO QuestionOptions(QuestionCode,OptionValue,DisplayOrder,CreatedBy) VALUES
/* DIAGNOSIS_METHOD */
('DIAGNOSIS_METHOD','OWN_CANCER',1,0),
('DIAGNOSIS_METHOD','FAMILY_MEMBER',2,0),

/* GENETIC_TYPE */
('GENETIC_TYPE','BRCA1',1,0),
('GENETIC_TYPE','BRCA2',2,0),
('GENETIC_TYPE','PALB2',3,0),

/* MRI_SCAN */
('MRI_SCAN','YES',1,0),
('MRI_SCAN','NO',2,0),

/* MAMMOGRAM_WITH_MRI */
('MAMMOGRAM_WITH_MRI','YES',1,0),
('MAMMOGRAM_WITH_MRI','NO',2,0),

/* MRI_WITH_MAMMOGRAM */
('MRI_WITH_MAMMOGRAM','YES',1,0),
('MRI_WITH_MAMMOGRAM','NO',2,0),

/* RED_FLAGS */
('RED_FLAGS','YES',1,0),
('RED_FLAGS','NO',2,0),

/* SUPPORT_ACCESS */
('SUPPORT_ACCESS','YES',1,0),
('SUPPORT_ACCESS','NO',2,0),
('SUPPORT_ACCESS','PLAN',3,0),

/* REPRODUCTIVE_OPTIONS */
('REPRODUCTIVE_OPTIONS','YES',1,0),
('REPRODUCTIVE_OPTIONS','NO',2,0),

/* REPRODUCTIVE_NO_REASON */
('REPRODUCTIVE_NO_REASON','PLAN_GP',1,0),
('REPRODUCTIVE_NO_REASON','NOT_CONSIDERED',2,0),
('REPRODUCTIVE_NO_REASON','NOT_NEEDED',3,0),
('REPRODUCTIVE_NO_REASON','PREVENTIVE_SURGERY',4,0),

/* PREVENTIVE_SURGERY */
('PREVENTIVE_SURGERY','YES',1,0),
('PREVENTIVE_SURGERY','NO',2,0),
('PREVENTIVE_SURGERY','DONT_KNOW',3,0),

/* SURGERY_TYPE */
('SURGERY_TYPE','MASTECTOMY',1,0),
('SURGERY_TYPE','SALPINGO',2,0),

/* MASTECTOMY_DISCUSSION */
('MASTECTOMY_DISCUSSION','YES',1,0),
('MASTECTOMY_DISCUSSION','NO',2,0),

/* MASTECTOMY_OUTCOME (when discussion = YES) */
('MASTECTOMY_OUTCOME','PLAN',1,0),
('MASTECTOMY_OUTCOME','DECLINED',2,0),
('MASTECTOMY_OUTCOME','NOT_RECOMMENDED',3,0),
('MASTECTOMY_OUTCOME','HAD_SURGERY',4,0),

/* MASTECTOMY_OUTCOME_NO (when discussion = NO) */
('MASTECTOMY_OUTCOME_NO','PLAN_DISCUSS',1,0),
('MASTECTOMY_OUTCOME_NO','NO_PLAN',2,0),
('MASTECTOMY_OUTCOME_NO','HAD_SURGERY',3,0),
('MASTECTOMY_OUTCOME_NO','NOT_CONSIDERED',4,0),

/* SALPINGO_DISCUSSION */
('SALPINGO_DISCUSSION','YES',1,0),
('SALPINGO_DISCUSSION','NO',2,0),

/* SALPINGO_OUTCOME (when discussion = YES) */
('SALPINGO_OUTCOME','PLAN_OPERATION',1,0),
('SALPINGO_OUTCOME','DECLINED',2,0),
('SALPINGO_OUTCOME','NOT_RECOMMENDED',3,0),
('SALPINGO_OUTCOME','HAD_OPERATION',4,0),

/* SALPINGO_OUTCOME_NO (when discussion = NO) */
('SALPINGO_OUTCOME_NO','PLAN_DISCUSS',1,0),
('SALPINGO_OUTCOME_NO','NO_PLAN',2,0),
('SALPINGO_OUTCOME_NO','NOT_CONSIDERED',3,0),

/* TAMOXIFEN_DISCUSSION */
('TAMOXIFEN_DISCUSSION','YES',1,0),
('TAMOXIFEN_DISCUSSION','NO',2,0),

/* TAMOXIFEN_OUTCOME (when discussion = YES) */
('TAMOXIFEN_OUTCOME','PLAN_START',1,0),
('TAMOXIFEN_OUTCOME','DECLINED',2,0),
('TAMOXIFEN_OUTCOME','STARTED',3,0),

/* TAMOXIFEN_OUTCOME_NO (when discussion = NO) */
('TAMOXIFEN_OUTCOME_NO','PLAN_DISCUSS',1,0),
('TAMOXIFEN_OUTCOME_NO','NOT_CONSIDERED',2,0),

/* BREAST_SELF_EXAM */
('BREAST_SELF_EXAM','YES',1,0),
('BREAST_SELF_EXAM','NO',2,0),
('BREAST_SELF_EXAM','DONT_KNOW',3,0),

/* BREAST_SELF_EXAM_ACTION */
('BREAST_SELF_EXAM_ACTION','PLAN_DISCUSS',1,0),
('BREAST_SELF_EXAM_ACTION','NO_PLAN',2,0),
('BREAST_SELF_EXAM_ACTION','MASTECTOMY',3,0),

/* FAMILY_NOTIFICATION */
('FAMILY_NOTIFICATION','NO_FAMILY',1,0),
('FAMILY_NOTIFICATION','NOTIFIED',2,0),
('FAMILY_NOTIFICATION','CONSIDERED',3,0),
('FAMILY_NOTIFICATION','PLAN',4,0),
('FAMILY_NOTIFICATION','NOT_CONSIDERED',5,0),
('FAMILY_NOTIFICATION','NO_INTENT',6,0);

/* ==================== DEPENDENCIES ==================== */

INSERT INTO QuestionDependencies(ParentQuestionCode,ParentOptionValue,ChildQuestionCode,CreatedBy) VALUES
/* MRI */
('MRI_SCAN','YES','MRI_HISTORY',0),

/* Reproductive */
('REPRODUCTIVE_OPTIONS','NO','REPRODUCTIVE_NO_REASON',0),
('REPRODUCTIVE_NO_REASON','PREVENTIVE_SURGERY','SURGERY_TYPE',0),
('REPRODUCTIVE_NO_REASON','PREVENTIVE_SURGERY','SURGERY_DATE',0),

/* Preventive surgery */
('PREVENTIVE_SURGERY','YES','SURGERY_TYPE',0),
('PREVENTIVE_SURGERY','YES','SURGERY_DATE',0),

/* Mastectomy discussion */
('MASTECTOMY_DISCUSSION','YES','MASTECTOMY_OUTCOME',0),
('MASTECTOMY_DISCUSSION','NO','MASTECTOMY_OUTCOME_NO',0),
('MASTECTOMY_OUTCOME','HAD_SURGERY','MASTECTOMY_DATE',0),
('MASTECTOMY_OUTCOME_NO','HAD_SURGERY','MASTECTOMY_DATE',0),

/* Salpingo discussion */
('SALPINGO_DISCUSSION','YES','SALPINGO_OUTCOME',0),
('SALPINGO_DISCUSSION','NO','SALPINGO_OUTCOME_NO',0),
('SALPINGO_OUTCOME','HAD_OPERATION','SALPINGO_DATE',0),
('SALPINGO_OUTCOME_NO','HAD_OPERATION','SALPINGO_DATE',0),

/* Tamoxifen discussion */
('TAMOXIFEN_DISCUSSION','YES','TAMOXIFEN_OUTCOME',0),
('TAMOXIFEN_DISCUSSION','NO','TAMOXIFEN_OUTCOME_NO',0),
('TAMOXIFEN_OUTCOME','STARTED','TAMOXIFEN_START',0),
('TAMOXIFEN_OUTCOME','STARTED','TAMOXIFEN_STOP',0),

/* Breast self-exam */
('BREAST_SELF_EXAM','NO','BREAST_SELF_EXAM_ACTION',0),
('BREAST_SELF_EXAM','DONT_KNOW','BREAST_SELF_EXAM_ACTION',0);


/* ==================== MESSAGES ==================== */

INSERT INTO QuestionMessages(QuestionCode,OptionValue,CreatedBy) VALUES
('MRI_SCAN','NO',0),
('PREVENTIVE_SURGERY','DONT_KNOW',0),
('MASTECTOMY_OUTCOME_NO','NOT_CONSIDERED',0),
('SALPINGO_OUTCOME_NO','NOT_CONSIDERED',0),
('TAMOXIFEN_OUTCOME_NO',NULL,0),
('BREAST_SELF_EXAM','NO',0),
('BREAST_SELF_EXAM','DONT_KNOW',0);

/* ==================== TRANSLATIONS (English - LanguageId 1) ==================== */

/* Question translations */
INSERT INTO QuestionTranslations(QuestionId,LanguageId,QuestionText,CreatedBy) VALUES
/* Diagnosis group */
(1,1,'Date of diagnosis of BRCA gene carrier',0),
(2,1,'How was BRCA gene carrier diagnosed',0),
(3,1,'Name of any and all cancers diagnosed and date of diagnosis',0),
(4,1,'Genetic Type of BRCA gene carrier',0),
/* Screening group */
(5,1,'Have you had a Magnetic Resonance Imaging (MRI) scan of your breasts?',0),
(6,1,'Please enter date of MRI and result',0),
(7,1,'Has a radiologist advised you to have a mammogram as well as an MRI',0),
(8,1,'Has a radiologist advised you to have an MRI as well as a mammogram',0),
(9,1,'Are you aware of the symptoms to look out for e.g. red flags',0),
(10,1,'Do you have access to additional support if required',0),
(11,1,'Have you discussed reproductive options with your GP?',0),
(12,1,'If No, which of the following apply',0),
(13,1,'Have you had preventive surgery for BRCA?',0),
(14,1,'Please enter the surgery you had',0),
(15,1,'Date of surgery',0),
(16,1,'Have you discussed risk-reducing mastectomy with a specialist surgeon?',0),
(17,1,'Please select which applies',0),
(18,1,'Please select which applies',0),
(19,1,'Date of risk reducing mastectomy',0),
(20,1,'Have you discussed risk-reducing salpingo-oophorectomy with a gynaecologist?',0),
(21,1,'Please select which applies',0),
(22,1,'Please select which applies',0),
(23,1,'Date of risk reducing salpingo-oophorectomy',0),
(24,1,'Have you discussed taking tamoxifen with a specialist',0),
(25,1,'Please select which applies',0),
(26,1,'Please select which applies',0),
(27,1,'Date started tamoxifen',0),
(28,1,'Date stopped tamoxifen',0),
(29,1,'Are you confident about breast self-examination?',0),
(30,1,'Please select which applies',0),
(31,1,'Notifying close family members - which applies?',0);

/* Option translations */
INSERT INTO QuestionOptionTranslations(OptionId,LanguageId,OptionText,CreatedBy) VALUES
(1,1,'Own cancer',0),
(2,1,'Family member had cancer',0),
(3,1,'BRCA1',0),
(4,1,'BRCA2',0),
(5,1,'PALB2',0),
(6,1,'Yes',0),
(7,1,'No',0),
(8,1,'Yes',0),
(9,1,'No',0),
(10,1,'Yes',0),
(11,1,'No',0),
(12,1,'Yes',0),
(13,1,'No',0),
(14,1,'Yes',0),
(15,1,'No',0),
(16,1,'I plan to access additional support',0),
(17,1,'Yes',0),
(18,1,'No',0),
(19,1,'I plan to discuss reproductive options with my GP',0),
(20,1,'I have not considered discussing reproductive options with my GP',0),
(21,1,'I do not need to discuss reproductive options with my GP',0),
(22,1,'I have had preventive surgery',0),
(23,1,'Yes',0),
(24,1,'No',0),
(25,1,'Don''t know',0),
(26,1,'Risk-reducing mastectomy',0),
(27,1,'Risk-reducing salpingo-oophorectomy',0),
(28,1,'Yes',0),
(29,1,'No',0),
(30,1,'I plan to have risk-reducing mastectomy',0),
(31,1,'I declined risk-reducing mastectomy',0),
(32,1,'Risk-reducing mastectomy was not recommended',0),
(33,1,'I have had risk reducing mastectomy',0),
(34,1,'I plan to discuss risk reducing mastectomy with a specialist surgeon',0),
(35,1,'I do not plan to discuss risk reducing mastectomy with a specialist surgeon',0),
(36,1,'I have had risk reducing mastectomy',0),
(37,1,'I have not considered discussing risk reducing mastectomy with a specialist surgeon',0),
(38,1,'Yes',0),
(39,1,'No',0),
(40,1,'I plan to have operation',0),
(41,1,'I declined operation',0),
(42,1,'Operation not recommended',0),
(43,1,'I have had risk reducing salpingo-oophorectomy',0),
(44,1,'I plan to discuss risk-reducing salpingo-oophorectomy with a gynaecologist',0),
(45,1,'I do not plan to discuss risk reducing salpingo-oophorectomy with a gynaecologist',0),
(46,1,'I have not considered discussing risk-reducing salpingo-oophorectomy with a gynaecologist',0),
(47,1,'Yes',0),
(48,1,'No',0),
(49,1,'I plan to start taking tamoxifen',0),
(50,1,'I have declined taking tamoxifen',0),
(51,1,'I have started taking tamoxifen',0),
(52,1,'I plan to discuss taking tamoxifen with a specialist',0),
(53,1,'I have not considered discussing taking tamoxifen with a specialist',0),
(54,1,'Yes',0),
(55,1,'No',0),
(56,1,'Don''t know',0),
(57,1,'I plan to discuss breast self-examination with my GP or Breast Care Nurse',0),
(58,1,'I do not plan to discuss breast self-examination',0),
(59,1,'I have had risk reducing mastectomy',0),
(60,1,'I have no close family members',0),
(61,1,'I have notified them',0),
(62,1,'I have considered notifying them',0),
(63,1,'I plan to discuss with them at an appropriate age',0),
(64,1,'I have not considered notifying them',0),
(65,1,'I do not intend to notify them',0);

/* Message translations */
INSERT INTO QuestionMessageTranslations(MessageId,LanguageId,MessageText,CreatedBy) VALUES
(1,1,'If you would like to discuss this with a specialist please contact your GP or your Breast Care Nurse to arrange screening MRI scan of your breasts',0),
(2,1,'Please contact your GP to find out if you have had preventive surgery for BRCA then enter it here in the app.',0),
(3,1,'If you would like to discuss this with a specialist surgeon please contact your GP or your Breast Care Nurse to arrange a referral for consultation',0),
(4,1,'If you would like to discuss this with a gynaecologist please contact your GP or your Breast Care Nurse to arrange a referral for consultation',0),
(5,1,'If you would like to discuss this with a specialist please contact your GP or your Breast Care Nurse to arrange a referral for consultation',0),
(6,1,'Click here to see the breast self-examination section in the app',0),
(7,1,'Click here to see the breast self-examination section in the app',0);

