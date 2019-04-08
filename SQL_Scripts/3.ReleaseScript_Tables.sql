
/*************************************************************************************************
	
	PROJECT: LAMP

	RELEASE SCRIPTS - TABLES & SEED DATA
		
*************************************************************************************************/


BEGIN TRAN RELEASE

/* Drop all Tables in following order	
	
	DROP TABLE [dbo].[HealthKit_ParamValues]
	DROP TABLE [dbo].[HealthKit_Parameters]
	DROP TABLE [dbo].[CTest_SpinWheelResult]
	DROP TABLE [dbo].[CTest_ScratchImageResult]
	DROP TABLE [dbo].[CTest_ScratchImage]
	DROP TABLE [dbo].[Admin_Settings]
	DROP TABLE [dbo].[Admin_JewelsTrailsBSettings]
	DROP TABLE [dbo].[Admin_JewelsTrailsASettings]
	DROP TABLE [dbo].[HealthKit_DailyValues]
	DROP TABLE [dbo].[HealthKit_BasicInfo]	
	DROP TABLE [dbo].[Admin_SurveyScheduleCustomTime]
	DROP TABLE [dbo].[Admin_SurveySchedule]
	DROP TABLE [dbo].[Admin_CTestScheduleCustomTime]
	DROP TABLE [dbo].[Admin_CTestSchedule]
	DROP TABLE [dbo].[Admin_CTestSurveySettings]
	DROP TABLE [dbo].[AppHelp]
	DROP TABLE [dbo].[Tips]
	DROP TABLE [dbo].[Blogs]
	DROP TABLE [dbo].[CTest_JewelsTrailsBResultDtl]
	DROP TABLE [dbo].[CTest_JewelsTrailsBResult]
	DROP TABLE [dbo].[CTest_JewelsTrailsAResultDtl]
	DROP TABLE [dbo].[CTest_JewelsTrailsAResult]
	DROP TABLE [dbo].[CTest_TemporalOrderResult]
	DROP TABLE [dbo].[CTest_CatAndDogNewResultDtl]
	DROP TABLE [dbo].[CTest_CatAndDogNewResult]
	DROP TABLE [dbo].[CTest_SpatialResultDtl]
	DROP TABLE [dbo].[CTest_SpatialResult]
	DROP TABLE [dbo].[CTest_VisualAssociationResult]
	DROP TABLE [dbo].[CTest_TrailsBDotTouchResultDtl]
	DROP TABLE [dbo].[CTest_TrailsBDotTouchResult]
	DROP TABLE [dbo].[CTest_TrailsBNewResultDtl]
	DROP TABLE [dbo].[CTest_TrailsBNewResult]
	DROP TABLE [dbo].[CTest_TrailsBResultDtl]
	DROP TABLE [dbo].[CTest_TrailsBResult]
	DROP TABLE [dbo].[CTest_SimpleMemoryResult]
	DROP TABLE [dbo].[CTest_Serial7Result]
	DROP TABLE [dbo].[CTest_NBackNewResult]
	DROP TABLE [dbo].[CTest_NBackResult]
	DROP TABLE [dbo].[CTest_DigitSpanResult]
	DROP TABLE [dbo].[CTest_CatAndDogResult]
	DROP TABLE [dbo].[CTest_3DFigureResult]
	DROP TABLE [dbo].[CTest_3DFigure]
	DROP TABLE [dbo].[HelpCalls]
	DROP TABLE [dbo].[Locations]
	DROP TABLE [dbo].[SurveyResultDtl]
	DROP TABLE [dbo].[SurveyResult]
	DROP TABLE [dbo].[SurveyQuestionOptions]
	DROP TABLE [dbo].[SurveyQuestions]
	DROP TABLE [dbo].[Admin_BatchScheduleSurvey]
	DROP TABLE [dbo].[Admin_BatchScheduleCTest]
	DROP TABLE [dbo].[Admin_BatchScheduleCustomTime]
	DROP TABLE [dbo].[Admin_BatchSchedule]
	DROP TABLE [dbo].[Survey]
	DROP TABLE [dbo].[CTest]
	DROP TABLE [dbo].[UserDevices]
	DROP TABLE [dbo].[UserSettings]
	DROP TABLE [dbo].[Users]
	DROP TABLE [dbo].[Repeat]
	DROP TABLE [dbo].[Slot]
	DROP TABLE [dbo].[Admin]

*/


--	==========================================================
--	Object: Admin  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin](
			[AdminID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminType] [tinyint] NULL,
			[Email] [nvarchar](max) NULL,
			[Password] [nvarchar](max) NULL,
			[FirstName] [nvarchar](max) NULL,
			[LastName] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_Admin_CreatedOn]  DEFAULT (getutcdate()),
			[EditedOn] [datetime] NULL,
			[IsDeleted] [bit] NULL CONSTRAINT [DF_Admin_IsDeleted]  DEFAULT ((0)),			
		 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
		(
			[AdminID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
		) ON [PRIMARY]
			
		EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1: Super Admin, 2: Admin' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Admin', @level2type=N'COLUMN',@level2name=N'AdminType'		

	END
	GO

--	==========================================================
--	Object: Slot  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Slot]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Slot](
			[SlotID] [bigint] NOT NULL,
			[SlotName] [nvarchar](max) NULL,
			[IsDefault] [bit] NULL CONSTRAINT [DF_Slot_IsDefault]  DEFAULT ((0)),
		 CONSTRAINT [PK_Slot] PRIMARY KEY CLUSTERED 
		(
			[SlotID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
		) ON [PRIMARY]
					
	END
	GO


--	==========================================================
--	Object: Repeat  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Repeat]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Repeat](
			[RepeatID] [bigint] NOT NULL,
			[RepeatInterval] [nvarchar](max) NULL,
			[IsDefault] [bit] NULL CONSTRAINT [DF_Repeat_IsDefault]  DEFAULT ((0)),
			[SortOrder] [int] NULL,
			[IsDeleted] [bit] NULL CONSTRAINT [DF_Repeat_IsDeleted]  DEFAULT ((0)),
		 CONSTRAINT [PK_Repeat] PRIMARY KEY CLUSTERED 
		(
			[RepeatID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
		) ON [PRIMARY]
					
	END
	GO

--	==========================================================
--	Object: Users  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Users](
			[UserID] [bigint] IDENTITY(1,1) NOT NULL,
			[Email] [nvarchar](max) NULL,
			[Password] [nvarchar](max) NULL,
			[FirstName] [nvarchar](max) NULL,
			[LastName] [nvarchar](max) NULL,
			[Phone] [nvarchar](max) NULL,
			[ZipCode] [nvarchar](max) NULL,
			[City] [nvarchar](max) NULL,
			[State] [nvarchar](max) NULL,
			[Gender] [tinyint] NULL,
			[Age] [tinyint] NULL,
			[BirthDate] [date] NULL,
			[ClinicalProfileURL] [nvarchar](max) NULL,
			[IsGuestUser] [bit] NULL CONSTRAINT [DF_Users_IsGuestUser]  DEFAULT ((0)),
			[PhysicianFirstName] [nvarchar](max) NULL,
			[PhysicianLastName] [nvarchar](max) NULL,
			[StudyCode] [nvarchar](max) NULL,
			[StudyId] [nvarchar](max) NULL,
			[MR] [nvarchar](max) NULL,
			[Education] [nvarchar](max) NULL,
			[Race] [nvarchar](max) NULL,
			[Ethucity] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_Users_CreatedOn]  DEFAULT (getutcdate()),
			[EditedOn] [datetime] NULL,
			[RegisteredOn] [datetime] NULL,
			[IsDeleted] [bit] NULL CONSTRAINT [DF_Users_IsDeleted]  DEFAULT ((0)),
			[DeletedOn] [datetime] NULL,
			[Status] [bit] NULL CONSTRAINT [DF_Users_Status]  DEFAULT ((1)),
			[StatusEditedOn] [datetime] NULL,
			[SessionToken] [nvarchar](max) NULL,
			[APPVersion] [nvarchar](max) NULL,
			[AdminID] [bigint] NULL,
		 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
		(
			[UserID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY],		 
		) ON [PRIMARY]
		
		ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_Users_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])
			
	END
	GO

