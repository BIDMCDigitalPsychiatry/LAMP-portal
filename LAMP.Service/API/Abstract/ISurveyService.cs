using LAMP.ViewModel;
namespace LAMP.Service
{
    /// <summary>
    /// Interface ISurveyService for capable of class SurveyService
    /// </summary>
    public interface ISurveyService
    {
        #region Survey

        // <summary>
        /// Save User Survey
        /// </summary>
        /// <param name="request">Survey request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveUserSurvey(SurveyRequest request);

        /// <summary>
        /// Get User Completed Survey
        /// </summary>
        /// <param name="request">UserId</param>
        /// <returns> Completed survey list of that user</returns>
        CompletedSurveyResponse GetUserCompletedSurvey(CompletedSurveyRequest request);

        /// <summary>
        /// Get Survey Questions And Answers
        /// </summary>
        /// <param name="request"></param>
        /// <returns>survey Questions And Answers or the error message </returns>
        SurveyResponse GetSurveyQueAndAns(SurveyQueAndAnsRequest request);

        /// <summary>
        /// Gets the surveys.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        SurveyQuestionsResponse GetSurveys(SurveyQuestionsRequest request);

        #endregion

        #region Location and HelpCall

        /// <summary>
        /// Save Location
        /// </summary>
        /// <param name="request">Location request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveLocation(LocationRequest request);

        /// <summary>
        /// Save HelpCall
        /// </summary>
        /// <param name="request">HelpCall request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveHelpCall(HelpCallRequest request);

        #endregion

        #region Games

        /// <summary>
        /// Save Cat and dog Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveCatAndDogGame(CatAndDogGameRequest request);

        /// <summary>
        /// Save Digit Span Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveDigitSpanGame(DigitSpanGameRequest request);

        /// <summary>
        /// Save NBack Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveNBackGame(NBackGameRequest request);

        /// <summary>
        /// Saves the n back new game.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        APIResponseBase SaveNBackNewGame(NBackNewGameRequest request);

        /// <summary>
        /// Save Serial7 Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveSerial7Game(Serial7GameRequest request);

        /// <summary>
        /// Save Simple Memory Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveSimpleMemoryGame(SimpleMemoryGameRequest request);

        /// <summary>
        /// Save TrailsB Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveTrailsBGame(TrailsBGameRequest request);

        /// <summary>
        /// Save Visual Association Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveVisualAssociationGame(VisualAssociationGameRequest request);

        /// <summary>
        /// Save 3DFigure Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase Save3DFigureGame(_3DFigureGameRequest request);

        /// <summary>
        /// Save Spatial Span Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveSpatialSpanGame(SpatialSpanGameRequest request);

        /// <summary>
        /// Save Cat and Dog New Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveCatAndDogNewGame(CatAndDogNewGameRequest request);

        /// <summary>
        /// Save Temporal Order Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveTemporalOrderGame(TemporalOrderGameRequest request);

        /// <summary>
        /// Saves the trails b new game.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        APIResponseBase SaveTrailsBNewGame(TrailsBNewGameRequest request);

        /// <summary>
        ///  Get the average and percentile score of a user for each game
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GraphResponse GetGameScoresforGraph(GraphRequest request);

        /// <summary>
        /// Saves the trails b dot touch game.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        APIResponseBase SaveTrailsBDotTouchGame(TrailsBDotTouchGameRequest request);

        /// <summary>
        /// Saves the jewels trails a game.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        APIResponseBase SaveJewelsTrailsAGame(JewelsTrailsAGameRequest request);

        /// <summary>
        /// Saves the jewels trails b game.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        APIResponseBase SaveJewelsTrailsBGame(JewelsTrailsBGameRequest request);

        /// <summary>
        /// Save the Scratch image game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveScratchImageGame(ScratchImageGameRequest request);

        /// <summary>
        /// Save Spin Wheel game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        APIResponseBase SaveSpinWheelGame(SpinWheelGameRequest request);

        /// <summary>
        /// Get Game high and low score for Graph
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Game high and low score</returns>
        GameGraphResponse GetGameHighAndLowScoreforGraph(GameGraphRequest request);

        /// <summary>
        /// Get all Game total spin wheel score
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>All game total Spin Wheel Score</returns>
        GameTotalScoreResponse GetAllGameTotalSpinWheelScore(GameTotalScoreRequest request);
        #endregion
    }

}


