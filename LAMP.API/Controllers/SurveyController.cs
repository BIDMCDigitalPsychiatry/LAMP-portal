using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using LAMP.Service;
using LAMP.Utility;
using LAMP.ViewModel;

namespace LAMP.API.Controllers
{
    /// <summary>
    /// SurveyController is responsible for handling survey web api acitivities
    /// </summary>
    [Authorize]
    [RoutePrefix("api")]
    public class SurveyController : BaseController
    {
        #region Fields
        private ISurveyService _SurveyService;
        #endregion Fields

        #region Constructor
        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="surveyService">Survey Service</param>
        /// <param name="accountService">Account Service</param>
        /// <param name="authContext">Authentication context Service</param>
        public SurveyController(ISurveyService surveyService, IAccountService accountService, IAuthenticationContext authContext)
            : base(accountService, authContext)
        {
            _SurveyService = surveyService;
        }
        #endregion

        #region Survey
        /// <summary>
        /// Save User Survey details
        /// </summary>
        /// <param name="request">Survey request</param>
        /// <returns>Status</returns>
        [Route("SaveUserSurvey")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveUserSurvey(SurveyRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveUserSurvey(request));
            return response;
        }

        /// <summary>
        /// Get User Completed Survey details
        /// </summary>
        /// <param name="request">User Id</param>
        /// <returns>Completed Survey details</returns>
        [Route("GetUserCompletedSurvey")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(CompletedSurveyResponse))]
        public async Task<CompletedSurveyResponse> GetUserCompletedSurvey(CompletedSurveyRequest request)
        {
            var response = new CompletedSurveyResponse();
            APIResponseBase tokenResponse = ValidateUserToken();
            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.GetUserCompletedSurvey(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Get Survey Question and Answer
        /// </summary>
        /// <param name="request">SurveyQueAndAns request</param>
        /// <returns>Get Survey Question and Answer list</returns>
        [Route("GetSurveyQueAndAns")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(SurveyResponse))]
        public async Task<SurveyResponse> GetSurveyQueAndAns(SurveyQueAndAnsRequest request)
        {
            var response = new SurveyResponse();
            APIResponseBase tokenResponse = ValidateUserToken();
            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.GetSurveyQueAndAns(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Gets the surveys.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Get Survey details</returns>
        [Route("GetSurveys")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(SurveyQuestionsResponse))]
        public async Task<SurveyQuestionsResponse> GetSurveys(SurveyQuestionsRequest request)
        {
            var response = new SurveyQuestionsResponse();
            APIResponseBase tokenResponse = ValidateUserToken();
            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.GetSurveys(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }
        #endregion

        #region Location
        /// <summary>
        /// Save Location details
        /// </summary>
        /// <param name="request">Location request</param>
        /// <returns>Status</returns>
        [Route("SaveLocation")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveLocation(LocationRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveLocation(request));
            return response;
        }
        #endregion

        #region HelpCall
        /// <summary>
        /// Save HelpCall details
        /// </summary>
        /// <param name="request">HelpCall request</param>
        /// <returns>Status</returns>
        [Route("SaveHelpCall")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveHelpCall(HelpCallRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveHelpCall(request));
            return response;
        }
        #endregion

        #region Game
        /// <summary>
        /// Save Cat And Dog Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveCatAndDogGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveCatAndDogGame(CatAndDogGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveCatAndDogGame(request));
            return response;
        }

        /// <summary>
        /// Save Digit Span Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveDigitSpanGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveDigitSpanGame(DigitSpanGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveDigitSpanGame(request));
            return response;
        }

        /// <summary>
        /// Save nBack Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveNBackGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveNBackGame(NBackGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveNBackGame(request));
            return response;
        }