--	==========================================================
--	Object: UserSettings  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSettings]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[UserSettings](
			[UserSettingID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[AppColor] [nvarchar](max) NULL,
			[SympSurvey_SlotID] [bigint] NULL,
			[SympSurvey_Time] [datetime] NULL,
			[SympSurvey_RepeatID] [bigint] NULL,
			[CognTest_SlotID] [bigint] NULL,
			[CognTest_Time] [datetime] NULL,
			[CognTest_RepeatID] [bigint] NULL,
			[24By7ContactNo] [nvarchar](max) NULL,
			[PersonalHelpline] [nvarchar](max) NULL,
			[PrefferedSurveys] [nvarchar](max) NULL,
			[PrefferedCognitions] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_UserSettings_CreatedOn]  DEFAULT (getutcdate()),
			[EditedOn] [datetime] NULL,
			[Protocol] [bit] CONSTRAINT [DF_Users_Protocol]  DEFAULT ((0)),
			[BlogsViewedOn] [datetime] NULL,
			[TipsViewedOn] [datetime] NULL,
			[ProtocolDate] [datetime] NULL,
			[Language] [nvarchar](10) NULL,
		 CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED 
		(
			[UserSettingID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_CognTest_Repeat] FOREIGN KEY([CognTest_RepeatID])
		REFERENCES [dbo].[Repeat] ([RepeatID])

		ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_CognTest_Slot] FOREIGN KEY([CognTest_SlotID])
		REFERENCES [dbo].[Slot] ([SlotID])

		ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_SympSurvey_Repeat] FOREIGN KEY([SympSurvey_RepeatID])
		REFERENCES [dbo].[Repeat] ([RepeatID])

		ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_SympSurvey_Slot] FOREIGN KEY([SympSurvey_SlotID])
		REFERENCES [dbo].[Slot] ([SlotID])

		ALTER TABLE [dbo].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
					
	END
	GO
	
--	==========================================================
--	Object: UserDevices  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserDevices]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[UserDevices](
			[UserDeviceID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[DeviceType] [tinyint] NOT NULL,
			[DeviceID] [varchar](max) NULL,
			[DeviceToken] [varchar](max) NULL,
			[LastLoginOn] [datetime] NULL,
		 CONSTRAINT [PK_UserDevices] PRIMARY KEY CLUSTERED 
		(
			[UserDeviceID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[UserDevices]  WITH CHECK ADD  CONSTRAINT [FK_UserDevices_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
					
	END
	GO

--	==========================================================
--	Object: CTest  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest](
			[CTestID] [bigint] NOT NULL,
			[CTestName] [nvarchar](100) NULL,
			[IsDistractionSurveyRequired] [bit] NULL CONSTRAINT [DF_CTest_IsDistractionSurveyRequired]  DEFAULT ((0)),
			[IsDeleted] [bit] NULL CONSTRAINT [DF_CTest_IsDeleted]  DEFAULT ((0)),
			[SortOrder] [int] NULL,
			[MaxVersions] [int] NULL,
		 CONSTRAINT [PK_CTest] PRIMARY KEY CLUSTERED 
		(
			[CTestID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
		) ON [PRIMARY]	

	END
	GO

--	==========================================================
--	Object: Survey  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Survey]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Survey](
			[SurveyID] [bigint] IDENTITY(1,1) NOT NULL,
			[SurveyName] [nvarchar](100) NULL,
			[AdminID] [bigint] NULL,
			[CreatedOn] [datetime] NULL,
			[EditedOn] [datetime] NULL,
			[IsDeleted] [bit] NULL,
			[Language] [nvarchar](10) NULL,		
			[Instructions] [nvarchar](max) NULL,	
		 CONSTRAINT [PK_Survey] PRIMARY KEY CLUSTERED 
		(
			[SurveyID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Survey] ADD  CONSTRAINT [DF_Survey_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[Survey] ADD  CONSTRAINT [DF_Survey_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
		
	END
	GO

--	==========================================================
--	Object: Admin_BatchSchedule  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_BatchSchedule]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_BatchSchedule](
			[AdminBatchSchID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminID] [bigint] NULL,
			[BatchName] [nvarchar](max) NULL,
			[ScheduleDate] [datetime] NULL,
			[SlotID] [bigint] NULL,
			[Time] [datetime] NULL,
			[RepeatID] [bigint] NULL,
			[CreatedOn] [datetime] NULL,
			[EditedOn] [datetime] NULL,
			[IsDeleted] [bit] NULL,
		 CONSTRAINT [PK_Admin_BatchSchedule] PRIMARY KEY CLUSTERED 
		(
			[AdminBatchSchID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_BatchSchedule] ADD  CONSTRAINT [DF_Admin_BatchSchedule_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[Admin_BatchSchedule] ADD  CONSTRAINT [DF_Admin_BatchSchedule_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]

		ALTER TABLE [dbo].[Admin_BatchSchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_BatchSchedule_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])

		ALTER TABLE [dbo].[Admin_BatchSchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_BatchSchedule_Repeat] FOREIGN KEY([RepeatID])
		REFERENCES [dbo].[Repeat] ([RepeatID])

		ALTER TABLE [dbo].[Admin_BatchSchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_BatchSchedule_Slot] FOREIGN KEY([SlotID])
		REFERENCES [dbo].[Slot] ([SlotID])		
	
	END
	GO

--	==========================================================
--	Object: Admin_BatchScheduleCustomTime  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_BatchScheduleCustomTime]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_BatchScheduleCustomTime](
			[AdminBatchSchCustTimID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminBatchSchID] [bigint] NULL,
			[Time] [datetime] NULL,
		 CONSTRAINT [PK_Admin_BatchScheduleCustomTime] PRIMARY KEY CLUSTERED 
		(
			[AdminBatchSchCustTimID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_BatchScheduleCustomTime]  WITH CHECK ADD  CONSTRAINT [FK_Admin_BatchScheduleCustomTime_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID])
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])		
	
	END
	GO

--	==========================================================
--	Object: Admin_BatchScheduleCTest  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_BatchScheduleCTest]') AND type in (N'U'))
	BEGIN
		
		CREATE TABLE [dbo].[Admin_BatchScheduleCTest](
			[AdminBatchSchCTestID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminBatchSchID] [bigint] NULL,
			[CTestID] [bigint] NULL,
			[Version] [int] NULL,
			[Order] [int] NULL,
		 CONSTRAINT [PK_Admin_BatchScheduleCTest] PRIMARY KEY CLUSTERED 
		(
			[AdminBatchSchCTestID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_BatchScheduleCTest]  WITH CHECK ADD  CONSTRAINT [FK_Admin_BatchScheduleCTest_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID])
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])
	
	END
	GO

--	==========================================================
--	Object: Admin_BatchScheduleSurvey  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_BatchScheduleSurvey]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_BatchScheduleSurvey](
			[AdminBatchSchSurveyID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminBatchSchID] [bigint] NULL,
			[SurveyID] [bigint] NULL,
			[Order] [int] NULL,
		 CONSTRAINT [PK_Admin_BatchScheduleSurvey] PRIMARY KEY CLUSTERED 
		(
			[AdminBatchSchSurveyID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_BatchScheduleSurvey]  WITH CHECK ADD  CONSTRAINT [FK_Admin_BatchScheduleSurvey_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID])
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])

		ALTER TABLE [dbo].[Admin_BatchScheduleSurvey] CHECK CONSTRAINT [FK_Admin_BatchScheduleSurvey_Admin_BatchSchedule]		
	
	END
	GO

