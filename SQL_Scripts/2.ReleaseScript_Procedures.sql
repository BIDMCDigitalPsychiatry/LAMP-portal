
/*************************************************************************************************

	PROJECT: LAMP

	RELEASE SCRIPTS - FUNCTIONS & STORED PROCEDURES 

*************************************************************************************************/


--	==================================================================================================================================================
--	Object: Admin_GetBatchSchedule_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_GetBatchSchedule_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[Admin_GetBatchSchedule_sp]
GO

CREATE PROCEDURE [dbo].[Admin_GetBatchSchedule_sp] 
(
	@p_AdminBatchSchID	BIGINT,
	@p_ErrID			BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To get Batch Schedule by AdminBatchSchID
-----------------------------------------------------------------------------------------------  
Return values       :     0 - Success   
					  1000	- Unexpected Error        
-----------------------------------------------------------------------------------------------
Call syntax         :	
						DECLARE @p_ErrID INT
						EXEC [Admin_GetBatchSchedule_sp]							
								@p_AdminBatchSchID	= 1,
								@p_ErrID = @p_ErrID OUTPUT
						SELECT	@p_ErrID Error

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
07-SEP-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
	SET NOCOUNT ON       

	BEGIN TRY
	  
		SET @p_ErrID = 0
		
	--	Resultset(1): BatchSchedule

		SELECT	AdminBatchSchID,
				AdminID,
				BatchName,
				ScheduleDate,
				SlotID,
				[Time],
				RepeatID
		FROM Admin_BatchSchedule
		WHERE AdminBatchSchID = @p_AdminBatchSchID

	--	Resultset(2): Schedule-CTest

		SELECT	AdminBatchSchCTestID,
				CTestID,
				[Version]
		FROM Admin_BatchScheduleCTest
		WHERE AdminBatchSchID = @p_AdminBatchSchID

	--	Resultset(3): Schedule-Survey

		SELECT	AdminBatchSchSurveyID,
				SurveyID
		FROM Admin_BatchScheduleSurvey
		WHERE AdminBatchSchID = @p_AdminBatchSchID

	--	Resultset(4): Custom Time

		SELECT	AdminBatchSchCustTimID,
				[Time]
		FROM Admin_BatchScheduleCustomTime
		WHERE AdminBatchSchID = @p_AdminBatchSchID

		RETURN @p_ErrID  

	END TRY  

	BEGIN CATCH

		SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

		IF @@TRANCOUNT > 0 ROLLBACK TRAN

		SET @p_ErrID = 1000

		RETURN @p_ErrID

	END CATCH

	END /* [Admin_GetBatchSchedule_sp] */

GO

--	==================================================================================================================================================
--	Object: Admin_GetUserDataToExport_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_GetUserDataToExport_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[Admin_GetUserDataToExport_sp]
GO

CREATE PROCEDURE [dbo].[Admin_GetUserDataToExport_sp] 
(
	@p_UserIDsXML	XML,
	@p_DateFrom		DATETIME,
	@p_DateTo		DATETIME,
	@p_ErrID		BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To get Users' data to export to Excel
-----------------------------------------------------------------------------------------------  
Return values       :     0 - Success   
					  1000	- Unexpected Error        
-----------------------------------------------------------------------------------------------
Call syntax         :	
						DECLARE @p_ErrID INT
						EXEC [Admin_GetUserDataToExport_sp]
								@p_UserIDsXML= '<UserIDs>
												   <UserID>134</UserID>
												   <UserID>135</UserID>
												   <UserID>136</UserID>
												   <UserID>104</UserID>
												</UserIDs>',
								@p_DateFrom	= '2017-01-01 00:00:00',
								@p_DateTo	= '2018-03-31 23:59:59',
								@p_ErrID	= @p_ErrID OUTPUT
						SELECT	@p_ErrID Error

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
09-MAR-2018		Zco Engr
30-OCT-2018		Zco Engr							Added new games(CTest 19,20) and fields
************************************************************************************************/
BEGIN  
    
	SET NOCOUNT ON       

	DECLARE @l_UserIDs TABLE(UserID BIGINT)

	DECLARE @l_CTestResult TABLE
		(	
			CTestID				INT, 
			UserID				BIGINT, 
			TotalGames			INT, 
			TotalPoints			NUMERIC(10,2), 
			TimeTaken			INT, 
			LastTestDate		DATETIME,
			LastTestStartTime	DATETIME,
			LastTestEndTime		DATETIME
		)
				
	BEGIN TRY
	  
		SET @p_ErrID = 0
			
		INSERT @l_UserIDs
		SELECT Usr.U.value('.','BIGINT') FROM @p_UserIDsXML.nodes('UserIDs/UserID')Usr(U)
	
	--	Resultset(1): User's Details
	
		SELECT	U.UserID,
				U.StudyId, 
				U.FirstName,
				U.LastName,
				U.Email, 
				U.Phone, 
				U.RegisteredOn, 
				MAX(SR.StartTime) AS LastSurveyDate, 
				SUM(ISNULL(SR.Point,0)) AS Points
		FROM Users U
		LEFT JOIN SurveyResult SR ON U.UserID = SR.UserID
			AND (SR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
		WHERE U.UserID IN(SELECT UserID FROM @l_UserIDs)
		GROUP BY U.UserID, U.StudyId, U.FirstName,U.LastName, U.Email, U.Phone, U.RegisteredOn
		ORDER BY U.UserID

	--	Resultset(2): Survey Headers
	
		SELECT	SR.UserID,
				SR.SurveyResultID,
				SR.SurveyName,
				SR.CreatedOn,
				DATEDIFF(SECOND,SR.StartTime,SR.EndTime)AS TimeTaken,
				SR.StartTime,
				SR.EndTime,
				SR.IsDistraction,
				SR.IsNotificationGame,
				SR.SpinWheelScore,
				SR.AdminBatchSchID
		FROM SurveyResult SR
		WHERE SR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (SR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
		ORDER BY SR.UserID
	
	--	Resultset(3): Survey Questions And Answer Details

		SELECT	SRD.SurveyResultDtlID,				
				SR.UserID,
				SR.SurveyResultID,				
				SRD.Question,
				SRD.CorrectAnswer,
				SRD.EnteredAnswer,
				SRD.TimeTaken,
				SRD.ClickRange
		FROM SurveyResultDtl SRD 
		JOIN SurveyResult SR ON SRD.SurveyResultID = SR.SurveyResultID
			AND SR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (SR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	-- Getting Cognition Test Headers

	  ;WITH cteCTestResult
	   AS
	   (	
			SELECT	1 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_NBackResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	2 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_TrailsBResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	3 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_SpatialResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
				AND TR.[Type] = 1
			GROUP BY TR.UserID
			UNION ALL
			SELECT	4 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_SpatialResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
				AND TR.[Type] = 2
			GROUP BY TR.UserID
			UNION ALL
			SELECT	5 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_SimpleMemoryResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	6 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_Serial7Result TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	8 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_3DFigureResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	9 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_VisualAssociationResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	10 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_DigitSpanResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
				AND TR.[Type] = 1
			GROUP BY TR.UserID
			UNION ALL
			SELECT	11 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_CatAndDogNewResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	12 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_TemporalOrderResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	13 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_DigitSpanResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
				AND TR.[Type] = 2
			GROUP BY TR.UserID
			UNION ALL
			SELECT	14 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_NBackNewResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	15 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_TrailsBNewResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	16 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_TrailsBDotTouchResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID			
			UNION ALL
			SELECT	17 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_JewelsTrailsAResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	18 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_JewelsTrailsBResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	19 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					SUM(TR.Point) AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_ScratchImageResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
			UNION ALL
			SELECT	20 AS CTestID,
					TR.UserID,
					SUM(1) AS TotalGames,
					0 AS TotalPoints,
					SUM(DATEDIFF(SECOND,TR.StartTime,TR.EndTime))AS TimeTaken,
					MAX(TR.CreatedOn) AS LastTestDate,
					MAX(TR.StartTime) AS LastTestStartTime,
					MAX(TR.EndTime) AS LastTestEndTime
			FROM CTest_SpinWheelResult TR
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			GROUP BY TR.UserID
		)
		INSERT @l_CTestResult
			(	UserID, CTestID, TotalGames, TotalPoints, TimeTaken, LastTestDate, LastTestStartTime, LastTestEndTime)
		SELECT	UserID, CTestID, TotalGames, TotalPoints, TimeTaken, LastTestDate, LastTestStartTime, LastTestEndTime
		FROM cteCTestResult

	--	Resultset(4): Cognition Test Headers

		SELECT	R.UserID,
				R.CTestID,
				T.CTestName,
				R.TotalGames,
				R.TotalPoints,
				R.TimeTaken,
				R.LastTestDate,
				R.LastTestStartTime,
				R.LastTestEndTime
		FROM @l_CTestResult R
		JOIN CTest T ON R.CTestID = T.CTestID
		ORDER BY R.UserID, R.CTestID

	--  Resultset(5): Cognition Test Result -- CTest_NBackResult
					
		SELECT	1 AS CTestID,
				TR.UserID,
				TR.NBackResultID,
				TR.TotalQuestions,
				TR.CorrectAnswers,
				TR.WrongAnswers,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Version],
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_NBackResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(6): Cognition Test Result -- CTest_TrailsBResult

		SELECT	2 AS CTestID,
				TR.UserID,
				TR.TrailsBResultID,
				TR.TotalAttempts,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_TrailsBResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

		--	Resultset(7): Cognition Test Result Details -- CTest_TrailsBResultDtl

			SELECT	2 AS CTestID,
					TR.UserID,
					TR.TrailsBResultID,
					TRD.Alphabet,
					TRD.TimeTaken,
					TRD.[Status],TRD.[Sequence]
			FROM CTest_TrailsBResult TR
			JOIN CTest_TrailsBResultDtl TRD ON TR.TrailsBResultID = TRD.TrailsBResultID
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
				
	--  Resultset(8): Cognition Test Result -- CTest_SpatialResult(Forward)

		SELECT	3 AS CTestID,
				TR.UserID,
				TR.SpatialResultID,
				TR.CorrectAnswers,
				TR.WrongAnswers,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Type],
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_SpatialResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			AND TR.[Type] = 1

	--  Resultset(9): Cognition Test Result -- CTest_SpatialResult(Backward)

		SELECT	4 AS CTestID,
				TR.UserID,
				TR.SpatialResultID,
				TR.CorrectAnswers,
				TR.WrongAnswers,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Type],
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_SpatialResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			AND TR.[Type] = 2

	--  Resultset(10): Cognition Test Result -- CTest_SimpleMemoryResult
			
		SELECT	5 AS CTestID,
				TR.UserID,
				TR.SimpleMemoryResultID,
				TR.TotalQuestions,
				TR.CorrectAnswers,
				TR.WrongAnswers,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Version],
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_SimpleMemoryResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(11): Cognition Test Result -- CTest_Serial7Result

		SELECT	6 AS CTestID,
				TR.UserID,
				TR.Serial7ResultID,
				TR.TotalQuestions,
				TR.TotalAttempts,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Version],
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_Serial7Result TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(12): Cognition Test Result -- CTest_3DFigureResult

		SELECT	8 AS CTestID,
				TR.UserID,
				TR.[3DFigureResultID],
				TR.DrawnFigFileName,
				CT.[FileName],
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_3DFigureResult TR
		JOIN CTest_3DFigure CT ON TR.[3DFigureID] = CT.[3DFigureID]
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(13): Cognition Test Result -- CTest_VisualAssociationResult

		SELECT	9 AS CTestID,
				TR.UserID,
				TR.VisualAssocResultID,
				TR.TotalQuestions,
				TR.TotalAttempts,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Version],
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_VisualAssociationResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(14): Cognition Test Result -- CTest_DigitSpanResult(Forward)

		SELECT	10 AS CTestID,
				TR.UserID,
				TR.DigitSpanResultID,
				TR.CorrectAnswers,
				TR.WrongAnswers,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Type],
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_DigitSpanResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			--AND TR.[Type] = 1

	--  Resultset(15): Cognition Test Result -- CTest_CatAndDogNewResult

		SELECT	11 AS CTestID,
				TR.UserID,
				TR.CatAndDogNewResultID,
				TR.CorrectAnswers,
				TR.WrongAnswers,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_CatAndDogNewResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(16): Cognition Test Result -- CTest_TemporalOrderResult

		SELECT	12 AS CTestID,
				TR.UserID,
				TR.TemporalOrderResultID,
				TR.CorrectAnswers,
				TR.WrongAnswers,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Version],
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_TemporalOrderResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(17): Cognition Test Result -- CTest_DigitSpanResult(Backward)

		SELECT	13 AS CTestID,
				TR.UserID,
				TR.DigitSpanResultID,
				TR.CorrectAnswers,
				TR.WrongAnswers,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Type],
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_DigitSpanResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
			AND TR.[Type] = 2

	--  Resultset(18): Cognition Test Result -- CTest_NBackNewResult

		SELECT	14 AS CTestID,
				TR.UserID,
				TR.NBackNewResultID,
				TR.TotalQuestions,
				TR.CorrectAnswers,
				TR.WrongAnswers,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_NBackNewResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(19): Cognition Test Result -- CTest_TrailsBNewResult

		SELECT	15 AS CTestID,
				TR.UserID,
				TR.TrailsBNewResultID,
				TR.TotalAttempts,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.[Version],
				TR.IsNotificationGame,
				TR.AdminBatchSchID
		FROM CTest_TrailsBNewResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

		--	Resultset(20): Cognition Test Result Details -- CTest_TrailsBNewResultDtl

			SELECT	15 AS CTestID,
					TR.UserID,
					TR.TrailsBNewResultID,
					TRD.Alphabet,
					TRD.TimeTaken,
					TRD.[Status],TRD.[Sequence]
			FROM CTest_TrailsBNewResult TR
			JOIN CTest_TrailsBNewResultDtl TRD ON TR.TrailsBNewResultID = TRD.TrailsBNewResultID
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
				
	--  Resultset(21): Cognition Test Result -- CTest_TrailsBDotTouchResult

		SELECT	16 AS CTestID,
				TR.UserID,
				TR.TrailsBDotTouchResultID,
				TR.TotalAttempts,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_TrailsBDotTouchResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

		--	Resultset(22): Cognition Test Result -- CTest_TrailsBDotTouchResultDtl

			SELECT	16 AS CTestID,
					TR.UserID,
					TR.TrailsBDotTouchResultID,
					TR.TrailsBDotTouchResultID,
					TRD.Alphabet,
					TRD.TimeTaken,
					TRD.[Status],TRD.[Sequence]
			FROM CTest_TrailsBDotTouchResult TR
			JOIN CTest_TrailsBDotTouchResultDtl TRD ON TR.TrailsBDotTouchResultID = TRD.TrailsBDotTouchResultID
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(23): Cognition Test Result -- CTest_JewelsTrailsAResult

		SELECT	17 AS CTestID,
				TR.UserID,
				TR.JewelsTrailsAResultID,
				TR.TotalAttempts,
				TR.TotalJewelsCollected, 
				TR.TotalBonusCollected,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_JewelsTrailsAResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

		--	Resultset(24): Cognition Test Result Details -- CTest_JewelsTrailsAResultDtl

			SELECT	17 AS CTestID,
					TR.UserID,
					TR.JewelsTrailsAResultID,
					TRD.Alphabet,
					TRD.TimeTaken,
					TRD.[Status],TRD.[Sequence]
			FROM CTest_JewelsTrailsAResult TR
			JOIN CTest_JewelsTrailsAResultDtl TRD ON TR.JewelsTrailsAResultID = TRD.JewelsTrailsAResultID
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(25): Cognition Test Result -- CTest_JewelsTrailsBResult

		SELECT	18 AS CTestID,
				TR.UserID,
				TR.JewelsTrailsBResultID,
				TR.TotalAttempts,
				TR.TotalJewelsCollected, 
				TR.TotalBonusCollected,
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_JewelsTrailsBResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

		--	Resultset(26): Cognition Test Result Details -- CTest_JewelsTrailsBResultDtl

			SELECT	18 AS CTestID,
					TR.UserID,
					TR.JewelsTrailsBResultID,
					TRD.Alphabet,
					TRD.TimeTaken,
					TRD.[Status],TRD.[Sequence]
			FROM CTest_JewelsTrailsBResult TR
			JOIN CTest_JewelsTrailsBResultDtl TRD ON TR.JewelsTrailsBResultID = TRD.JewelsTrailsBResultID
			WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
				AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(27): Cognition Test Result -- CTest_ScratchImageResult

		SELECT	19 AS CTestID,
				TR.UserID,
				TR.ScratchImageResultID,
				TR.DrawnFigFileName,
				CT.[FileName],
				TR.StartTime,
				TR.EndTime,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration,
				TR.IsNotificationGame,
				TR.SpinWheelScore,
				TR.AdminBatchSchID
		FROM CTest_ScratchImageResult TR
		JOIN CTest_ScratchImage CT ON TR.ScratchImageID = CT.ScratchImageID
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(28): Cognition Test Result -- CTest_SpinWheelResult

		SELECT	20 AS CTestID,
				TR.UserID,
				TR.SpinWheelResultID,
				TR.StartTime,
				TR.EndTime,
				TR.CollectedStars,
				DATEDIFF(SECOND,TR.StartTime,TR.EndTime) AS Duration
		FROM CTest_SpinWheelResult TR
		WHERE TR.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (TR.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(27): Cognition Test Footer

		SELECT	R.UserID,
				MAX(R.LastTestStartTime)AS LastResult,
				COUNT(1) AS OverallRating
		FROM @l_CTestResult R
		GROUP BY R.UserID
		ORDER BY R.UserID

	--  Resultset(28): Help Call History
		
		SELECT	HC.HelpCallID,
				HC.UserID,
				(CASE HC.[Type]	 WHEN  1 THEN 'Emergency'
								 WHEN  2 THEN 'Personal Help Line' 
				END)AS CallType,
				HC.CalledNumber,
				HC.CallDateTime,
				HC.CallDuraion
		FROM HelpCalls HC
		WHERE HC.UserID IN(SELECT UserID FROM @l_UserIDs) 
			AND (HC.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(29): Locations

		SELECT	L.LocationID,
				L.UserID,
				L.LocationName,
				L.[Address],
				L.CreatedOn,
				L.Latitude,
				L.Longitude
		FROM Locations L
		WHERE L.UserID IN(SELECT UserID FROM @l_UserIDs) 
			AND L.[Type] = 1
			AND (L.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(30): Environment

		SELECT	L.LocationID,
				L.UserID,
				L.LocationName,
				L.[Address],
				L.CreatedOn,
				L.Latitude,
				L.Longitude
		FROM Locations L
		WHERE L.UserID IN(SELECT UserID FROM @l_UserIDs) 
			AND L.[Type] = 2
			AND (L.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)

	--  Resultset(31): Health-kit BasicInfo

		SELECT	HB.HKBasicInfoID,
				HB.UserID,
				HB.DateOfBirth,
				HB.Sex,
				HB.BloodType
		FROM HealthKit_BasicInfo HB
		WHERE HB.UserID IN(SELECT UserID FROM @l_UserIDs) 
			AND (HB.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo)
		ORDER BY HB.UserID

	--  Resultset(32): Health-kit DailyValues

		SELECT	HD.HKDailyValueID,
				HD.UserID,
				HD.CreatedOn,
				HD.Height,
				HD.[Weight],
				HD.HeartRate,
				HD.BloodPressure,
				HD.RespiratoryRate,
				HD.Sleep,
				HD.Steps,
				HD.FlightClimbed,
				HD.Segment,
				HD.Distance
		FROM HealthKit_DailyValues HD
		WHERE HD.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (HD.CreatedOn BETWEEN @p_DateFrom AND @p_DateTo) 
		ORDER BY HD.UserID

	--  Resultset(33): Health-kit ParamValues

		SELECT	V.HKParamValueID, 
				V.UserID,
				V.DateTime,
				CAST(V.DateTime AS DATE)AS [Date],
				CAST(V.DateTime AS TIME)AS [Time],
				P.HKParamID, 
				P.HKParamName, 
				V.Value
		FROM HealthKit_ParamValues V
		JOIN HealthKit_Parameters P ON V.HKParamID = P.HKParamID
		WHERE V.UserID IN(SELECT UserID FROM @l_UserIDs)
			AND (V.DateTime BETWEEN @p_DateFrom AND @p_DateTo) 
		ORDER BY V.UserID, V.DateTime, P.HKParamID

		RETURN @p_ErrID  

	END TRY  

	BEGIN CATCH

		SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

		IF @@TRANCOUNT > 0 ROLLBACK TRAN

		SET @p_ErrID = 1000

		RETURN @p_ErrID

	END CATCH

	END /* [Admin_GetUserDataToExport_sp] */

GO

--	==================================================================================================================================================
--	Object: Admin_SaveBatchSchedule_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_SaveBatchSchedule_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[Admin_SaveBatchSchedule_sp]
GO

CREATE PROCEDURE [dbo].[Admin_SaveBatchSchedule_sp] 
(
	@p_AdminBatchSchID	BIGINT = 0,
	@p_AdminID			BIGINT,
	@p_BatchName		VARCHAR(MAX),
	@p_ScheduleDate		DATETIME = NULL,
	@p_SlotID			BIGINT = NULL,
	@p_Time				DATETIME = NULL,
	@p_RepeatID			BIGINT,
	@p_CTestXML			XML = NULL,
	@p_SurveyXML		XML = NULL,
	@p_CustomTimeXML	XML = NULL,
	@p_ErrID			BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To Save/Update 
-----------------------------------------------------------------------------------------------  
Return Values       :    0 - Success   
					  1000 - Unexpected Error
					  1003 - BatchName Already Exists
-----------------------------------------------------------------------------------------------
Call Syntax         :	
						DECLARE @p_ErrID BIGINT

						EXEC [Admin_SaveBatchSchedule_sp]
								@p_AdminBatchSchID	= 0,
								@p_AdminID			= 1,
								@p_BatchName		= 'Batch-1',								
								@p_ScheduleDate		= '2018-02-15 10:00:00',
								@p_SlotID			= NULL,
								@p_Time				= NULL,
								@p_RepeatID			= 11,
								@p_CTestXML			= '<CTests>
														  <CTest>
															 <CTestID>1</CTestID>
															 <Version>1</Version>
															 <Order>1</Order>
														  </CTest>
														  <CTest>
															 <CTestID>2</CTestID>
															 <Version>1</Version>
															 <Order>2</Order>
														  </CTest>
													   </CTests>',
								@p_SurveyXML		= '<Surveys>
														  <Survey>
														     <SurveyID>1</SurveyID>
															  <Order>3</Order>
														  </Survey>
														  <Survey>
															 <SurveyID>2</SurveyID>
															  <Order>4</Order>
														  </Survey>
													   </Surveys>',
								@p_CustomTimeXML	= '<CustomTimes>
														  <CustomTime>2018-02-15 10:15:00</CustomTime>
														  <CustomTime>2018-02-15 10:30:00</CustomTime>
														  <CustomTime>2018-02-15 10:45:00</CustomTime>
													   </CustomTimes>',
								@p_ErrID			= @p_ErrID OUTPUT

						SELECT	@p_ErrID Error

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
14-FEB-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
SET NOCOUNT ON       

BEGIN TRY  

	SET @p_ErrID = 0

--  Checking Duplication

	IF EXISTS(SELECT TOP 1 1 
				FROM Admin_BatchSchedule 
				WHERE AdminID = @p_AdminID 
					AND BatchName = @p_BatchName
					AND AdminBatchSchID <> @p_AdminBatchSchID
					AND IsDeleted = 0)
	BEGIN
		SET @p_ErrID = 1003 -- BatchName Already Exists
		RETURN @p_ErrID
	END

/*	-------------------------------------------------------------------------------------------
	Adding New Batch Schedule
	-------------------------------------------------------------------------------------------	*/

	IF @p_AdminBatchSchID = 0
	BEGIN
			
		BEGIN TRAN
					
		INSERT Admin_BatchSchedule
				(				
					AdminID,
					BatchName,
					ScheduleDate,
					SlotID,
					[Time],
					RepeatID,
					CreatedOn
				)
		VALUES	(	
					@p_AdminID,
					@p_BatchName,
					@p_ScheduleDate,
					@p_SlotID,
					@p_Time,
					@p_RepeatID,
					GETUTCDATE()
				)

		SET @p_AdminBatchSchID = SCOPE_IDENTITY()

		-- CTests
		INSERT Admin_BatchScheduleCTest
			(	AdminBatchSchID, CTestID, [Version], [Order])
		SELECT	@p_AdminBatchSchID,
				Tst.T.value('CTestID[1]','BIGINT') AS CTestID,
				Tst.T.value('Version[1]','INT') AS [Version],
				Tst.T.value('Order[1]','INT') AS [Order]
		FROM @p_CTestXML.nodes('CTests/CTest')Tst(T)

		-- Surveys
		INSERT Admin_BatchScheduleSurvey
			(	AdminBatchSchID, SurveyID, [Order])
		SELECT	@p_AdminBatchSchID, 
				Srv.S.value('SurveyID[1]','BIGINT') AS SurveyID,
				Srv.S.value('Order[1]','INT') AS [Order]
		FROM @p_SurveyXML.nodes('Surveys/Survey')Srv(S)

		-- For Repeat = Custom Only
		INSERT Admin_BatchScheduleCustomTime
			(	AdminBatchSchID, [Time])
		SELECT	@p_AdminBatchSchID, Tim.T.value('.','DATETIME') AS [Time]
		FROM @p_CustomTimeXML.nodes('CustomTimes/CustomTime')Tim(T)
					
		COMMIT TRAN
			
	END

/*	-------------------------------------------------------------------------------------------
	Updating Bacth Schedule
	-------------------------------------------------------------------------------------------	*/

	ELSE IF @p_AdminBatchSchID > 0
	BEGIN
			
		BEGIN TRAN

		UPDATE Admin_BatchSchedule
		SET		AdminID			= @p_AdminID,
				BatchName		= @p_BatchName,
				ScheduleDate	= @p_ScheduleDate,
				SlotID			= @p_SlotID,
				[Time]			= @p_Time,
				RepeatID		= @p_RepeatID,
				EditedOn		= GETUTCDATE()
		WHERE AdminBatchSchID = @p_AdminBatchSchID

		-- CTests
		DELETE Admin_BatchScheduleCTest WHERE AdminBatchSchID = @p_AdminBatchSchID
		INSERT Admin_BatchScheduleCTest
			(	AdminBatchSchID, CTestID, [Version], [Order])
		SELECT	@p_AdminBatchSchID,
				Tst.T.value('CTestID[1]','BIGINT') AS CTestID,
				Tst.T.value('Version[1]','INT') AS [Version],
				Tst.T.value('Order[1]','INT') AS [Order]
		FROM @p_CTestXML.nodes('CTests/CTest')Tst(T)

		-- Surveys
		DELETE Admin_BatchScheduleSurvey WHERE AdminBatchSchID = @p_AdminBatchSchID
		INSERT Admin_BatchScheduleSurvey
			(	AdminBatchSchID, SurveyID, [Order])
		SELECT	@p_AdminBatchSchID, 
				Srv.S.value('SurveyID[1]','BIGINT') AS SurveyID,
				Srv.S.value('Order[1]','INT') AS [Order]
		FROM @p_SurveyXML.nodes('Surveys/Survey')Srv(S)

		-- For Repeat = Custom Only
		DELETE Admin_BatchScheduleCustomTime WHERE AdminBatchSchID = @p_AdminBatchSchID
		INSERT Admin_BatchScheduleCustomTime
			(	AdminBatchSchID, [Time])
		SELECT	@p_AdminBatchSchID, Tim.T.value('.','DATETIME') AS [Time]
		FROM @p_CustomTimeXML.nodes('CustomTimes/CustomTime')Tim(T)

		COMMIT TRAN

	END

/*	------------------------------------------------------------------------------------------- */
	
	RETURN @p_ErrID

END TRY  

BEGIN CATCH

	SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

	IF @@TRANCOUNT > 0 
	ROLLBACK TRAN

	SET @p_ErrID = 1000

	RETURN @p_ErrID

END CATCH

END /* [Admin_SaveBatchSchedule_sp] */


GO

--	==================================================================================================================================================
--	Object: Admin_SaveCTestSchedule_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_SaveCTestSchedule_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[Admin_SaveCTestSchedule_sp]
GO

CREATE PROCEDURE [dbo].[Admin_SaveCTestSchedule_sp] 
(
	@p_AdminCTestSchID	BIGINT = 0,
	@p_AdminID			BIGINT,
	@p_CTestID			BIGINT,
	@p_Version			INT = NULL,
	@p_ScheduleDate		DATETIME = NULL,
	@p_SlotID			BIGINT = NULL,
	@p_Time				DATETIME = NULL,
	@p_RepeatID			BIGINT,
	@p_CustomTimeXML	XML = NULL,
	@p_ErrID			BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To Save/Update 
-----------------------------------------------------------------------------------------------  
Return Values       :     0 - Success   
					  1000	- Unexpected Error
					  1001  - CTest Schedule Already Exists
-----------------------------------------------------------------------------------------------
Call Syntax         :	
						DECLARE @p_ErrID BIGINT

						EXEC [Admin_SaveCTestSchedule_sp]
								@p_AdminCTestSchID	= 0,
								@p_AdminID			= 1,
								@p_CTestID			= 1,
								@p_Version			= 1,
								@p_ScheduleDate		= '2018-02-15 10:00:00',
								@p_SlotID			= NULL,
								@p_Time				= NULL,
								@p_RepeatID			= 11,
								@p_CustomTimeXML	= '<CustomTimes>
														  <CustomTime>2018-02-15 10:15:00</CustomTime>
														  <CustomTime>2018-02-15 10:30:00</CustomTime>
														  <CustomTime>2018-02-15 10:45:00</CustomTime>
													   </CustomTimes>',
								@p_ErrID			= @p_ErrID OUTPUT

						SELECT	@p_ErrID Error

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
14-FEB-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
SET NOCOUNT ON       

BEGIN TRY  

	SET @p_ErrID = 0

--  Checking Duplication

	IF EXISTS(SELECT TOP 1 1 
				FROM Admin_CTestSchedule 
				WHERE AdminID = @p_AdminID 
					AND CTestID = @p_CTestID 
					AND ISNULL([Version],0) = ISNULL(@p_Version,0) 
					AND AdminCTestSchID <> @p_AdminCTestSchID  
					AND IsDeleted = 0)
	BEGIN
		SET @p_ErrID = 1001 -- CTest Schedule Already Exists
		RETURN @p_ErrID
	END

/*	-------------------------------------------------------------------------------------------
	Adding New Schedule
	-------------------------------------------------------------------------------------------	*/

	IF @p_AdminCTestSchID = 0
	BEGIN
			
		BEGIN TRAN
					
		INSERT Admin_CTestSchedule
				(				
					AdminID,
					CTestID,
					[Version],
					ScheduleDate,
					SlotID,
					[Time],
					RepeatID,
					CreatedOn
				)
		VALUES	(	
					@p_AdminID,
					@p_CTestID,
					@p_Version,
					@p_ScheduleDate,
					@p_SlotID,
					@p_Time,
					@p_RepeatID,
					GETUTCDATE()
				)

		SET @p_AdminCTestSchID = SCOPE_IDENTITY()
		
		-- For Repeat = Custom Only
		INSERT Admin_CTestScheduleCustomTime
			(	AdminCTestSchID, [Time])
		SELECT	@p_AdminCTestSchID, Tim.T.value('.','DATETIME') AS [Time]
		FROM @p_CustomTimeXML.nodes('CustomTimes/CustomTime')Tim(T)
					
		COMMIT TRAN
			
	END

/*	-------------------------------------------------------------------------------------------
	Updating Schedule
	-------------------------------------------------------------------------------------------	*/

	ELSE IF @p_AdminCTestSchID > 0
	BEGIN
			
		BEGIN TRAN

		UPDATE Admin_CTestSchedule
		SET		AdminID			= @p_AdminID,
				CTestID			= @p_CTestID,
				[Version]		= @p_Version,
				ScheduleDate	= @p_ScheduleDate,
				SlotID			= @p_SlotID,
				Time			= @p_Time,
				RepeatID		= @p_RepeatID,
				EditedOn		= GETUTCDATE()
		WHERE AdminCTestSchID = @p_AdminCTestSchID


		-- For Repeat = Custom Only
		DELETE Admin_CTestScheduleCustomTime WHERE AdminCTestSchID = @p_AdminCTestSchID

		INSERT Admin_CTestScheduleCustomTime
			(	AdminCTestSchID, [Time])
		SELECT	@p_AdminCTestSchID, Tim.T.value('.','DATETIME') AS [Time]
		FROM @p_CustomTimeXML.nodes('CustomTimes/CustomTime')Tim(T)

		COMMIT TRAN

	END

/*	------------------------------------------------------------------------------------------- */
	
	RETURN @p_ErrID

END TRY  

BEGIN CATCH

	SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

	IF @@TRANCOUNT > 0 
	ROLLBACK TRAN

	SET @p_ErrID = 1000

	RETURN @p_ErrID

END CATCH

END /* [Admin_SaveCTestSchedule_sp] */


GO

--	==================================================================================================================================================
--	Object: Admin_SaveSurveySchedule_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_SaveSurveySchedule_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[Admin_SaveSurveySchedule_sp]
GO

CREATE PROCEDURE [dbo].[Admin_SaveSurveySchedule_sp] 
(
	@p_AdminSurveySchID	BIGINT = 0,
	@p_AdminID			BIGINT,
	@p_SurveyID			BIGINT,
	@p_ScheduleDate		DATETIME = NULL,
	@p_SlotID			BIGINT = NULL,
	@p_Time				DATETIME = NULL,
	@p_RepeatID			BIGINT,
	@p_CustomTimeXML	XML = NULL,
	@p_ErrID			BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To Save/Update 
-----------------------------------------------------------------------------------------------  
Return Values       :     0 - Success   
					  1000	- Unexpected Error
					  1002  - Survey Schedule Already Exists
-----------------------------------------------------------------------------------------------
Call Syntax         :	
						DECLARE @p_ErrID BIGINT
						EXEC [Admin_SaveSurveySchedule_sp]
								@p_AdminSurveySchID = 0,
								@p_AdminID			= 1,
								@p_SurveyID			= 1,
								@p_ScheduleDate		= '2018-02-15 10:00:00',
								@p_SlotID			= NULL,
								@p_Time				= NULL,
								@p_RepeatID			= 11,
								@p_CustomTimeXML	= '<CustomTimes>
														  <CustomTime>2018-02-15 10:15:00</CustomTime>
														  <CustomTime>2018-02-15 10:30:00</CustomTime>
														  <CustomTime>2018-02-15 10:45:00</CustomTime>
													   </CustomTimes>',
								@p_ErrID			= @p_ErrID OUTPUT
						SELECT	@p_ErrID Error

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
16-FEB-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
SET NOCOUNT ON       

BEGIN TRY  

	SET @p_ErrID = 0

--  Checking Duplication

	IF EXISTS(SELECT TOP 1 1 
				FROM Admin_SurveySchedule 
				WHERE AdminID = @p_AdminID 
					AND SurveyID = @p_SurveyID 
					AND AdminSurveySchID <> @p_AdminSurveySchID 
					AND IsDeleted = 0)
	BEGIN
		SET @p_ErrID = 1002 -- Survey Schedule Already Exists
		RETURN @p_ErrID
	END

/*	-------------------------------------------------------------------------------------------
	Adding New Schedule
	-------------------------------------------------------------------------------------------	*/

	IF @p_AdminSurveySchID = 0
	BEGIN
			
		BEGIN TRAN
					
		INSERT Admin_SurveySchedule
				(				
					AdminID,
					SurveyID,
					ScheduleDate,
					SlotID,
					[Time],
					RepeatID,
					CreatedOn
				)
		VALUES	(	
					@p_AdminID,
					@p_SurveyID,
					@p_ScheduleDate,
					@p_SlotID,
					@p_Time,
					@p_RepeatID,
					GETUTCDATE()
				)
		
		SET @p_AdminSurveySchID = SCOPE_IDENTITY()
		
		-- For Repeat = Custom Only
		INSERT Admin_SurveyScheduleCustomTime
			(	AdminSurveySchID, [Time])
		SELECT	@p_AdminSurveySchID, Tim.T.value('.','DATETIME') AS [Time]
		FROM @p_CustomTimeXML.nodes('CustomTimes/CustomTime')Tim(T)
					
		COMMIT TRAN
			
	END

/*	-------------------------------------------------------------------------------------------
	Updating Schedule
	-------------------------------------------------------------------------------------------	*/

	ELSE IF @p_AdminSurveySchID > 0
	BEGIN
			
		BEGIN TRAN

		UPDATE Admin_SurveySchedule
		SET		AdminID			= @p_AdminID,
				SurveyID			= @p_SurveyID,
				ScheduleDate	= @p_ScheduleDate,
				SlotID			= @p_SlotID,
				Time			= @p_Time,
				RepeatID		= @p_RepeatID,
				EditedOn		= GETUTCDATE()
		WHERE AdminSurveySchID = @p_AdminSurveySchID

		-- For Repeat = Custom Only
		DELETE Admin_SurveyScheduleCustomTime WHERE AdminSurveySchID = @p_AdminSurveySchID

		INSERT Admin_SurveyScheduleCustomTime
			(	AdminSurveySchID, [Time])
		SELECT	@p_AdminSurveySchID, Tim.T.value('.','DATETIME') AS [Time]
		FROM @p_CustomTimeXML.nodes('CustomTimes/CustomTime')Tim(T)

		COMMIT TRAN

	END

/*	------------------------------------------------------------------------------------------- */

	RETURN @p_ErrID

END TRY  

BEGIN CATCH

	SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

	IF @@TRANCOUNT > 0 
	ROLLBACK TRAN

	SET @p_ErrID = 1000

	RETURN @p_ErrID

END CATCH

END /* [Admin_SaveSurveySchedule_sp] */


GO

--	==================================================================================================================================================
--	Object: GetAdminBatchScheduleByUserID_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAdminBatchScheduleByUserID_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[GetAdminBatchScheduleByUserID_sp]
GO
 
CREATE PROCEDURE [dbo].[GetAdminBatchScheduleByUserID_sp] 
(
	@p_UserID			BIGINT,
	@p_LastFetchedTS	DATETIME = NULL,
	@p_LastUpdatedTS	DATETIME = NULL OUTPUT,
	@p_ErrID			BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To get Batch Schedule done by User's Admin
-----------------------------------------------------------------------------------------------  
Return values       :     0 - Success   
					  1000	- Unexpected Error        
-----------------------------------------------------------------------------------------------
Call syntax         :	
						DECLARE @p_ErrID INT,
								@p_LastUpdatedTS DATETIME
						EXEC [GetAdminBatchScheduleByUserID_sp] 
								@p_UserID		 = 173,			
								@p_LastFetchedTS = NULL,
								@p_LastUpdatedTS = @p_LastUpdatedTS OUTPUT,													
								@p_ErrID		 = @p_ErrID OUTPUT	 
						SELECT	@p_ErrID Error,
								@p_LastUpdatedTS LastUpdatedTS		 

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
12-FEB-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
	SET NOCOUNT ON       
	
	DECLARE @l_Schedule TABLE
	(
		ScheduleID		BIGINT,
		BatchName		NVARCHAR(MAX),
		ScheduleDate	DATETIME,
		[Time]			DATETIME,
		RepeatID		BIGINT,
		IsDeleted		BIT,
		LastUpdatedTS	DATETIME
	)

	BEGIN TRY  

		SET @p_ErrID = 0
				
	--	Getting Batch Schedule Details Created by Admin of this User

		IF @p_LastFetchedTS IS NULL -- On first call, all records will be returned excluding deleted ones
		BEGIN		 
			INSERT @l_Schedule
				(	ScheduleID,
					BatchName,
					ScheduleDate,
					[Time],				
					RepeatID,
					IsDeleted,
					LastUpdatedTS
				)
			SELECT	BS.AdminBatchSchID,
					BS.BatchName,
					BS.ScheduleDate,
					BS.[Time],				
					BS.RepeatID,
					BS.IsDeleted,
					ISNULL(BS.EditedOn,BS.CreatedOn)
			FROM Admin_BatchSchedule BS
			JOIN Users U ON BS.AdminID = U.AdminID
				AND U.UserID = @p_UserID
				AND BS.IsDeleted = 0
		END
		ELSE
		BEGIN		 
			INSERT @l_Schedule
				(	ScheduleID,
					BatchName,
					ScheduleDate,
					[Time],				
					RepeatID,
					IsDeleted,
					LastUpdatedTS
				)
			SELECT	BS.AdminBatchSchID,
					BS.BatchName,
					BS.ScheduleDate,
					BS.[Time],				
					BS.RepeatID,
					BS.IsDeleted,
					ISNULL(BS.EditedOn,BS.CreatedOn)
			FROM Admin_BatchSchedule BS
			JOIN Users U ON BS.AdminID = U.AdminID
				AND U.UserID = @p_UserID
				AND ISNULL(BS.EditedOn,BS.CreatedOn) > @p_LastFetchedTS
		END						
		
	--  Return Bacth Schedule

		SELECT	ScheduleID, 
				BatchName, 
				ScheduleDate, 
				[Time], 
				RepeatID, 
				IsDeleted
		FROM @l_Schedule

	--  Return Bacth Schedule - CTest

		SELECT	BS.ScheduleID,
				C.CTestID,
				C.[Version],
				C.[Order]
		FROM @l_Schedule BS
		JOIN Admin_BatchScheduleCTest C ON BS.ScheduleID = C.AdminBatchSchID

	--  Return Bacth Schedule - Survey

		SELECT	BS.ScheduleID,
				S.SurveyID,
				S.[Order]
		FROM @l_Schedule BS
		JOIN Admin_BatchScheduleSurvey S ON BS.ScheduleID = S.AdminBatchSchID

	--  Return Bacth Schedule - Custom Time

		SELECT	BS.ScheduleID,
				CT.[Time]
		FROM @l_Schedule BS
		JOIN Admin_BatchScheduleCustomTime CT ON BS.ScheduleID = CT.AdminBatchSchID

		
		SELECT @p_LastUpdatedTS = MAX(LastUpdatedTS) FROM @l_Schedule
			
		SELECT @p_LastUpdatedTS = ISNULL(@p_LastUpdatedTS,GETUTCDATE())

		RETURN @p_ErrID  

	END TRY  

	BEGIN CATCH

		SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

		IF @@TRANCOUNT > 0 ROLLBACK TRAN

		SET @p_ErrID = 1000

		RETURN @p_ErrID

	END CATCH

	END /* [GetAdminBatchScheduleByUserID_sp] */

GO

--	==================================================================================================================================================
--	Object: GetAdminCTestScheduleByUserID_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAdminCTestScheduleByUserID_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[GetAdminCTestScheduleByUserID_sp]
GO
 
CREATE PROCEDURE [dbo].[GetAdminCTestScheduleByUserID_sp] 
(
	@p_UserID			BIGINT,
	@p_LastFetchedTS	DATETIME = NULL,
	@p_LastUpdatedTS	DATETIME = NULL OUTPUT,
	@p_ErrID			BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To get CTest Schedule done by User's Admin
-----------------------------------------------------------------------------------------------  
Return values       :     0 - Success   
					  1000	- Unexpected Error        
-----------------------------------------------------------------------------------------------
Call syntax         :	
						DECLARE @p_ErrID INT,
								@p_LastUpdatedTS DATETIME
						EXEC [GetAdminCTestScheduleByUserID_sp] 
								@p_UserID		 = 173,			
								@p_LastFetchedTS = NULL,
								@p_LastUpdatedTS = @p_LastUpdatedTS OUTPUT,													
								@p_ErrID		 = @p_ErrID OUTPUT	 
						SELECT	@p_ErrID Error,
								@p_LastUpdatedTS LastUpdatedTS		 

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
12-FEB-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
	SET NOCOUNT ON       
	
	DECLARE @l_Schedule TABLE
	(
		ScheduleID		BIGINT,
		CTestID			BIGINT,		
		CTestName		NVARCHAR(MAX),
		[Version]		INT,
		ScheduleDate	DATETIME,
		[Time]			DATETIME,
		RepeatID		BIGINT,
		IsDeleted		BIT,
		LastUpdatedTS	DATETIME
	)

	BEGIN TRY  

		SET @p_ErrID = 0
				
	--	Getting CTest Schedule Details Created by Admin of this User

		IF @p_LastFetchedTS IS NULL -- On first call, all records will be returned excluding deleted ones
		BEGIN		 
			INSERT @l_Schedule
				(	ScheduleID,
					CTestID,
					CTestName,
					[Version],
					ScheduleDate,
					[Time],				
					RepeatID,
					IsDeleted,
					LastUpdatedTS
				)
			SELECT	CS.AdminCTestSchID,
					CS.CTestID,
					CT.CTestName,
					CS.[Version],
					CS.ScheduleDate,
					CS.[Time],				
					CS.RepeatID,
					CS.IsDeleted,
					ISNULL(CS.EditedOn,CS.CreatedOn)
			FROM Admin_CTestSchedule CS
			JOIN CTest CT ON CS.CTestID = CT.CTestID
			JOIN Users U ON CS.AdminID = U.AdminID
				AND U.UserID = @p_UserID
				AND CS.IsDeleted = 0
		END
		ELSE
		BEGIN		 
			INSERT @l_Schedule
				(	ScheduleID,
					CTestID,
					CTestName,
					[Version],
					ScheduleDate,
					[Time],				
					RepeatID,
					IsDeleted,
					LastUpdatedTS
				)
			SELECT	CS.AdminCTestSchID,
					CS.CTestID,
					CT.CTestName,
					CS.[Version],
					CS.ScheduleDate,
					CS.[Time],				
					CS.RepeatID,
					CS.IsDeleted,
					ISNULL(CS.EditedOn,CS.CreatedOn)
			FROM Admin_CTestSchedule CS
			JOIN CTest CT ON CS.CTestID = CT.CTestID
			JOIN Users U ON CS.AdminID = U.AdminID
				AND U.UserID = @p_UserID
				AND ISNULL(CS.EditedOn,CS.CreatedOn) > @p_LastFetchedTS
		END						
		
	--  Return Schedule

		SELECT	ScheduleID AS GameScheduleID, 
				CTestID AS CTestId, 
				CTestName, 
				[Version], 
				ScheduleDate, 
				[Time], 
				RepeatID, 
				IsDeleted
		FROM @l_Schedule

		SELECT	SS.ScheduleID,
				CT.[Time]
		FROM @l_Schedule SS
		JOIN Admin_CTestScheduleCustomTime CT ON SS.ScheduleID = CT.AdminCTestSchID
		
		SELECT @p_LastUpdatedTS = MAX(LastUpdatedTS) FROM @l_Schedule
			
		SELECT @p_LastUpdatedTS = ISNULL(@p_LastUpdatedTS,GETUTCDATE())

		RETURN @p_ErrID  

	END TRY  

	BEGIN CATCH

		SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

		IF @@TRANCOUNT > 0 ROLLBACK TRAN

		SET @p_ErrID = 1000

		RETURN @p_ErrID

	END CATCH

	END /* [GetAdminCTestScheduleByUserID_sp] */

GO

--	==================================================================================================================================================
--	Object: GetAdminSurveyScheduleByUserID_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAdminSurveyScheduleByUserID_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[GetAdminSurveyScheduleByUserID_sp]
GO

CREATE PROCEDURE [dbo].[GetAdminSurveyScheduleByUserID_sp] 
(
	@p_UserID			BIGINT,
	@p_LastFetchedTS	DATETIME = NULL,
	@p_LastUpdatedTS	DATETIME = NULL OUTPUT,
	@p_ErrID			BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To get Survey Schedule done by User's Admin
-----------------------------------------------------------------------------------------------  
Return values       :     0 - Success   
					  1000	- Unexpected Error        
-----------------------------------------------------------------------------------------------
Call syntax         :	
						DECLARE @p_ErrID INT,
								@p_LastUpdatedTS DATETIME
						EXEC [GetAdminSurveyScheduleByUserID_sp]
								@p_UserID		 = 173,			
								@p_LastFetchedTS = NULL,
								@p_LastUpdatedTS = @p_LastUpdatedTS OUTPUT,													
								@p_ErrID		 = @p_ErrID OUTPUT	 
						SELECT	@p_ErrID Error,
								@p_LastUpdatedTS LastUpdatedTS 

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
12-FEB-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
	SET NOCOUNT ON       

	DECLARE @l_Schedule TABLE
	(
		ScheduleID		BIGINT,
		SurveyID		BIGINT,		
		SurveyName		NVARCHAR(MAX),
		ScheduleDate	DATETIME,
		[Time]			DATETIME,
		RepeatID		BIGINT,
		IsDeleted		BIT,
		LastUpdatedTS	DATETIME
	)
		
	BEGIN TRY  

		SET @p_ErrID = 0
			
	--	Getting Survey Schedule Details Created by Admin of this User
		
		IF @p_LastFetchedTS IS NULL -- On first call, all records will be returned excluding deleted ones
		BEGIN
			INSERT @l_Schedule
				(	ScheduleID,
					SurveyID,
					SurveyName,
					ScheduleDate,
					[Time],
					RepeatID,
					IsDeleted,
					LastUpdatedTS
				)
			SELECT	SS.AdminSurveySchID,
					SS.SurveyID,
					SU.SurveyName,
					SS.ScheduleDate,
					SS.[Time],
					SS.RepeatID,
					SS.IsDeleted,
					ISNULL(SS.EditedOn,SS.CreatedOn)
			FROM Admin_SurveySchedule SS
			JOIN Survey SU ON SS.SurveyID = SU.SurveyID
			JOIN Users U ON SS.AdminID = U.AdminID
				AND U.UserID = @p_UserID
				AND SS.IsDeleted = 0
		END
		ELSE
		BEGIN
			INSERT @l_Schedule
					(	ScheduleID,
						SurveyID,
						SurveyName,
						ScheduleDate,
						[Time],
						RepeatID,
						IsDeleted,
						LastUpdatedTS
					)
			SELECT	SS.AdminSurveySchID,
					SS.SurveyID,
					SU.SurveyName,
					SS.ScheduleDate,
					SS.[Time],
					SS.RepeatID,
					SS.IsDeleted,
					ISNULL(SS.EditedOn,SS.CreatedOn)
			FROM Admin_SurveySchedule SS
			JOIN Survey SU ON SS.SurveyID = SU.SurveyID
			JOIN Users U ON SS.AdminID = U.AdminID
				AND U.UserID = @p_UserID
				AND ISNULL(SS.EditedOn,SS.CreatedOn) > @p_LastFetchedTS
		END

	--  Return Schedule

		SELECT	ScheduleID AS SurveyScheduleID, 
				SurveyID AS SurveyId, 
				SurveyName, 
				ScheduleDate, 
				[Time], 
				RepeatID, 
				IsDeleted
		FROM @l_Schedule
	
		SELECT	SS.ScheduleID,
				CT.[Time]
		FROM @l_Schedule SS
		JOIN Admin_SurveyScheduleCustomTime CT ON SS.ScheduleID = CT.AdminSurveySchID

		SELECT @p_LastUpdatedTS = MAX(LastUpdatedTS) FROM @l_Schedule

		SELECT @p_LastUpdatedTS = ISNULL(@p_LastUpdatedTS,GETUTCDATE())

		RETURN @p_ErrID  

	END TRY  

	BEGIN CATCH

		SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

		IF @@TRANCOUNT > 0 ROLLBACK TRAN

		SET @p_ErrID = 1000

		RETURN @p_ErrID

	END CATCH

	END /* [GetAdminSurveyScheduleByUserID_sp] */

GO

--	==================================================================================================================================================
--	Object: GetJewelsTrailsAResult_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJewelsTrailsAResult_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[GetJewelsTrailsAResult_sp]
GO

CREATE PROCEDURE [dbo].[GetJewelsTrailsAResult_sp] 
(
	@p_UserID	BIGINT,
	@p_ErrID	BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To get JewelsTrails_A Result for last 30 days
-----------------------------------------------------------------------------------------------  
Return values       :     0 - Success   
					  1000	- Unexpected Error        
-----------------------------------------------------------------------------------------------
Call syntax         :	
						DECLARE @p_ErrID INT
						EXEC [GetJewelsTrailsAResult_sp] 
								@p_UserID	= 173,								
								@p_ErrID	= @p_ErrID OUTPUT	 
						SELECT	@p_ErrID Error		 

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
29-JAN-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
	SET NOCOUNT ON       
	
	DECLARE @l_tblDateList TABLE([Date] DATE)

	BEGIN TRY  

		SET @p_ErrID = 0
					
	--	Taking list of dates from 30 days back upto this day
			
		DECLARE @l_FromDate DATE
		DECLARE @l_ToDate   DATE

		SET @l_FromDate = DATEADD(DAY,-29,CAST(GETUTCDATE()AS DATE)) -- Date before 30 days
		SET @l_ToDate	= CAST(GETUTCDATE() AS DATE) -- Current Date

		WHILE @l_FromDate <= @l_ToDate
		BEGIN	
			INSERT @l_tblDateList([Date])
			SELECT @l_FromDate
			SET @l_FromDate = DATEADD(DAY,1,@l_FromDate)
		END

	--	Getting JewelsTrails_A Result (Including dates with no transaction)

		SELECT	D.[Date],
				R.TotalJewelsCollected,
				R.TotalBonusCollected,
				R.Score
		FROM @l_tblDateList D
		LEFT JOIN CTest_JewelsTrailsAResult R ON D.[Date] = CAST(R.CreatedOn AS DATE)
			AND R.UserID = @p_UserID
			AND R.[Status] <> 1
		ORDER BY D.[Date]

		RETURN @p_ErrID  

	END TRY  

	BEGIN CATCH

		SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

		IF @@TRANCOUNT > 0 ROLLBACK TRAN

		SET @p_ErrID = 1000

		RETURN @p_ErrID

	END CATCH

	END /* [GetJewelsTrailsAResult_sp] */

GO

--	==================================================================================================================================================
--	Object: GetJewelsTrailsBResult_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJewelsTrailsBResult_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[GetJewelsTrailsBResult_sp]
GO

CREATE PROCEDURE [dbo].[GetJewelsTrailsBResult_sp] 
(
	@p_UserID	BIGINT,
	@p_ErrID	BIGINT = 0 OUTPUT			
)	     
AS   
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To get JewelsTrails_B Result for last 30 days
-----------------------------------------------------------------------------------------------  
Return values       :     0 - Success   
					  1000	- Unexpected Error        
-----------------------------------------------------------------------------------------------
Call syntax         :	
						DECLARE @p_ErrID INT
						EXEC [GetJewelsTrailsBResult_sp] 
								@p_UserID	= 173,								
								@p_ErrID	= @p_ErrID OUTPUT	 
						SELECT	@p_ErrID Error		 

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
29-JAN-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
	SET NOCOUNT ON       
	
	DECLARE @l_tblDateList TABLE([Date] DATE)

	BEGIN TRY  

		SET @p_ErrID = 0
					
	--	Taking list of dates from 30 days back upto this day
			
		DECLARE @l_FromDate DATE
		DECLARE @l_ToDate   DATE

		SET @l_FromDate = DATEADD(DAY,-29,CAST(GETUTCDATE()AS DATE)) -- Date before 30 days
		SET @l_ToDate	= CAST(GETUTCDATE() AS DATE) -- Current Date

		WHILE @l_FromDate <= @l_ToDate
		BEGIN	
			INSERT @l_tblDateList([Date])
			SELECT @l_FromDate
			SET @l_FromDate = DATEADD(DAY,1,@l_FromDate)
		END

	--	Getting JewelsTrails_B Result (Including dates with no transaction)

		SELECT	D.[Date],
				R.TotalJewelsCollected,
				R.TotalBonusCollected,
				R.Score
		FROM @l_tblDateList D
		LEFT JOIN CTest_JewelsTrailsBResult R ON D.[Date] = CAST(R.CreatedOn AS DATE)
			AND R.UserID = @p_UserID
			AND R.[Status] <> 1
		ORDER BY D.[Date]

		RETURN @p_ErrID  

	END TRY  

	BEGIN CATCH

		SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

		IF @@TRANCOUNT > 0 ROLLBACK TRAN

		SET @p_ErrID = 1000

		RETURN @p_ErrID

	END CATCH

	END /* [GetJewelsTrailsBResult_sp] */

GO

--	==================================================================================================================================================
--	Object: SaveHealthKitData_sp  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveHealthKitData_sp]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[SaveHealthKitData_sp]
GO

CREATE PROCEDURE [dbo].[SaveHealthKitData_sp] 
(
	@p_UserID			BIGINT,
	@p_DateOfBirth		DATETIME = NULL,
	@p_Sex				NVARCHAR(MAX) = NULL,
	@p_BloodType		NVARCHAR(MAX) = NULL,
	@p_Height			NVARCHAR(MAX) = NULL,
	@p_Weight			NVARCHAR(MAX) = NULL,
	@p_HeartRate		NVARCHAR(MAX) = NULL,
	@p_BloodPressure	NVARCHAR(MAX) = NULL,
	@p_RespiratoryRate	NVARCHAR(MAX) = NULL,
	@p_Sleep			NVARCHAR(MAX) = NULL,
	@p_Steps			NVARCHAR(MAX) = NULL,
	@p_FlightClimbed	NVARCHAR(MAX) = NULL,
	@p_Segment			NVARCHAR(MAX) = NULL,
	@p_Distance			NVARCHAR(MAX) = NULL,
	@p_ErrID			BIGINT = 0 OUTPUT			
)	     
AS		
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To Save/Update HealthKitData
-----------------------------------------------------------------------------------------------  
Return Values       :     0 - Success   
					  1000	- Unexpected Error
-----------------------------------------------------------------------------------------------
Call Syntax         :	
						DECLARE @p_ErrID BIGINT
						EXEC [SaveHealthKitData_sp]
								@p_UserID			= 1,
								@p_DateOfBirth		= '2000-01-01',
								@p_Sex				= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_BloodType		= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_Height			= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_Weight			= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_HeartRate		= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_BloodPressure	= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_RespiratoryRate	= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_Sleep			= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_Steps			= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_FlightClimbed	= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_Segment			= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_Distance			= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_ErrID			= @p_ErrID OUTPUT
						SELECT	@p_ErrID Error

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
29-MAY-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
SET NOCOUNT ON       

DECLARE @l_HKDailyValueID BIGINT

BEGIN TRY  

	SET @p_ErrID = 0

	BEGIN TRAN

--	Updating HealthKit_BasicInfo -- Keeping one reord per user
	IF EXISTS(SELECT 1 FROM HealthKit_BasicInfo WHERE UserID = @p_UserID)
	BEGIN
		UPDATE HealthKit_BasicInfo
		SET	DateOfBirth	= @p_DateOfBirth,
			Sex			= @p_Sex,
			BloodType	= @p_BloodType,
			EditedOn	= GETUTCDATE()
		WHERE UserID	= @p_UserID
	END
	ELSE
	BEGIN
		INSERT HealthKit_BasicInfo
			(UserID, DateOfBirth, Sex, BloodType)
		VALUES	
			(@p_UserID, @p_DateOfBirth, @p_Sex, @p_BloodType)
	END

--	Addding HealthKit_DailyValues (1 call per hour from App)

	INSERT HealthKit_DailyValues
		(	UserID,
			Height,
			[Weight],
			HeartRate,
			BloodPressure,
			RespiratoryRate,
			Sleep,
			Steps,
			FlightClimbed,
			Segment,
			Distance
		)
	VALUES	
		(	@p_UserID, 
			@p_Height, 
			@p_Weight, 
			@p_HeartRate,
			@p_BloodPressure,
			@p_RespiratoryRate,
			@p_Sleep,
			@p_Steps,
			@p_FlightClimbed,
			@p_Segment,
			@p_Distance
		)

--	Deleting HealthKit_DailyValues older than 90 days	
	DELETE HealthKit_DailyValues WHERE UserID = @p_UserID AND DATEDIFF(DAY,CreatedOn,GETUTCDATE()) > 90
		
	COMMIT TRAN

	RETURN @p_ErrID

END TRY  

BEGIN CATCH

	SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

	IF @@TRANCOUNT > 0 
	ROLLBACK TRAN

	SET @p_ErrID = 1000

	RETURN @p_ErrID

END CATCH

END /* [SaveHealthKitData_sp] */


GO

--	==================================================================================================================================================
--	Object: SaveHealthKitData_sp_v2  |  Action: DROP & CREATE
--	==================================================================================================================================================

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveHealthKitData_sp_v2]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[SaveHealthKitData_sp_v2]
GO

CREATE PROCEDURE [dbo].[SaveHealthKitData_sp_v2] 
(
	@p_UserID		BIGINT,
	@p_DateOfBirth	DATETIME,
	@p_Sex			NVARCHAR(MAX),
	@p_BloodType	NVARCHAR(MAX),
	@p_ValuesXML	XML,
	@p_ErrID		BIGINT = 0 OUTPUT		
)	     
AS		
/**********************************************************************************************
Project     	    : LAMP
Purpose				: To Save/Update HealthKitData
-----------------------------------------------------------------------------------------------  
Return Values       :     0 - Success   
					  1000	- Unexpected Error
-----------------------------------------------------------------------------------------------
Call Syntax         :	
						DECLARE @p_ErrID BIGINT
						EXEC [SaveHealthKitData_sp_v2]
								@p_UserID		= 1,
								@p_DateOfBirth	= '2000-01-01',
								@p_Sex			= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_BloodType	= 'UIevUhRgfrUoV+8HAFck5A==',
								@p_ValuesXML	= '<ParamValues>
														<ParamValue>
															<ParamID>1</ParamID>
															<Value>25</Value>
															<DateTime>2018-11-21 10:00:00</DateTime>
														</ParamValue>
														<ParamValue>
															<ParamID>2</ParamID>
															<Value>100</Value>
															<DateTime>2018-11-21 10:30:00</DateTime>
														</ParamValue>
														<ParamValue>
															<ParamID>2</ParamID>
															<Value>105</Value>
															<DateTime>2018-11-21 10:30:00</DateTime>
														</ParamValue>
													</ParamValues>',
								@p_ErrID		= @p_ErrID OUTPUT
						SELECT	@p_ErrID Error

************************************************************************************************
Change History
************************************************************************************************
Date			Author				Revision		Description
------------------------------------------------------------------------------------------------
29-MAY-2018		Zco Engr
************************************************************************************************/
BEGIN  
    
	SET NOCOUNT ON       

	BEGIN TRY

	SET @p_ErrID = 0

	BEGIN TRAN

--	Updating HealthKit_BasicInfo -- Keeping one reord per user
	IF EXISTS(SELECT 1 FROM HealthKit_BasicInfo WHERE UserID = @p_UserID)
	BEGIN
		UPDATE HealthKit_BasicInfo
		SET	DateOfBirth	= @p_DateOfBirth,
			Sex			= @p_Sex,
			BloodType	= @p_BloodType,
			EditedOn	= GETUTCDATE()
		WHERE UserID	= @p_UserID
	END
	ELSE
	BEGIN
		INSERT HealthKit_BasicInfo
			(UserID, DateOfBirth, Sex, BloodType)
		VALUES	
			(@p_UserID, @p_DateOfBirth, @p_Sex, @p_BloodType)
	END

--	Adding HealthKit_ParamValues
   ;WITH cteValues
	AS
	(	SELECT	@p_UserID								AS UserID,
				Val.V.value('ParamID[1]','BIGINT')		AS ParamID,
				Val.V.value('Value[1]','NVARCHAR(MAX)') AS Value,
				Val.V.value('DateTime[1]','DateTime')	AS [DateTime]
		FROM @p_ValuesXML.nodes('ParamValues/ParamValue')Val(V)
	)
	INSERT HealthKit_ParamValues
		(	UserID, HKParamID, Value, [DateTime])
	SELECT	UserID, ParamID, Value, [DateTime]
	FROM cteValues C
	WHERE NOT EXISTS(SELECT 1 FROM HealthKit_ParamValues HKV 
						WHERE HKV.UserID = C.UserID 
							AND HKV.HKParamID = C.ParamID 
							AND HKV.[DateTime] = C.[DateTime]
					)

--	Deleting HealthKit_ParamValues older than 90 days	
	DELETE HealthKit_ParamValues WHERE UserID = @p_UserID AND DATEDIFF(DAY,[DateTime],GETUTCDATE()) > 90
		
	COMMIT TRAN

	RETURN @p_ErrID

END TRY  

BEGIN CATCH

	SELECT  ERROR_NUMBER(),ERROR_MESSAGE()

	IF @@TRANCOUNT > 0 
	ROLLBACK TRAN

	SET @p_ErrID = 1000

	RETURN @p_ErrID

END CATCH

END /* [SaveHealthKitData_sp_v2] */


GO


/*
---------------------------------------------
END OF RELEASE SCRIPTS
---------------------------------------------
*/