        /// <summary>
        /// Save Serial7 Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveSerial7Game")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveSerial7Game(Serial7GameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveSerial7Game(request));
            return response;
        }

        /// <summary>
        /// Save Simple Memory Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveSimpleMemoryGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveSimpleMemoryGame(SimpleMemoryGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveSimpleMemoryGame(request));
            return response;
        }

        /// <summary>
        /// Save TrailsB Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveTrailsBGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveTrailsBGame(TrailsBGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveTrailsBGame(request));
            return response;
        }

        /// <summary>
        /// Save Visual Association Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveVisualAssociationGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveVisualAssociationGame(VisualAssociationGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveVisualAssociationGame(request));
            return response;
        }

        /// <summary>
        /// Save 3DFigure Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("Save3DFigureGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> Save3DFigureGame(_3DFigureGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.Save3DFigureGame(request));
            return response;
        }

        /// <summary>
        /// Save Spatial Span Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveSpatialSpanGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveSpatialSpanGame(SpatialSpanGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveSpatialSpanGame(request));
            return response;
        }

        /// <summary>
        /// Save Cat and Dog New Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveCatAndDogNewGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveCatAndDogNewGame(CatAndDogNewGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveCatAndDogNewGame(request));
            return response;
        }

        /// <summary>
        /// Save nBack New Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveNBackGameNewGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveNBackGameNewGame(NBackNewGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveNBackNewGame(request));
            return response;
        }

        /// <summary>
        /// Save TrailsB New Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveTrailsBGameNew")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveTrailsBGameNew(TrailsBNewGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveTrailsBNewGame(request));
            return response;
        }

        /// <summary>
        /// Save Temporal Order Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveTemporalOrderGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveTemporalOrderGame(TemporalOrderGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveTemporalOrderGame(request));
            return response;
        }

        /// <summary>
        /// Save TrailsB New Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveTrailsBDotTouchGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveTrailsBDotTouchGame(TrailsBDotTouchGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveTrailsBDotTouchGame(request));
            return response;
        }

        /// <summary>
        /// Save Jewels Trails A Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveJewelsTrailsAGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveJewelsTrailsAGame(JewelsTrailsAGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveJewelsTrailsAGame(request));
            return response;
        }

        /// <summary>
        /// Save Jewels Trails B Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveJewelsTrailsBGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveJewelsTrailsBGame(JewelsTrailsBGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveJewelsTrailsBGame(request));
            return response;
        }

        /// <summary>
        /// Save Scratch Image Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveScratchImageGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveScratchImageGame(ScratchImageGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveScratchImageGame(request));
            return response;
        }

        /// <summary>
        /// Save Spin Wheel Game
        /// </summary>
        /// <param name="request">Game Request</param>
        /// <returns>Status</returns>
        [Route("SaveSpinWheelGame")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(APIResponseBase))]
        public async Task<APIResponseBase> SaveSpinWheelGame(SpinWheelGameRequest request)
        {
            APIResponseBase response = ValidateUserToken();
            if (response != null && response.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.SaveSpinWheelGame(request));
            return response;
        }
        #endregion

        #region Graph
        /// <summary>
        ///  Get the average and percentile score of a user for each game
        /// </summary>
        /// <param name="request">Graph Request</param>
        /// <returns>Game score</returns>
        [Route("GetGameScoresforGraph")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(GraphResponse))]
        public async Task<GraphResponse> GetGameScoresforGraph(GraphRequest request)
        {
            var response = new GraphResponse();
            APIResponseBase tokenResponse = ValidateUserToken();

            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.GetGameScoresforGraph(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Get Game High and Low Score for Graph
        /// </summary>
        /// <param name="request">Graph Request</param>
        /// <returns>Game High and Low Score</returns>
        [Route("GetGameHighAndLowScoreforGraph")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(GameGraphResponse))]
        public async Task<GameGraphResponse> GetGameHighAndLowScoreforGraph(GameGraphRequest request)
        {
            var response = new GameGraphResponse();
            APIResponseBase tokenResponse = ValidateUserToken();

            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.GetGameHighAndLowScoreforGraph(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }

        /// <summary>
        /// Get all Game Total Spin Wheel Score
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Spin Wheel Score</returns>
        [Route("GetAllGameTotalSpinWheelScore")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(GameTotalScoreResponse))]
        public async Task<GameTotalScoreResponse> GetAllGameTotalSpinWheelScore(GameTotalScoreRequest request)
        {
            var response = new GameTotalScoreResponse();
            APIResponseBase tokenResponse = ValidateUserToken();

            if (tokenResponse != null && tokenResponse.ErrorCode == (int)LAMPConstants.API_SUCCESS_CODE)
                response = await Task.Run(() => _SurveyService.GetAllGameTotalSpinWheelScore(request));
            else
            {
                response.ErrorCode = tokenResponse.ErrorCode;
                response.ErrorMessage = tokenResponse.ErrorMessage;
            }
            return response;
        }
        #endregion

    }

}