--	==========================================================
--	Object: SurveyQuestions  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SurveyQuestions]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[SurveyQuestions](
			[QuestionID] [bigint] IDENTITY(1,1) NOT NULL,
			[SurveyID] [bigint] NOT NULL,
			[QuestionText] [nvarchar](max) NULL,
			[AnswerType] [tinyint] NULL,
			[IsDeleted] [bit] NULL,
		 CONSTRAINT [PK_SurveyQuestions] PRIMARY KEY CLUSTERED 
		(
			[QuestionID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[SurveyQuestions] ADD  CONSTRAINT [DF_SurveyQuestions_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]

		ALTER TABLE [dbo].[SurveyQuestions]  WITH CHECK ADD  CONSTRAINT [FK_SurveyQuestions_Survey] FOREIGN KEY([SurveyID])
		REFERENCES [dbo].[Survey] ([SurveyID])

		EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1: Likert Response, 2: Scroll Wheels, 3: Yes or NO, 4: Clock, 5: Years' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SurveyQuestions', @level2type=N'COLUMN',@level2name=N'AnswerType'
		
	END
	GO

--	==========================================================
--	Object: SurveyQuestionOptions  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SurveyQuestionOptions]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[SurveyQuestionOptions](
			[OptionID] [bigint] IDENTITY(1,1) NOT NULL,
			[QuestionID] [bigint] NOT NULL,
			[OptionText] [nvarchar](100) NULL,
		 CONSTRAINT [PK_SurveyQuestionOptions] PRIMARY KEY CLUSTERED 
		(
			[OptionID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[SurveyQuestionOptions]  WITH CHECK ADD  CONSTRAINT [FK_SurveyQuestionOptions_SurveyQuestions] FOREIGN KEY([QuestionID])
		REFERENCES [dbo].[SurveyQuestions] ([QuestionID])
	
	END
	GO
				
--	==========================================================
--	Object: SurveyResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SurveyResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[SurveyResult](
			[SurveyResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[SurveyType] [tinyint] NOT NULL,
			[SurveyName] [nvarchar](max) NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [nvarchar](max) NULL,
			[Comment] [nvarchar](max) NULL,
			[Point] [numeric](10, 2) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_SurveyResult_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsDistraction] [bit] NULL CONSTRAINT [DF_SurveyResult_IsDistraction]  DEFAULT ((0)),
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_SurveyResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
			[SurveyID] [bigint] NULL,
		 CONSTRAINT [PK_SurveyResult] PRIMARY KEY CLUSTERED 
		(
			[SurveyResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[SurveyResult]  WITH CHECK ADD  CONSTRAINT [FK_SurveyResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[SurveyResult] ADD CONSTRAINT [FK_CTest_SurveyResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])

		ALTER TABLE [dbo].[SurveyResult]  WITH CHECK ADD  CONSTRAINT [FK_SurveyResult_Survey] FOREIGN KEY([SurveyID])
		REFERENCES [dbo].[Survey] ([SurveyID])

		EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1: Radio Button, 2: Free Response, 3: Likert Response' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SurveyResult', @level2type=N'COLUMN',@level2name=N'SurveyType'
					
	END
	GO
	
--	==========================================================
--	Object: SurveyResultDtl  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SurveyResultDtl]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[SurveyResultDtl](
			[SurveyResultDtlID] [bigint] IDENTITY(1,1) NOT NULL,
			[SurveyResultID] [bigint] NOT NULL,
			[Question] [nvarchar](max) NULL,
			[CorrectAnswer] [nvarchar](max) NULL,
			[EnteredAnswer] [nvarchar](max) NULL,
			[TimeTaken] [int] NULL,
			[ClickRange] [nvarchar](100) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_SurveyResultDtl_CreatedOn]  DEFAULT (getutcdate()),			
		 CONSTRAINT [PK_SurveyResultDtl] PRIMARY KEY CLUSTERED 
		(
			[SurveyResultDtlID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[SurveyResultDtl]  WITH CHECK ADD  CONSTRAINT [FK_SurveyResultDtl_SurveyResult] FOREIGN KEY([SurveyResultID])
		REFERENCES [dbo].[SurveyResult] ([SurveyResultID])
					
	END
	GO
	
--	==========================================================
--	Object: Locations  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Locations]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Locations](
			[LocationID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[LocationName] [nvarchar](max) NULL,
			[Address] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_Locations_CreatedOn]  DEFAULT (getutcdate()),
			[Type] [tinyint] NULL,
			[Latitude] [nvarchar](max) NULL,
			[Longitude] [nvarchar](max) NULL,
		 CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED 
		(
			[LocationID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Locations]  WITH CHECK ADD  CONSTRAINT [FK_Locations_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1: Location, 2: Environment' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Locations', @level2type=N'COLUMN',@level2name=N'Type'
					
	END
	GO
	
--	==========================================================
--	Object: HelpCalls  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HelpCalls]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[HelpCalls](
			[HelpCallID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[CalledNumber] [nvarchar](max) NULL,
			[CallDateTime] [datetime] NULL,
			[CallDuraion] [bigint] NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_HelpCalls_CreatedOn]  DEFAULT (getutcdate()),
			[Type] [tinyint] NULL,
		 CONSTRAINT [PK_HelpCalls] PRIMARY KEY CLUSTERED 
		(
			[HelpCallID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[HelpCalls]  WITH CHECK ADD  CONSTRAINT [FK_HelpCalls_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1: Emergency, 2: Personal Help Line' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HelpCalls', @level2type=N'COLUMN',@level2name=N'Type'
					
	END
	GO
		
--	==========================================================
--	Object: CTest_3DFigure  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_3DFigure]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_3DFigure](
			[3DFigureID] [bigint] IDENTITY(1,1) NOT NULL,
			[FigureName] [nvarchar](max) NULL,
			[FileName] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_3DFigure_CreatedOn]  DEFAULT (getutcdate()),
		 CONSTRAINT [PK_CTest_3DFigure] PRIMARY KEY CLUSTERED 
		(
			[3DFigureID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
					
	END
	GO
	
--	==========================================================
--	Object: CTest_3DFigureResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_3DFigureResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_3DFigureResult](
			[3DFigureResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[3DFigureID] [bigint] NOT NULL,
			[DrawnFigFileName] [nvarchar](max) NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[GameName] [nvarchar](max) NULL,
			[Point] [numeric](10, 2) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_3DFigureResult_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_3DFigureResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_3DFigureResult] PRIMARY KEY CLUSTERED 
		(
			[3DFigureResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_3DFigureResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_3DFigureResult_CTest_3DFigure] FOREIGN KEY([3DFigureID])
		REFERENCES [dbo].[CTest_3DFigure] ([3DFigureID])

		ALTER TABLE [dbo].[CTest_3DFigureResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_3DFigureResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_3DFigureResult] ADD CONSTRAINT [FK_CTest_3DFigureResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])	
						
	END
	GO
	
--	==========================================================
--	Object: CTest_CatAndDogResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_CatAndDogResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_CatAndDogResult](
			[CatAndDogResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalQuestions] [int] NULL,
			[CorrectAnswers] [int] NULL,
			[WrongAnswers] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_CatAndDogResult_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_CatAndDogResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_CatAndDogResult] PRIMARY KEY CLUSTERED 
		(
			[CatAndDogResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_CatAndDogResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_CatAndDogResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_CatAndDogResult] ADD CONSTRAINT [FK_CTest_CatAndDogResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])
							
	END
	GO
	
--	==========================================================
--	Object: CTest_DigitSpanResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_DigitSpanResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_DigitSpanResult](
			[DigitSpanResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[Type] [tinyint] NULL,
			[CorrectAnswers] [int] NULL,
			[WrongAnswers] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_DigitSpanResult_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_DigitSpanResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_DigitSpanResult] PRIMARY KEY CLUSTERED 
		(
			[DigitSpanResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_DigitSpanResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_DigitSpanResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
	
		ALTER TABLE [dbo].[CTest_DigitSpanResult] ADD CONSTRAINT [FK_CTest_DigitSpanResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])

	END
	GO
	
--	==========================================================
--	Object: CTest_NBackResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_NBackResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_NBackResult](
			[NBackResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalQuestions] [int] NULL,
			[CorrectAnswers] [int] NULL,
			[WrongAnswers] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[Version] [int] NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_NBackResult_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_NBackResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_NBackResult] PRIMARY KEY CLUSTERED 
		(
			[NBackResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_NBackResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_NBackResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_NBackResult] ADD CONSTRAINT [FK_CTest_NBackResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])
						
	END
	GO

--	==========================================================
--	Object: CTest_NBackNewResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_NBackNewResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_NBackNewResult](
			[NBackNewResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalQuestions] [int] NULL,
			[CorrectAnswers] [int] NULL,
			[WrongAnswers] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_NBackNewResult_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_NBackNewResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_NBackNewResult] PRIMARY KEY CLUSTERED 
		(
			[NBackNewResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_NBackNewResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_NBackNewResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_NBackNewResult] ADD CONSTRAINT [FK_CTest_NBackNewResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])	
				
	END
	GO
		
--	==========================================================
--	Object: CTest_Serial7Result  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_Serial7Result]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_Serial7Result](
			[Serial7ResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalQuestions] [int] NULL,
			[TotalAttempts] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[Version] [int] NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_Serial7Result_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_Serial7Result_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_Serial7Result] PRIMARY KEY CLUSTERED 
		(
			[Serial7ResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_Serial7Result]  WITH CHECK ADD  CONSTRAINT [FK_CTest_Serial7Result_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_Serial7Result] ADD CONSTRAINT [FK_CTest_Serial7Result_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])
							
	END
	GO
	
--	==========================================================
--	Object: CTest_SimpleMemoryResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_SimpleMemoryResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_SimpleMemoryResult](
			[SimpleMemoryResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalQuestions] [int] NULL,
			[CorrectAnswers] [int] NULL,
			[WrongAnswers] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[Version] [int] NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_SimpleMemoryResult_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_SimpleMemoryResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_SimpleMemoryResult] PRIMARY KEY CLUSTERED 
		(
			[SimpleMemoryResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_SimpleMemoryResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_SimpleMemoryResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_SimpleMemoryResult] ADD CONSTRAINT [FK_CTest_SimpleMemoryResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])
					
	END
	GO
	
--	==========================================================
--	Object: CTest_TrailsBResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_TrailsBResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_TrailsBResult](
			[TrailsBResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalAttempts] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_TrailsBResult_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_TrailsBResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_TrailsBResult] PRIMARY KEY CLUSTERED 
		(
			[TrailsBResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_TrailsBResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_TrailsBResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_TrailsBResult] ADD CONSTRAINT [FK_CTest_TrailsBResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])
							
	END
	GO

--	==========================================================
--	Object: CTest_TrailsBResultDtl  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_TrailsBResultDtl]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_TrailsBResultDtl](
			[TrailsBResultDtlID] [bigint] IDENTITY(1,1) NOT NULL,
			[TrailsBResultID] [bigint] NOT NULL,
			[Alphabet] [nvarchar](1) NULL,
			[TimeTaken] [nvarchar](10) NULL,
			[Status] [bit] NULL,
			[Sequence] [int] NULL,
			[CreatedOn] [datetime] NULL,
		 CONSTRAINT [PK_CTest_TrailsBResultDtl] PRIMARY KEY CLUSTERED 
		(
			[TrailsBResultDtlID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_TrailsBResultDtl] ADD  CONSTRAINT [DF_CTest_TrailsBResultDtl_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_TrailsBResultDtl]  WITH CHECK ADD  CONSTRAINT [FK_CTest_TrailsBResultDtl_CTest_TrailsBResult] FOREIGN KEY([TrailsBResultID])
		REFERENCES [dbo].[CTest_TrailsBResult] ([TrailsBResultID])

	END


--	==========================================================
--	Object: CTest_TrailsBNewResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_TrailsBNewResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_TrailsBNewResult](
			[TrailsBNewResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalAttempts] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,			
			[Score] [nvarchar](max) NULL,
			[Version] [int] NULL,
			[CreatedOn] [datetime] NULL,
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_TrailsBNewResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_TrailsBNewResult] PRIMARY KEY CLUSTERED 
		(
			[TrailsBNewResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_TrailsBNewResult] ADD  CONSTRAINT [DF_CTest_TrailsBNewResult_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_TrailsBNewResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_TrailsBNewResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_TrailsBNewResult] ADD CONSTRAINT [FK_CTest_TrailsBNewResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])
							
	END
	GO
	
--	==========================================================
--	Object: CTest_TrailsBNewResultDtl  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_TrailsBNewResultDtl]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_TrailsBNewResultDtl](
			[TrailsBNewResultDtlID] [bigint] IDENTITY(1,1) NOT NULL,
			[TrailsBNewResultID] [bigint] NOT NULL,
			[Alphabet] [nvarchar](max) NULL,
			[TimeTaken] [nvarchar](max) NULL,
			[Status] [bit] NULL,
			[Sequence] [int] NULL,
			[CreatedOn] [datetime] NULL,
		 CONSTRAINT [PK_CTest_TrailsBNewResultDtl] PRIMARY KEY CLUSTERED 
		(
			[TrailsBNewResultDtlID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_TrailsBNewResultDtl] ADD  CONSTRAINT [DF_CTest_TrailsBNewResultDtl_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_TrailsBNewResultDtl]  WITH CHECK ADD  CONSTRAINT [FK_CTest_TrailsBNewResultDtl_CTest_TrailsBNewResult] FOREIGN KEY([TrailsBNewResultID])
		REFERENCES [dbo].[CTest_TrailsBNewResult] ([TrailsBNewResultID])
					
	END
	GO

--	==========================================================
--	Object: CTest_TrailsBDotTouchResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_TrailsBDotTouchResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_TrailsBDotTouchResult](
			[TrailsBDotTouchResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalAttempts] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_TrailsBDotTouchResult_CreatedOn]  DEFAULT (getutcdate()),
			[Score] [nvarchar](max) NULL,
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_TrailsBDotTouchResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_TrailsBDotTouchResult] PRIMARY KEY CLUSTERED 
		(
			[TrailsBDotTouchResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_TrailsBDotTouchResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_TrailsBDotTouchResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_TrailsBDotTouchResult] ADD CONSTRAINT [FK_CTest_TrailsBDotTouchResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])
							
	END
	GO

--	==========================================================
--	Object: CTest_TrailsBDotTouchResultDtl  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_TrailsBDotTouchResultDtl]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_TrailsBDotTouchResultDtl](
			[TrailsBDotTouchResultDtlID] [bigint] IDENTITY(1,1) NOT NULL,
			[TrailsBDotTouchResultID] [bigint] NOT NULL,
			[Alphabet] [nvarchar](max) NULL,
			[TimeTaken] [nvarchar](max) NULL,
			[Status] [bit] NULL,
			[Sequence] [int] NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_TrailsBDotTouchResultDtl_CreatedOn]  DEFAULT (getutcdate()),
		 CONSTRAINT [PK_CTest_TrailsBDotTouchResultDtl] PRIMARY KEY CLUSTERED 
		(
			[TrailsBDotTouchResultDtlID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_TrailsBDotTouchResultDtl]  WITH CHECK ADD  CONSTRAINT [FK_CTest_TrailsBDotTouchResultDtl_CTest_TrailsBDotTouchResult] FOREIGN KEY([TrailsBDotTouchResultID])
		REFERENCES [dbo].[CTest_TrailsBDotTouchResult] ([TrailsBDotTouchResultID])
					
	END
	GO
							
--	==========================================================
--	Object: CTest_VisualAssociationResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_VisualAssociationResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_VisualAssociationResult](
			[VisualAssocResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalQuestions] [int] NULL,
			[TotalAttempts] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[Version] [int] NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_VisualAssociationResult_CreatedOn]  DEFAULT (getutcdate()),
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_VisualAssociationResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_VisualAssociationResult] PRIMARY KEY CLUSTERED 
		(
			[VisualAssocResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_VisualAssociationResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_VisualAssociationResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_VisualAssociationResult] ADD CONSTRAINT [FK_CTest_VisualAssociationResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])
							
	END
	GO

--	==========================================================
--	Object: CTest_SpatialResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_SpatialResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_SpatialResult](
			[SpatialResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[Type] [tinyint] NULL,
			[CorrectAnswers] [int] NULL,
			[WrongAnswers] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL,
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_SpatialResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_SpatialResult] PRIMARY KEY CLUSTERED 
		(
			[SpatialResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_SpatialResult] ADD  CONSTRAINT [DF_CTest_SpatialResult_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_SpatialResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_SpatialResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_SpatialResult] ADD CONSTRAINT [FK_CTest_SpatialResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])

	END
	GO

--	==========================================================
--	Object: CTest_SpatialResultDtl  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_SpatialResultDtl]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_SpatialResultDtl](
			[SpatialResultDtlID] [bigint] IDENTITY(1,1) NOT NULL,
			[SpatialResultID] [bigint] NOT NULL,
			[GameIndex] [tinyint] NULL,
			[TimeTaken] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL,
			[Status] [bit] NULL,
			[Level] [int] NULL,
			[Sequence] [int] NULL,
		 CONSTRAINT [PK_CTest_SpatialResultDtl] PRIMARY KEY CLUSTERED 
		(
			[SpatialResultDtlID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


		ALTER TABLE [dbo].[CTest_SpatialResultDtl] ADD  CONSTRAINT [DF_CTest_SpatialResultDtl_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_SpatialResultDtl]  WITH CHECK ADD  CONSTRAINT [FK_CTest_SpatialResultDtl_CTest_SpatialResult] FOREIGN KEY([SpatialResultID])
		REFERENCES [dbo].[CTest_SpatialResult] ([SpatialResultID])
		
	END
	GO

--	==========================================================
--	Object: CTest_CatAndDogNewResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_CatAndDogNewResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_CatAndDogNewResult](
			[CatAndDogNewResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[CorrectAnswers] [int] NULL,
			[WrongAnswers] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL,
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_CatAndDogNewResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_CatAndDogNewResult] PRIMARY KEY CLUSTERED 
		(
			[CatAndDogNewResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_CatAndDogNewResult] ADD  CONSTRAINT [DF_CTest_CatAndDogNewResult_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_CatAndDogNewResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_CatAndDogNewResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])

	END
	GO

--	==========================================================
--	Object: CTest_CatAndDogNewResultDtl  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_CatAndDogNewResultDtl]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_CatAndDogNewResultDtl](
			[CatAndDogNewResultDtlID] [bigint] IDENTITY(1,1) NOT NULL,
			[CatAndDogNewResultID] [bigint] NOT NULL,
			[CorrectAnswers] [int] NULL,
			[WrongAnswers] [int] NULL,
			[TimeTaken] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL,
		 CONSTRAINT [PK_CTest_CatAndDogNewResultDtl] PRIMARY KEY CLUSTERED 
		(
			[CatAndDogNewResultDtlID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_CatAndDogNewResultDtl] ADD  CONSTRAINT [DF_CTest_CatAndDogNewResultDtl_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_CatAndDogNewResultDtl]  WITH CHECK ADD  CONSTRAINT [FK_CTest_CatAndDogNewResultDtl_CTest_CatAndDogNewResult] FOREIGN KEY([CatAndDogNewResultID])
		REFERENCES [dbo].[CTest_CatAndDogNewResult] ([CatAndDogNewResultID])		

	END
	GO

--	==========================================================
--	Object: CTest_TemporalOrderResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_TemporalOrderResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_TemporalOrderResult](
			[TemporalOrderResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[CorrectAnswers] [int] NULL,
			[WrongAnswers] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[Score] [nvarchar](max) NULL,
			[Version] [int] NULL,
			[CreatedOn] [datetime] NULL,
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_TemporalOrderResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_TemporalOrderResult] PRIMARY KEY CLUSTERED 
		(
			[TemporalOrderResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_TemporalOrderResult] ADD  CONSTRAINT [DF_CTest_TemporalOrderResult_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_TemporalOrderResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_TemporalOrderResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])

		ALTER TABLE [dbo].[CTest_TemporalOrderResult] ADD CONSTRAINT [FK_CTest_TemporalOrderResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])

	END
	GO

--	==========================================================
--	Object: CTest_JewelsTrailsAResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_JewelsTrailsAResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_JewelsTrailsAResult](
			[JewelsTrailsAResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalAttempts] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[TotalJewelsCollected] [nvarchar](max) NULL,
			[TotalBonusCollected] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL,
			[Score] [nvarchar](max) NULL,
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_JewelsTrailsAResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_JewelsTrailsAResult] PRIMARY KEY CLUSTERED 
		(
			[JewelsTrailsAResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_JewelsTrailsAResult] ADD  CONSTRAINT [DF_CTest_JewelsTrailsAResult_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_JewelsTrailsAResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_JewelsTrailsAResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])

		ALTER TABLE [dbo].[CTest_JewelsTrailsAResult] ADD CONSTRAINT [FK_CTest_JewelsTrailsAResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])

	END
	GO

--	==========================================================
--	Object: CTest_JewelsTrailsAResultDtl  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_JewelsTrailsAResultDtl]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_JewelsTrailsAResultDtl](
			[JewelsTrailsAResultDtlID] [bigint] IDENTITY(1,1) NOT NULL,
			[JewelsTrailsAResultID] [bigint] NOT NULL,
			[Alphabet] [nvarchar](max) NULL,
			[TimeTaken] [nvarchar](max) NULL,
			[Status] [bit] NULL,
			[Sequence] [int] NULL,
			[CreatedOn] [datetime] NULL,
		 CONSTRAINT [PK_CTest_JewelsTrailsAResultDtl] PRIMARY KEY CLUSTERED 
		(
			[JewelsTrailsAResultDtlID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_JewelsTrailsAResultDtl] ADD  CONSTRAINT [DF_CTest_JewelsTrailsAResultDtl_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_JewelsTrailsAResultDtl]  WITH CHECK ADD  CONSTRAINT [FK_CTest_JewelsTrailsAResultDtl_CTest_JewelsTrailsAResult] FOREIGN KEY([JewelsTrailsAResultID])
		REFERENCES [dbo].[CTest_JewelsTrailsAResult] ([JewelsTrailsAResultID])

	END
	GO

--	==========================================================
--	Object: CTest_JewelsTrailsBResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_JewelsTrailsBResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_JewelsTrailsBResult](
			[JewelsTrailsBResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[TotalAttempts] [int] NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[Rating] [tinyint] NULL,
			[Point] [numeric](10, 2) NULL,
			[TotalJewelsCollected] [nvarchar](max) NULL,
			[TotalBonusCollected] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL,
			[Score] [nvarchar](max) NULL,
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_JewelsTrailsBResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_JewelsTrailsBResult] PRIMARY KEY CLUSTERED 
		(
			[JewelsTrailsBResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_JewelsTrailsBResult] ADD  CONSTRAINT [DF_CTest_JewelsTrailsBResult_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_JewelsTrailsBResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_JewelsTrailsBResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])

		ALTER TABLE [dbo].[CTest_JewelsTrailsBResult] ADD CONSTRAINT [FK_CTest_JewelsTrailsBResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])

	END
	GO

--	==========================================================
--	Object: CTest_JewelsTrailsBResultDtl  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_JewelsTrailsBResultDtl]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_JewelsTrailsBResultDtl](
			[JewelsTrailsBResultDtlID] [bigint] IDENTITY(1,1) NOT NULL,
			[JewelsTrailsBResultID] [bigint] NOT NULL,
			[Alphabet] [nvarchar](max) NULL,
			[TimeTaken] [nvarchar](max) NULL,
			[Status] [bit] NULL,
			[Sequence] [int] NULL,
			[CreatedOn] [datetime] NULL,
		 CONSTRAINT [PK_CTest_JewelsTrailsBResultDtl] PRIMARY KEY CLUSTERED 
		(
			[JewelsTrailsBResultDtlID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_JewelsTrailsBResultDtl] ADD  CONSTRAINT [DF_CTest_JewelsTrailsBResultDtl_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_JewelsTrailsBResultDtl]  WITH CHECK ADD  CONSTRAINT [FK_CTest_JewelsTrailsBResultDtl_CTest_JewelsTrailsBResult] FOREIGN KEY([JewelsTrailsBResultID])
		REFERENCES [dbo].[CTest_JewelsTrailsBResult] ([JewelsTrailsBResultID])

	END
	GO


--	==========================================================
--	Object: Blogs  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Blogs]') AND type in (N'U'))
	BEGIN
	
		CREATE TABLE [dbo].[Blogs](
			[BlogID] [bigint] IDENTITY(1,1) NOT NULL,
			[BlogTitle] [nvarchar](max) NULL,
			[BlogText] [nvarchar](max) NULL,
			[Content] [nvarchar](max) NULL,
			[ImageURL] [nvarchar](250) NULL,
			[CreatedOn] [datetime] NULL,
			[EditedOn] [datetime] NULL,
			[IsDeleted] [bit] NULL CONSTRAINT [DF_Blogs_IsDeleted]  DEFAULT ((0)),
			[AdminID] [bigint] NULL,
		 CONSTRAINT [PK_Blogs] PRIMARY KEY CLUSTERED 
		(
			[BlogID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[Blogs] ADD  CONSTRAINT [DF_Blogs_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[Blogs] ADD CONSTRAINT [FK_Blogs_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])

	END
	GO

--	==========================================================
--	Object: Tips  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tips]') AND type in (N'U'))
	BEGIN
	
		CREATE TABLE [dbo].[Tips](
			[TipID] [bigint] IDENTITY(1,1) NOT NULL,
			[TipText] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL,
			[EditedOn] [datetime] NULL,
			[IsDeleted] [bit] NULL CONSTRAINT [DF_Tips_IsDeleted]  DEFAULT ((0)),
			[AdminID] [bigint] NULL,
		 CONSTRAINT [PK_Tips] PRIMARY KEY CLUSTERED 
		(
			[TipID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[Tips] ADD  CONSTRAINT [DF_Tips_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[Tips] ADD CONSTRAINT [FK_Tips_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])

	END
	GO

--	==========================================================
--	Object: AppHelp  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppHelp]') AND type in (N'U'))
	BEGIN
	
		CREATE TABLE [dbo].[AppHelp](
			[HelpID] [bigint] IDENTITY(1,1) NOT NULL,
			[HelpTitle] [nvarchar](max) NULL,
			[HelpText] [nvarchar](max) NULL,
			[Content] [nvarchar](max) NULL,
			[ImageURL] [nvarchar](250) NULL,
			[CreatedOn] [datetime] NULL,
			[EditedOn] [datetime] NULL,
			[IsDeleted] [bit] NULL,
			[AdminID] [bigint] NULL,
		 CONSTRAINT [PK_AppHelp] PRIMARY KEY CLUSTERED 
		(
			[HelpID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[AppHelp] ADD  CONSTRAINT [DF_AppHelp_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[AppHelp] ADD  CONSTRAINT [DF_AppHelp_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]

		ALTER TABLE [dbo].[AppHelp]  WITH CHECK ADD  CONSTRAINT [FK_AppHelp_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])

	END
	GO

--	==========================================================
--	Object: Admin_CTestSurveySettings  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_CTestSurveySettings]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_CTestSurveySettings](
			[AdminCTestSurveySettingID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminID] [bigint] NULL,
			[CTestID] [bigint] NULL,
			[SurveyID] [bigint] NULL,
		 CONSTRAINT [PK_Admin_CTestSurveySettings] PRIMARY KEY CLUSTERED 
		(
			[AdminCTestSurveySettingID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_CTestSurveySettings]  WITH CHECK ADD  CONSTRAINT [FK_Admin_CTestSurveySettings_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])

		ALTER TABLE [dbo].[Admin_CTestSurveySettings]  WITH CHECK ADD  CONSTRAINT [FK_Admin_CTestSurveySettings_CTest] FOREIGN KEY([CTestID])
		REFERENCES [dbo].[CTest] ([CTestID])

		ALTER TABLE [dbo].[Admin_CTestSurveySettings]  WITH CHECK ADD  CONSTRAINT [FK_Admin_CTestSurveySettings_Survey] FOREIGN KEY([SurveyID])
		REFERENCES [dbo].[Survey] ([SurveyID])

	END
	GO

--	==========================================================
--	Object: Admin_CTestSchedule  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_CTestSchedule]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_CTestSchedule](
			[AdminCTestSchID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminID] [bigint] NULL,
			[CTestID] [bigint] NULL,
			[Version] [int] NULL,
			[ScheduleDate] [datetime] NULL,
			[SlotID] [bigint] NULL,
			[Time] [datetime] NULL,
			[RepeatID] [bigint] NULL,
			[CreatedOn] [datetime] NULL,
			[EditedOn] [datetime] NULL,
			[IsDeleted] [bit] NULL CONSTRAINT [DF_Admin_CTestSchedule_IsDeleted]  DEFAULT ((0)),
		 CONSTRAINT [PK_Admin_CTestSchedule] PRIMARY KEY CLUSTERED 
		(
			[AdminCTestSchID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_CTestSchedule] ADD  CONSTRAINT [DF_Admin_CTestSchedule_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[Admin_CTestSchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_CTestSchedule_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])

		ALTER TABLE [dbo].[Admin_CTestSchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_CTestSchedule_CTest] FOREIGN KEY([CTestID])
		REFERENCES [dbo].[CTest] ([CTestID])

		ALTER TABLE [dbo].[Admin_CTestSchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_CTestSchedule_Repeat] FOREIGN KEY([RepeatID])
		REFERENCES [dbo].[Repeat] ([RepeatID])

		ALTER TABLE [dbo].[Admin_CTestSchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_CTestSchedule_Slot] FOREIGN KEY([SlotID])
		REFERENCES [dbo].[Slot] ([SlotID])

	END
	GO

--	==========================================================
--	Object: Admin_CTestScheduleCustomTime  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_CTestScheduleCustomTime]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_CTestScheduleCustomTime](
			[AdminCTstSchCustTimID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminCTestSchID] [bigint] NOT NULL,
			[Time] [datetime] NULL,
		 CONSTRAINT [PK_Admin_CTestScheduleCustomTime] PRIMARY KEY CLUSTERED 
		(
			[AdminCTstSchCustTimID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_CTestScheduleCustomTime]  WITH CHECK ADD  CONSTRAINT [FK_Admin_CTestScheduleCustomTime_Admin_CTestSchedule] FOREIGN KEY([AdminCTestSchID])
		REFERENCES [dbo].[Admin_CTestSchedule] ([AdminCTestSchID])

	END
	GO

--	==========================================================
--	Object: Admin_SurveySchedule  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_SurveySchedule]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_SurveySchedule](
			[AdminSurveySchID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminID] [bigint] NULL,
			[SurveyID] [bigint] NULL,
			[ScheduleDate] [datetime] NULL,
			[SlotID] [bigint] NULL,
			[Time] [datetime] NULL,
			[RepeatID] [bigint] NULL,
			[CreatedOn] [datetime] NULL,
			[EditedOn] [datetime] NULL,
			[IsDeleted] [bit] NULL CONSTRAINT [DF_Admin_SurveySchedule_IsDeleted]  DEFAULT ((0)),
		 CONSTRAINT [PK_Admin_SurveySchedule] PRIMARY KEY CLUSTERED 
		(
			[AdminSurveySchID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_SurveySchedule] ADD  CONSTRAINT [DF_Admin_SurveySchedule_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[Admin_SurveySchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_SurveySchedule_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])

		ALTER TABLE [dbo].[Admin_SurveySchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_SurveySchedule_Repeat] FOREIGN KEY([RepeatID])
		REFERENCES [dbo].[Repeat] ([RepeatID])

		ALTER TABLE [dbo].[Admin_SurveySchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_SurveySchedule_Slot] FOREIGN KEY([SlotID])
		REFERENCES [dbo].[Slot] ([SlotID])

		ALTER TABLE [dbo].[Admin_SurveySchedule]  WITH CHECK ADD  CONSTRAINT [FK_Admin_SurveySchedule_Survey] FOREIGN KEY([SurveyID])
		REFERENCES [dbo].[Survey] ([SurveyID])

	END
	GO

--	==========================================================
--	Object: Admin_SurveyScheduleCustomTime  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_SurveyScheduleCustomTime]') AND type in (N'U'))
	BEGIN
		
		CREATE TABLE [dbo].[Admin_SurveyScheduleCustomTime](
			[AdminSurvSchCustTimID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminSurveySchID] [bigint] NOT NULL,
			[Time] [datetime] NULL,
		 CONSTRAINT [PK_Admin_SurveyScheduleCustomTime] PRIMARY KEY CLUSTERED 
		(
			[AdminSurvSchCustTimID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_SurveyScheduleCustomTime]  WITH CHECK ADD  CONSTRAINT [FK_Admin_SurveyScheduleCustomTime_Admin_SurveySchedule] FOREIGN KEY([AdminSurveySchID])
		REFERENCES [dbo].[Admin_SurveySchedule] ([AdminSurveySchID])

	END
	GO

--	==========================================================
--	Object: HealthKit_BasicInfo  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HealthKit_BasicInfo]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[HealthKit_BasicInfo](
			[HKBasicInfoID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[DateOfBirth] [date] NULL,
			[Sex] [nvarchar](max) NULL,
			[BloodType] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_HealthKit_BasicInfo_CreatedOn]  DEFAULT (getutcdate()),
			[EditedOn] [datetime] NULL,
		 CONSTRAINT [PK_HealthKit_BasicInfo] PRIMARY KEY CLUSTERED 
		(
			[HKBasicInfoID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[HealthKit_BasicInfo]  WITH CHECK ADD  CONSTRAINT [FK_HealthKit_BasicInfo_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])

		ALTER TABLE [dbo].[HealthKit_BasicInfo] CHECK CONSTRAINT [FK_HealthKit_BasicInfo_Users]	

	END
	GO

--	==========================================================
--	Object: HealthKit_DailyValues  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HealthKit_DailyValues]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[HealthKit_DailyValues](
			[HKDailyValueID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[Height] [nvarchar](max) NULL,
			[Weight] [nvarchar](max) NULL,
			[HeartRate] [nvarchar](max) NULL,
			[BloodPressure] [nvarchar](max) NULL,
			[RespiratoryRate] [nvarchar](max) NULL,
			[Sleep] [nvarchar](max) NULL,
			[Steps] [nvarchar](max) NULL,
			[FlightClimbed] [nvarchar](max) NULL,
			[Segment] [nvarchar](max) NULL,
			[Distance] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_HealthKit_DailyValues_CreatedOn]  DEFAULT (getutcdate()),
			[EditedOn] [datetime] NULL,
		 CONSTRAINT [PK_HealthKit_DailyValues] PRIMARY KEY CLUSTERED 
		(
			[HKDailyValueID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[HealthKit_DailyValues]  WITH CHECK ADD  CONSTRAINT [FK_HealthKit_DailyValues_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])

		ALTER TABLE [dbo].[HealthKit_DailyValues] CHECK CONSTRAINT [FK_HealthKit_DailyValues_Users]
	
	END
	GO

--	==========================================================
--	Object: Admin_JewelsTrailsASettings  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_JewelsTrailsASettings]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_JewelsTrailsASettings](
			[AdminJTASettingID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminID] [bigint] NULL,
			[NoOfSeconds_Beg] [int] NULL,
			[NoOfSeconds_Int] [int] NULL,
			[NoOfSeconds_Adv] [int] NULL,
			[NoOfSeconds_Exp] [int] NULL,
			[NoOfDiamonds] [int] NULL,
			[NoOfShapes] [int] NULL,
			[NoOfBonusPoints] [int] NULL,
			[X_NoOfChangesInLevel] [int] NULL,
			[X_NoOfDiamonds] [int] NULL,
			[Y_NoOfChangesInLevel] [int] NULL,
			[Y_NoOfShapes] [int] NULL,
		 CONSTRAINT [PK_Admin_JewelsTrailsASettings] PRIMARY KEY CLUSTERED 
		(
			[AdminJTASettingID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_JewelsTrailsASettings]  WITH CHECK ADD  CONSTRAINT [FK_Admin_JewelsTrailsASettings_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])
	
	END
	GO

--	==========================================================
--	Object: Admin_JewelsTrailsBSettings  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_JewelsTrailsBSettings]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_JewelsTrailsBSettings](
			[AdminJTBSettingID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminID] [bigint] NULL,
			[NoOfSeconds_Beg] [int] NULL,
			[NoOfSeconds_Int] [int] NULL,
			[NoOfSeconds_Adv] [int] NULL,
			[NoOfSeconds_Exp] [int] NULL,
			[NoOfDiamonds] [int] NULL,
			[NoOfShapes] [int] NULL,
			[NoOfBonusPoints] [int] NULL,
			[X_NoOfChangesInLevel] [int] NULL,
			[X_NoOfDiamonds] [int] NULL,
			[Y_NoOfChangesInLevel] [int] NULL,
			[Y_NoOfShapes] [int] NULL,
		 CONSTRAINT [PK_Admin_JewelsTrailsBSettings] PRIMARY KEY CLUSTERED 
		(
			[AdminJTBSettingID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_JewelsTrailsBSettings]  WITH CHECK ADD  CONSTRAINT [FK_Admin_JewelsTrailsBSettings_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])
	
	END
	GO

--	==========================================================
--	Object: Admin_Settings  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_Settings]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[Admin_Settings](
			[AdminSettingID] [bigint] IDENTITY(1,1) NOT NULL,
			[AdminID] [bigint] NULL,
			[ReminderClearInterval] [bigint] NULL,
		 CONSTRAINT [PK_Admin_Settings] PRIMARY KEY CLUSTERED 
		(
			[AdminSettingID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[Admin_Settings]  WITH CHECK ADD  CONSTRAINT [FK_Admin_Settings_Admin] FOREIGN KEY([AdminID])
		REFERENCES [dbo].[Admin] ([AdminID])		
	
	END
	GO

--	==========================================================
--	Object: CTest_ScratchImage  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_ScratchImage]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_ScratchImage](
			[ScratchImageID] [bigint] IDENTITY(1,1) NOT NULL,
			[FigureName] [nvarchar](max) NULL,
			[FileName] [nvarchar](max) NULL,
			[CreatedOn] [datetime] NULL CONSTRAINT [DF_CTest_ScratchImage_CreatedOn]  DEFAULT (getutcdate()),
		 CONSTRAINT [PK_CTest_ScratchImage] PRIMARY KEY CLUSTERED 
		(
			[ScratchImageID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	
	END
	GO

--	==========================================================
--	Object: CTest_ScratchImageResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_ScratchImageResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_ScratchImageResult](
			[ScratchImageResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[ScratchImageID] [bigint] NOT NULL,
			[DrawnFigFileName] [nvarchar](max) NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[GameName] [nvarchar](max) NULL,
			[Point] [numeric](10, 2) NULL,
			[CreatedOn] [datetime] NULL,
			[Status] [tinyint] NULL,
			[IsNotificationGame] [bit] NULL CONSTRAINT [DF_CTest_ScratchImageResult_IsNotificationGame]  DEFAULT ((0)),
			[SpinWheelScore] [nvarchar](max) NULL,
			[AdminBatchSchID] [bigint] NULL,
		 CONSTRAINT [PK_CTest_ScratchImageResult] PRIMARY KEY CLUSTERED 
		(
			[ScratchImageResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_ScratchImageResult] ADD  CONSTRAINT [DF_CTest_ScratchImageResult_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

		ALTER TABLE [dbo].[CTest_ScratchImageResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_ScratchImageResult_CTest_ScratchImage] FOREIGN KEY([ScratchImageID])
		REFERENCES [dbo].[CTest_ScratchImage] ([ScratchImageID])

		ALTER TABLE [dbo].[CTest_ScratchImageResult]  WITH CHECK ADD  CONSTRAINT [FK_CTest_ScratchImageResult_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
		ALTER TABLE [dbo].[CTest_ScratchImageResult] ADD CONSTRAINT [FK_CTest_ScratchImageResult_Admin_BatchSchedule] FOREIGN KEY([AdminBatchSchID]) 
		REFERENCES [dbo].[Admin_BatchSchedule] ([AdminBatchSchID])

	END
	GO

--	==========================================================
--	Object: CTest_SpinWheelResult  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTest_SpinWheelResult]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[CTest_SpinWheelResult](
			[SpinWheelResultID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[StartTime] [datetime] NULL,
			[EndTime] [datetime] NULL,
			[CollectedStars] [nvarchar](max) NULL,			
			[DayStreak] [int] NULL,
			[StrakSpin] [tinyint] NULL,
			[GameDate] [datetime] NULL,
			[CreatedOn] [datetime] NULL,
		 CONSTRAINT [PK_CTest_SpinWheelResult] PRIMARY KEY CLUSTERED 
		(
			[SpinWheelResultID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

		ALTER TABLE [dbo].[CTest_SpinWheelResult] ADD  CONSTRAINT [DF_CTest_SpinWheelResult_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]

	END
	GO

--	==========================================================
--	Object: HealthKit_Parameters  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HealthKit_Parameters]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[HealthKit_Parameters](
			[HKParamID] [bigint] NOT NULL,
			[HKParamName] [nvarchar](max) NULL,
		 CONSTRAINT [PK_HealthKit_Parameters] PRIMARY KEY CLUSTERED 
		(
			[HKParamID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
		
	END
	GO

--	==========================================================
--	Object: HealthKit_ParamValues  |  Action: CREATE
--	==========================================================

	IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HealthKit_ParamValues]') AND type in (N'U'))
	BEGIN

		CREATE TABLE [dbo].[HealthKit_ParamValues](
			[HKParamValueID] [bigint] IDENTITY(1,1) NOT NULL,
			[UserID] [bigint] NOT NULL,
			[HKParamID] [bigint] NOT NULL,
			[Value] [nvarchar](max) NULL,
			[DateTime] [datetime] NULL,
		 CONSTRAINT [PK_HealthKit_ParamValues] PRIMARY KEY CLUSTERED 
		(
			[HKParamValueID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


		ALTER TABLE [dbo].[HealthKit_ParamValues]  WITH CHECK ADD  CONSTRAINT [FK_HealthKit_ParamValues_HealthKit_Parameters] FOREIGN KEY([HKParamID])
		REFERENCES [dbo].[HealthKit_Parameters] ([HKParamID])

		ALTER TABLE [dbo].[HealthKit_ParamValues]  WITH CHECK ADD  CONSTRAINT [FK_HealthKit_ParamValues_Users] FOREIGN KEY([UserID])
		REFERENCES [dbo].[Users] ([UserID])
		
	END
	GO

--	==========================================================
--	********************   SEED DATA   ***********************
--	==========================================================
	
	IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin])
	BEGIN
		SET IDENTITY_INSERT [dbo].[Admin] ON 
		INSERT [dbo].[Admin] ([AdminID], [Email], [Password], [FirstName], [LastName], [CreatedOn], [EditedOn], [IsDeleted], [AdminType]) VALUES (1, N'AgcFhxt8/OVvH3UcZU41sXIoE03DxGH9d9NRavaS3Ss=', N'/owv3wdDDHISMWd41+kUP+JAJMSV0OTS/5KmMVGVZPMFfL4XBJCYaUdNQd+9Q5vV', N'THamEEtomA4Goj9paPT8/Q==', N'uZjbP6lfljcSjF5sqRCmCA==', CAST(N'2017-01-15 23:13:00.380' AS DateTime), CAST(N'2017-11-15 11:20:37.733' AS DateTime), 0, 1)
		SET IDENTITY_INSERT [dbo].[Admin] OFF
	END
	GO

	IF NOT EXISTS(SELECT TOP 1 1 FROM [Admin_Settings])
	BEGIN
		INSERT [dbo].[Admin_Settings]([AdminID], [ReminderClearInterval]) VALUES(1, 1)
	END
	GO

	IF NOT EXISTS(SELECT TOP 1 1 FROM [Slot])
	BEGIN
		INSERT [dbo].[Slot] ([SlotID], [SlotName], [IsDefault]) VALUES (1, N'Morning', 1)
		INSERT [dbo].[Slot] ([SlotID], [SlotName], [IsDefault]) VALUES (2, N'Afternoon', 0)
		INSERT [dbo].[Slot] ([SlotID], [SlotName], [IsDefault]) VALUES (3, N'Evening', 0)
	END
	GO

	IF NOT EXISTS(SELECT TOP 1 1 FROM [Repeat])
	BEGIN
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (1, N'Hourly', 0, 1)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (2, N'Every 3 hours', 1, 2)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (3, N'Every 6 hours', 0, 3)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (4, N'Every 12 hours', 0, 4)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (5, N'Daily (Mo Tu We Th Fr Sa)', 0, 5)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (6, N'Bi Weekly (Tu Th)', 0, 6)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (7, N'Tri Weekly (Mo We Fr)', 0, 7)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (8, N'Weekly', 0, 8)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (9, N'Bi Monthly', 0, 9)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (10, N'Monthly', 0, 10)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (11, N'Custom', 0, 11)
		INSERT [dbo].[Repeat] ([RepeatID], [RepeatInterval], [IsDefault], [SortOrder]) VALUES (12, N'None', 0, 12)
	END
	GO

	IF NOT EXISTS(SELECT TOP 1 1 FROM [CTest_3DFigure])
	BEGIN
		SET IDENTITY_INSERT [dbo].[CTest_3DFigure] ON 
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (1, N'Circle', N'circle.png', CAST(N'2017-02-06 07:01:28.847' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (2, N'Diamond', N'diamond.png', CAST(N'2017-02-06 07:01:56.830' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (3, N'Heart', N'heart.png', CAST(N'2017-02-06 07:02:52.927' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (4, N'Hexagon', N'hexagon.png', CAST(N'2017-02-06 07:03:15.473' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (5, N'Moon', N'moon.png', CAST(N'2017-02-06 07:03:35.867' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (6, N'Pentagon', N'pentagon.png', CAST(N'2017-03-06 06:40:14.150' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (7, N'Square', N'square.png', CAST(N'2017-03-06 06:40:33.383' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (8, N'Star', N'star.png', CAST(N'2017-03-06 06:40:37.820' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (9, N'Down Triangle', N'downtriangle.png', CAST(N'2017-03-06 06:40:45.523' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (10, N'Triangle', N'triangle.png', CAST(N'2017-03-06 06:40:50.320' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (11, N'SquareWithTriangle', N'squareWithTriangle.png', CAST(N'2017-05-09 06:01:04.337' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (12, N'Cross', N'cross.png', CAST(N'2017-05-09 06:04:14.200' AS DateTime))
		INSERT [dbo].[CTest_3DFigure] ([3DFigureID], [FigureName], [FileName], [CreatedOn]) VALUES (13, N'Cube', N'cube.png', CAST(N'2017-05-09 06:08:42.343' AS DateTime))
		SET IDENTITY_INSERT [dbo].[CTest_3DFigure] OFF
	END
	GO

	IF NOT EXISTS(SELECT TOP 1 1 FROM [CTest_ScratchImage])
	BEGIN
		SET IDENTITY_INSERT [dbo].[CTest_ScratchImage] ON 
		INSERT [dbo].[CTest_ScratchImage] ([ScratchImageID], [FigureName], [FileName], [CreatedOn]) VALUES (1, N'1', N'1.png', CAST(N'2018-09-19 11:32:17.787' AS DateTime))
		INSERT [dbo].[CTest_ScratchImage] ([ScratchImageID], [FigureName], [FileName], [CreatedOn]) VALUES (2, N'2', N'2.png', CAST(N'2018-09-19 11:32:31.130' AS DateTime))
		INSERT [dbo].[CTest_ScratchImage] ([ScratchImageID], [FigureName], [FileName], [CreatedOn]) VALUES (3, N'3', N'3.png', CAST(N'2018-09-28 04:34:38.933' AS DateTime))
		INSERT [dbo].[CTest_ScratchImage] ([ScratchImageID], [FigureName], [FileName], [CreatedOn]) VALUES (4, N'4', N'4.png', CAST(N'2018-09-28 04:34:51.717' AS DateTime))
		INSERT [dbo].[CTest_ScratchImage] ([ScratchImageID], [FigureName], [FileName], [CreatedOn]) VALUES (5, N'5', N'5.png', CAST(N'2018-09-28 04:34:56.293' AS DateTime))
		INSERT [dbo].[CTest_ScratchImage] ([ScratchImageID], [FigureName], [FileName], [CreatedOn]) VALUES (6, N'6', N'6.png', CAST(N'2018-09-28 04:35:00.120' AS DateTime))
		SET IDENTITY_INSERT [dbo].[CTest_ScratchImage] OFF
	END
	GO

	IF NOT EXISTS(SELECT TOP 1 1 FROM [CTest])
	BEGIN
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (1, N'n-back Test', 0, 0, 1, 15)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (2, N'trails-b Test', 0, 0, 2, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (3, N'Spatial Span Forward', 0, 0, 3, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (4, N'Spatial Span Backward', 0, 0, 4, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (5, N'Simple Memory Test', 1, 0, 5, 4)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (6, N'Serial 7s', 0, 0, 6, 6)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (7, N'Cats and Dogs', 0, 1, 7, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (8, N'3D Figure Copy', 0, 0, 8, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (9, N'Visual Association task', 1, 0, 9, 4)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (10, N'Digital Span Forward', 0, 0, 10, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (11, N'Cats and Dogs(New)', 0, 0, 11, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (12, N'Temporal Order', 1, 0, 12, 4)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (13, N'Digital Span Backward', 0, 0, 13, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (14, N'n-back Test (New)', 0, 0, 14, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (15, N'trails-b Test(New)', 0, 0, 15, 15)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (16, N'trails-b Test(DotTouch)', 0, 0, 16, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (17, N'Jewels Trails A', 0, 0, 17, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (18, N'Jewels Trails B', 0, 0, 18, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (19, N'Scratch Images', 0, 0, 19, NULL)
		INSERT [dbo].[CTest] ([CTestID], [CTestName], [IsDistractionSurveyRequired], [IsDeleted], [SortOrder], [MaxVersions]) VALUES (20, N'Spin Wheel', 0, 0, 20, NULL)
	END
	GO

	IF NOT EXISTS(SELECT TOP 1 1 FROM [HealthKit_Parameters])
	BEGIN
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (1, N'Height')
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (2, N'Weight')
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (3, N'Heart Rate')
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (4, N'Blood Pressure')
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (5, N'Respiratory Rate')
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (6, N'Sleep')
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (7, N'Steps')
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (8, N'Flight Climbed')
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (9, N'Segment')
		INSERT [dbo].[HealthKit_Parameters] ([HKParamID], [HKParamName]) VALUES (10, N'Distance')
	END
	GO


--	==========================================================
--	INDEXING
--	==========================================================
	




--	==========================================================
--	END OF SCRIPTS
--	==========================================================


COMMIT TRAN RELEASE	