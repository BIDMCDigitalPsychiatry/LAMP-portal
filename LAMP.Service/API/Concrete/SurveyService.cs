using System;
using System.Collections.Generic;
using System.Linq;
using LAMP.DataAccess;
using LAMP.DataAccess.Entities;
using LAMP.Utility;
using LAMP.ViewModel;

namespace LAMP.Service
{
    /// <summary>
    /// Class SurveyService
    /// </summary>
    public class SurveyService : ISurveyService
    {
        #region Variables
        private IUnitOfWork _UnitOfWork;
        private IAccountService _AccountService;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="UnitOfWork">UnitOfWork</param>
        public SurveyService(IUnitOfWork unitOfWork, IAccountService accountService)
        {
            _UnitOfWork = unitOfWork;
            _AccountService = accountService;
        }
        #endregion

        #region Public Methods

        #region Survey
        /// <summary>
        /// Save User Survey
        /// </summary>
        /// <param name="request">Survey request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveUserSurvey(SurveyRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                SurveyResult objSurvey = new SurveyResult();
                objSurvey.UserID = request.UserID;
                objSurvey.SurveyType = request.SurveyType;
                objSurvey.SurveyName = CryptoUtil.EncryptInfo(request.SurveyName);
                objSurvey.StartTime = request.StartTime;
                objSurvey.EndTime = request.EndTime;
                objSurvey.Rating = CryptoUtil.EncryptInfo(request.Rating);
                objSurvey.Comment = CryptoUtil.EncryptInfo(request.Comment);
                objSurvey.Point = request.Point;
                objSurvey.CreatedOn = DateTime.UtcNow;
                objSurvey.Status = request.StatusType;
                if (request.IsDistraction != null)
                    objSurvey.IsDistraction = request.IsDistraction;
                if (request.IsNotificationGame != null)
                    objSurvey.IsNotificationGame = request.IsNotificationGame;
                if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                    objSurvey.AdminBatchSchID = request.AdminBatchSchID;
                if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                    objSurvey.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                if (request.SurveyID != null)
                    objSurvey.SurveyID = request.SurveyID;
                _UnitOfWork.ISurveyResultRepository.Add(objSurvey);
                _UnitOfWork.Commit();

                SurveyResultDtl objSurveyDtl = null;
                foreach (SurveyQueAndAns obj in request.QuestAndAnsList)
                {
                    objSurveyDtl = new SurveyResultDtl();
                    objSurveyDtl.SurveyResultID = objSurvey.SurveyResultID;
                    objSurveyDtl.Question = CryptoUtil.EncryptInfo(obj.Question);
                    objSurveyDtl.CorrectAnswer = CryptoUtil.EncryptInfo(obj.Answer);
                    objSurveyDtl.CreatedOn = DateTime.UtcNow;
                    objSurveyDtl.TimeTaken = obj.TimeTaken;
                    objSurveyDtl.ClickRange = obj.ClickRange;
                    _UnitOfWork.ISurveyResultDtlRepository.Add(objSurveyDtl);
                }
                _UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Get User Completed Survey
        /// </summary>
        /// <param name="request">UserId</param>
        /// <returns> Completed survey list of that user</returns>
        public CompletedSurveyResponse GetUserCompletedSurvey(CompletedSurveyRequest request)
        {
            var response = new CompletedSurveyResponse();
            try
            {
                List<CompletedSurvey> completedSurveyList = null;
                completedSurveyList = (from survey in _UnitOfWork.ISurveyResultRepository.RetrieveAll().Where(s => s.UserID == request.UserID)
                                       select new CompletedSurvey
                                       {
                                           SurveyResultID = survey.SurveyResultID,
                                           SurveyName = survey.SurveyName,
                                           EndTime = (DateTime)survey.EndTime
                                       }).ToList();

                if (completedSurveyList != null && completedSurveyList.Count() > 0)
                {
                    completedSurveyList = completedSurveyList.Select(survey => new CompletedSurvey()
                    {
                        SurveyResultID = survey.SurveyResultID,
                        SurveyName = CryptoUtil.DecryptInfo(survey.SurveyName),
                        EndTime = survey.EndTime
                    }).ToList();
                    response.CompletedSurveyList = completedSurveyList;
                }
                else
                    response.CompletedSurveyList = null;

                response.ErrorCode = LAMPConstants.API_SUCCESS_CODE;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new CompletedSurveyResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Get Survey Questions And Answers
        /// </summary>
        /// <param name="request"></param>
        /// <returns>survey Questions And Answers or the error message </returns>
        public SurveyResponse GetSurveyQueAndAns(SurveyQueAndAnsRequest request)
        {
            var response = new SurveyResponse();
            try
            {
                List<SurveyQueAndAns> surveyQueAndAnsList = null;
                surveyQueAndAnsList = (from survey in _UnitOfWork.ISurveyResultDtlRepository.RetrieveAll().Where(s => s.SurveyResultID == request.SurveyResultID)
                                       select new SurveyQueAndAns
                                       {
                                           Question = survey.Question,
                                           Answer = survey.CorrectAnswer
                                       }).ToList();
                if (surveyQueAndAnsList != null && surveyQueAndAnsList.Count > 0)
                {
                    surveyQueAndAnsList = surveyQueAndAnsList.Select(survey => new SurveyQueAndAns()
                    {
                        Question = CryptoUtil.DecryptInfo(survey.Question),
                        Answer = CryptoUtil.DecryptInfo(survey.Answer)
                    }).ToList();
                    response.SurveyQueAndAnsList = surveyQueAndAnsList;
                }
                else
                    response.SurveyQueAndAnsList = null;
                response.ErrorCode = LAMPConstants.API_SUCCESS_CODE;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new SurveyResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Gets the surveys.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public SurveyQuestionsResponse GetSurveys(SurveyQuestionsRequest request)
        {
            var response = new SurveyQuestionsResponse();
            try
            {
                long adminId = GetAdminId(request.UserID);
                List<SurveyWithQuestions> surveyList = new List<SurveyWithQuestions>();
                if (adminId != 0)
                {
                    if (request.LastUpdatedDate != null)
                    {
                        surveyList = (from surveys in _UnitOfWork.ISurveyRepository.RetrieveAll().Where(s => s.AdminID == adminId && (s.EditedOn > request.LastUpdatedDate))
                                      select new SurveyWithQuestions
                                      {
                                          SurveyID = surveys.SurveyID,
                                          SurveyName = surveys.SurveyName,
                                          LanguageCode = surveys.Language == null ? LAMPConstants.DEFAULT_LANGUAGE : surveys.Language,
                                          IsDeleted = surveys.IsDeleted,
                                          Questions = (from questions in surveys.SurveyQuestions.ToList()
                                                       select new SurveyQuestions
                                                           {
                                                               QuestionId = questions.QuestionID,
                                                               QuestionText = questions.QuestionText,
                                                               AnswerType = ((SurveyQuestionTypes)questions.AnswerType).ToString(),
                                                               IsDeleted = questions.IsDeleted
                                                           }).ToList()
                                      }).ToList();
                    }
                    else
                    {
                        surveyList = (from surveys in _UnitOfWork.ISurveyRepository.RetrieveAll().Where(s => s.AdminID == adminId)
                                      select new SurveyWithQuestions
                                      {
                                          SurveyID = surveys.SurveyID,
                                          SurveyName = surveys.SurveyName,
                                          LanguageCode = surveys.Language == null ? LAMPConstants.DEFAULT_LANGUAGE : surveys.Language,
                                          IsDeleted = surveys.IsDeleted,
                                          Questions = (from questions in surveys.SurveyQuestions.ToList()
                                                       select new SurveyQuestions
                                                           {
                                                               QuestionId = questions.QuestionID,
                                                               QuestionText = questions.QuestionText,
                                                               AnswerType = ((SurveyQuestionTypes)questions.AnswerType).ToString(),
                                                               IsDeleted = questions.IsDeleted
                                                           }).ToList()
                                      }).ToList();
                    }
                    if (surveyList.Count > 0)
                    {
                        foreach (SurveyWithQuestions survey in surveyList)
                        {
                            if (survey.Questions != null)
                            {
                                foreach (SurveyQuestions question in survey.Questions)
                                {
                                    if (question.AnswerType == SurveyQuestionTypes.ScrollWheels.ToString())
                                    {
                                        question.QuestionOptions = (from options in _UnitOfWork.ISurveyQuestionOptionRepository.RetrieveAll().Where(o => o.QuestionID == question.QuestionId)
                                                                    select new Options
                                                                    {

                                                                        OptionText = options.OptionText
                                                                    }).ToList();
                                    }
                                }
                            }
                        }
                    }
                    response.Survey = surveyList;
                    response.LastUpdatedDate = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new SurveyQuestionsResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        #endregion

        #region Location

        /// <summary>
        /// Save Location
        /// </summary>
        /// <param name="request">Location request</param>
        /// request tye:- 1:Location, 2:Environment
        /// <returns>Status</returns>
        public APIResponseBase SaveLocation(LocationRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    Location location = new Location();
                    location.UserID = request.UserID;
                    location.LocationName = CryptoUtil.EncryptInfo(request.LocationName.Trim());
                    location.Address = CryptoUtil.EncryptInfo(request.Address.Trim());
                    location.Type = request.Type;
                    if (request.Latitude != null)
                        location.Latitude = CryptoUtil.EncryptInfo(request.Latitude.Trim());
                    if (request.Longitude != null)
                        location.Longitude = CryptoUtil.EncryptInfo(request.Longitude.Trim());
                    location.CreatedOn = DateTime.UtcNow;
                    _UnitOfWork.ILocationRepository.Add(location);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }

        }

        #endregion

        #region HelpCall

        /// <summary>
        /// Save Help Call
        /// </summary>
        /// <param name="request">Help Call Request</param>
        /// request tye:- 1:Emergency, 2:Personal Helpline
        /// <returns>Status</returns>
        public APIResponseBase SaveHelpCall(HelpCallRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    HelpCall call = new HelpCall();
                    call.UserID = request.UserID;
                    call.CalledNumber = CryptoUtil.EncryptInfo(request.CalledNumber.Trim());
                    call.CallDateTime = request.CallDateTime;
                    call.CallDuraion = request.CallDuraion;
                    call.Type = request.Type;
                    call.CreatedOn = DateTime.UtcNow;
                    _UnitOfWork.IHelpCallRepository.Add(call);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }

        }

        #endregion

        #region Games

        /// <summary>
        /// Save Cat and dog Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveCatAndDogGame(CatAndDogGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_CatAndDogResult catAndDog = new CTest_CatAndDogResult();
                    catAndDog.UserID = request.UserID;
                    catAndDog.TotalQuestions = request.TotalQuestions;
                    catAndDog.CorrectAnswers = request.CorrectAnswers;
                    catAndDog.WrongAnswers = request.WrongAnswers;
                    catAndDog.StartTime = request.StartTime;
                    catAndDog.EndTime = request.EndTime;
                    catAndDog.Rating = 3;
                    catAndDog.Point = request.Point;
                    catAndDog.CreatedOn = DateTime.UtcNow;
                    catAndDog.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        catAndDog.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        catAndDog.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        catAndDog.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.ICatAndDogResultRepository.Add(catAndDog);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save Digit Span Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveDigitSpanGame(DigitSpanGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_DigitSpanResult digitSpan = new CTest_DigitSpanResult();
                    digitSpan.UserID = request.UserID;
                    digitSpan.Type = request.Type;
                    digitSpan.CorrectAnswers = request.CorrectAnswers;
                    digitSpan.WrongAnswers = request.WrongAnswers;
                    digitSpan.StartTime = request.StartTime;
                    digitSpan.EndTime = request.EndTime;
                    digitSpan.Rating = 3;
                    digitSpan.Point = request.Point;
                    digitSpan.CreatedOn = DateTime.UtcNow;
                    digitSpan.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    digitSpan.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        digitSpan.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        digitSpan.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        digitSpan.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.IDigitSpanResultRepository.Add(digitSpan);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save NBack Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveNBackGame(NBackGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_NBackResult nBack = new CTest_NBackResult();
                    nBack.UserID = request.UserID;
                    nBack.TotalQuestions = request.TotalQuestions;
                    nBack.CorrectAnswers = request.CorrectAnswers;
                    nBack.WrongAnswers = request.WrongAnswers;
                    nBack.StartTime = request.StartTime;
                    nBack.EndTime = request.EndTime;
                    nBack.Rating = 3;
                    nBack.Point = request.Point;
                    nBack.CreatedOn = DateTime.UtcNow;
                    nBack.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    nBack.Version = Convert.ToInt32(request.Version);
                    nBack.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        nBack.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        nBack.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        nBack.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.INBackResultRepository.Add(nBack);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save NBack New Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveNBackNewGame(NBackNewGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_NBackNewResult nBack = new CTest_NBackNewResult();
                    nBack.UserID = request.UserID;
                    nBack.TotalQuestions = request.TotalQuestions;
                    nBack.CorrectAnswers = request.CorrectAnswers;
                    nBack.WrongAnswers = request.WrongAnswers;
                    nBack.StartTime = request.StartTime;
                    nBack.EndTime = request.EndTime;
                    nBack.Rating = 3;
                    nBack.Point = request.Point;
                    nBack.CreatedOn = DateTime.UtcNow;
                    nBack.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    nBack.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        nBack.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        nBack.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        nBack.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.INBackNewResultRepository.Add(nBack);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save Serial7 Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveSerial7Game(Serial7GameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_Serial7Result serial7 = new CTest_Serial7Result();
                    serial7.UserID = request.UserID;
                    serial7.TotalQuestions = request.TotalQuestions;
                    serial7.TotalAttempts = request.TotalAttempts;
                    serial7.StartTime = request.StartTime;
                    serial7.EndTime = request.EndTime;
                    serial7.Rating = 3;
                    serial7.Point = request.Point;
                    serial7.CreatedOn = DateTime.UtcNow;
                    serial7.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    serial7.Version = Convert.ToInt32(request.Version);
                    serial7.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        serial7.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        serial7.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        serial7.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.ISerial7ResultRepository.Add(serial7);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save Simple Memory Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveSimpleMemoryGame(SimpleMemoryGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_SimpleMemoryResult simpleMemory = new CTest_SimpleMemoryResult();
                    simpleMemory.UserID = request.UserID;
                    simpleMemory.TotalQuestions = request.TotalQuestions;
                    simpleMemory.CorrectAnswers = request.CorrectAnswers;
                    simpleMemory.WrongAnswers = request.WrongAnswers;
                    simpleMemory.StartTime = request.StartTime;
                    simpleMemory.EndTime = request.EndTime;
                    simpleMemory.Rating = 3;
                    simpleMemory.Point = request.Point;
                    simpleMemory.CreatedOn = DateTime.UtcNow;
                    simpleMemory.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    simpleMemory.Version = Convert.ToInt32(request.Version);
                    simpleMemory.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        simpleMemory.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        simpleMemory.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        simpleMemory.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.ISimpleMemoryResultRepository.Add(simpleMemory);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save TrailsB Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveTrailsBGame(TrailsBGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_TrailsBResult trailsB = new CTest_TrailsBResult();
                    trailsB.UserID = request.UserID;
                    trailsB.TotalAttempts = request.TotalAttempts;
                    trailsB.StartTime = request.StartTime;
                    trailsB.EndTime = request.EndTime;
                    trailsB.Rating = 3;
                    trailsB.Point = request.Point;
                    trailsB.CreatedOn = DateTime.UtcNow;
                    trailsB.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    trailsB.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        trailsB.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        trailsB.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        trailsB.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.ITrailsBResultRepository.Add(trailsB);

                    CTest_TrailsBResultDtl trialsBDtl = new CTest_TrailsBResultDtl();
                    int sequence = 0;
                    if (request.RoutesList.Count > 0)
                    {
                        foreach (RoutesList routes in request.RoutesList)
                        {
                            sequence += 1;
                            foreach (Route route in routes.Routes)
                            {
                                trialsBDtl = new CTest_TrailsBResultDtl();
                                trialsBDtl.Alphabet = route.Alphabet;
                                trialsBDtl.TimeTaken = route.TimeTaken;
                                trialsBDtl.Status = route.Status;
                                trialsBDtl.CreatedOn = DateTime.UtcNow;
                                trialsBDtl.Sequence = sequence;
                                _UnitOfWork.ITrailsBResultDtlRepository.Add(trialsBDtl);
                            }
                        }
                    }

                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save Visual Association Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveVisualAssociationGame(VisualAssociationGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_VisualAssociationResult visualAssociation = new CTest_VisualAssociationResult();
                    visualAssociation.UserID = request.UserID;
                    visualAssociation.TotalQuestions = request.TotalQuestions;
                    visualAssociation.TotalAttempts = request.TotalAttempts;
                    visualAssociation.StartTime = request.StartTime;
                    visualAssociation.EndTime = request.EndTime;
                    visualAssociation.Rating = 3;
                    visualAssociation.Point = request.Point;
                    visualAssociation.CreatedOn = DateTime.UtcNow;
                    visualAssociation.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    visualAssociation.Version = Convert.ToInt32(request.Version);
                    visualAssociation.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        visualAssociation.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        visualAssociation.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        visualAssociation.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.IVisualAssociationResultRepository.Add(visualAssociation);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save 3DFigure Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase Save3DFigureGame(_3DFigureGameRequest request)
        {
            var response = new APIResponseBase();
            string fileName = string.Empty;
            try
            {
                if (request != null)
                {
                    if (request.DrawnFig != null && request.DrawnFig.Length > 0)
                    {
                        string imagePath = System.Configuration.ConfigurationManager.AppSettings["User3DFigurePath"];
                        CTest_3DFigureResult _3DFigure = new CTest_3DFigureResult();
                        _3DFigure.UserID = request.UserID;
                        if (request.GameName != null)
                            _3DFigure.GameName = request.GameName;
                        _3DFigure.C3DFigureID = request.C3DFigureID;
                        //Create Image and save in folder
                        fileName = Helper.UploadImage(request.DrawnFig, request.DrawnFigFileName, request.UserID.ToString(), imagePath);
                        _3DFigure.DrawnFigFileName = CryptoUtil.EncryptInfo(fileName);
                        _3DFigure.StartTime = request.StartTime;
                        _3DFigure.EndTime = request.EndTime;
                        _3DFigure.CreatedOn = DateTime.UtcNow;
                        if (request.Point != null)
                            _3DFigure.Point = request.Point;
                        if (request.IsNotificationGame != null)
                            _3DFigure.IsNotificationGame = request.IsNotificationGame;
                        if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                            _3DFigure.AdminBatchSchID = request.AdminBatchSchID;
                        if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                            _3DFigure.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                        _UnitOfWork.I3DFigureResultRepository.Add(_3DFigure);
                        _UnitOfWork.Commit();
                    }
                    else
                    {
                        response.ErrorCode = LAMPConstants.API_EMPTY_IMAGE_STRING;
                        response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_EMPTY_IMAGE_STRING);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save Spatial Span Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveSpatialSpanGame(SpatialSpanGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_SpatialResult spatialSpan = new CTest_SpatialResult();
                    spatialSpan.UserID = request.UserID;
                    spatialSpan.Type = request.Type;
                    spatialSpan.CorrectAnswers = request.CorrectAnswers;
                    spatialSpan.WrongAnswers = request.WrongAnswers;
                    spatialSpan.StartTime = request.StartTime;
                    spatialSpan.EndTime = request.EndTime;
                    spatialSpan.Rating = 3;
                    spatialSpan.Point = request.Point;
                    spatialSpan.CreatedOn = DateTime.UtcNow;
                    spatialSpan.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    spatialSpan.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        spatialSpan.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        spatialSpan.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        spatialSpan.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.ISpatialSpanResultRepository.Add(spatialSpan);

                    CTest_SpatialResultDtl spatialResultDtl = new CTest_SpatialResultDtl();
                    int sequence = 0;
                    if (request.BoxList.Count > 0)
                    {
                        foreach (BoxList boxes in request.BoxList)
                        {
                            sequence += 1;
                            foreach (Box box in boxes.Boxes)
                            {
                                spatialResultDtl = new CTest_SpatialResultDtl();
                                spatialResultDtl.GameIndex = box.GameIndex;
                                spatialResultDtl.TimeTaken = box.TimeTaken;
                                spatialResultDtl.Status = box.Status;
                                spatialResultDtl.CreatedOn = DateTime.UtcNow;
                                spatialResultDtl.Level = box.Level;
                                spatialResultDtl.Sequence = sequence;
                                _UnitOfWork.ISpatialResultDtlRepository.Add(spatialResultDtl);
                            }
                        }
                    }
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save Cat and Dog New Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveCatAndDogNewGame(CatAndDogNewGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_CatAndDogNewResult catAndDog = new CTest_CatAndDogNewResult();
                    catAndDog.UserID = request.UserID;
                    catAndDog.CorrectAnswers = request.CorrectAnswers;
                    catAndDog.WrongAnswers = request.WrongAnswers;
                    catAndDog.StartTime = request.StartTime;
                    catAndDog.EndTime = request.EndTime;
                    catAndDog.Rating = 3;
                    catAndDog.Point = request.Point;
                    catAndDog.CreatedOn = DateTime.UtcNow;
                    catAndDog.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    catAndDog.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        catAndDog.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        catAndDog.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        catAndDog.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.ICatAndDogNewResultRepository.Add(catAndDog);

                    if (request.GameLevelDetailList != null && request.GameLevelDetailList.Count > 0)
                    {
                        CTest_CatAndDogNewResultDtl catAndDogLevelDetail;
                        foreach (CatAndDogNewGameLevelDetailRequest levelDetail in request.GameLevelDetailList)
                        {

                            catAndDogLevelDetail = new CTest_CatAndDogNewResultDtl();
                            catAndDogLevelDetail.CatAndDogNewResultID = catAndDog.CatAndDogNewResultID;
                            catAndDogLevelDetail.CorrectAnswers = levelDetail.CorrectAnswer;
                            catAndDogLevelDetail.WrongAnswers = levelDetail.WrongAnswer;
                            catAndDogLevelDetail.TimeTaken = levelDetail.TimeTaken;
                            catAndDogLevelDetail.CreatedOn = DateTime.UtcNow;
                            _UnitOfWork.ICatAndDogNewResultDtlRepository.Add(catAndDogLevelDetail);
                        }
                    }
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save Temporal Order Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveTemporalOrderGame(TemporalOrderGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_TemporalOrderResult temporalOrder = new CTest_TemporalOrderResult();
                    temporalOrder.UserID = request.UserID;
                    temporalOrder.CorrectAnswers = request.CorrectAnswers;
                    temporalOrder.WrongAnswers = request.WrongAnswers;
                    temporalOrder.StartTime = request.StartTime;
                    temporalOrder.EndTime = request.EndTime;
                    temporalOrder.Rating = 3;
                    temporalOrder.Point = request.Point;
                    temporalOrder.CreatedOn = DateTime.UtcNow;
                    temporalOrder.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    temporalOrder.Version = Convert.ToInt32(request.Version);
                    temporalOrder.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        temporalOrder.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        temporalOrder.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        temporalOrder.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.ITemporalOrderResultRepository.Add(temporalOrder);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save TrailsB New Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveTrailsBNewGame(TrailsBNewGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_TrailsBNewResult trailsB = new CTest_TrailsBNewResult();
                    trailsB.UserID = request.UserID;
                    trailsB.TotalAttempts = request.TotalAttempts;
                    trailsB.StartTime = request.StartTime;
                    trailsB.EndTime = request.EndTime;
                    trailsB.Rating = 3;
                    trailsB.Point = request.Point;
                    trailsB.CreatedOn = DateTime.UtcNow;
                    trailsB.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    trailsB.Version = Convert.ToInt32(request.Version);
                    trailsB.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        trailsB.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        trailsB.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        trailsB.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.ITrailsBNewResultRepository.Add(trailsB);

                    CTest_TrailsBNewResultDtl trialsBDtl = new CTest_TrailsBNewResultDtl();
                    int sequence = 0;
                    if (request.RoutesList.Count > 0)
                    {
                        foreach (RoutesList routes in request.RoutesList)
                        {
                            sequence += 1;
                            foreach (Route route in routes.Routes)
                            {
                                trialsBDtl = new CTest_TrailsBNewResultDtl();
                                trialsBDtl.Alphabet = route.Alphabet;
                                trialsBDtl.TimeTaken = route.TimeTaken;
                                trialsBDtl.Status = route.Status;
                                trialsBDtl.CreatedOn = DateTime.UtcNow;
                                trialsBDtl.Sequence = sequence;
                                _UnitOfWork.ITrailsBNewResultDtlRepository.Add(trialsBDtl);
                            }
                        }
                    }
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                LogUtil.Error("SaveTrailsBNewGame:" + ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Saves the trails b dot touch game.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveTrailsBDotTouchGame(TrailsBDotTouchGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_TrailsBDotTouchResult trailsB = new CTest_TrailsBDotTouchResult();
                    trailsB.UserID = request.UserID;
                    trailsB.TotalAttempts = request.TotalAttempts;
                    trailsB.StartTime = request.StartTime;
                    trailsB.EndTime = request.EndTime;
                    trailsB.Rating = 3;
                    trailsB.Point = request.Point;
                    trailsB.CreatedOn = DateTime.UtcNow;
                    trailsB.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    trailsB.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        trailsB.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        trailsB.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        trailsB.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.ITrailsBDotTouchResultRepository.Add(trailsB);

                    CTest_TrailsBDotTouchResultDtl trialsBDtl = new CTest_TrailsBDotTouchResultDtl();
                    int sequence = 0;
                    if (request.RoutesList.Count > 0)
                    {
                        foreach (RoutesList routes in request.RoutesList)
                        {
                            sequence += 1;
                            foreach (Route route in routes.Routes)
                            {
                                trialsBDtl = new CTest_TrailsBDotTouchResultDtl();
                                trialsBDtl.Alphabet = route.Alphabet;
                                trialsBDtl.TimeTaken = route.TimeTaken;
                                trialsBDtl.Status = route.Status;
                                trialsBDtl.CreatedOn = DateTime.UtcNow;
                                trialsBDtl.Sequence = sequence;
                                _UnitOfWork.ITrailsBDotTouchResultDtlRepository.Add(trialsBDtl);
                            }
                        }
                    }

                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Saves the Jewels Trails A game.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveJewelsTrailsAGame(JewelsTrailsAGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_JewelsTrailsAResult trailsB = new CTest_JewelsTrailsAResult();
                    trailsB.UserID = request.UserID;
                    trailsB.TotalAttempts = request.TotalAttempts;
                    trailsB.StartTime = request.StartTime;
                    trailsB.EndTime = request.EndTime;
                    trailsB.Rating = 3;
                    trailsB.Point = request.Point;
                    trailsB.CreatedOn = DateTime.UtcNow;
                    trailsB.TotalBonusCollected = CryptoUtil.EncryptInfo(request.TotalBonusCollected.ToString());
                    trailsB.TotalJewelsCollected = CryptoUtil.EncryptInfo(request.TotalJewelsCollected.ToString());
                    trailsB.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    trailsB.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        trailsB.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        trailsB.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        trailsB.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.IJewelsTrailsAResultRepository.Add(trailsB);

                    CTest_JewelsTrailsAResultDtl trialsBDtl = new CTest_JewelsTrailsAResultDtl();
                    int sequence = 0;
                    if (request.RoutesList.Count > 0)
                    {
                        foreach (RoutesList routes in request.RoutesList)
                        {
                            sequence += 1;
                            foreach (Route route in routes.Routes)
                            {
                                trialsBDtl = new CTest_JewelsTrailsAResultDtl();
                                trialsBDtl.Alphabet = route.Alphabet;
                                trialsBDtl.TimeTaken = route.TimeTaken;
                                trialsBDtl.Status = route.Status;
                                trialsBDtl.CreatedOn = DateTime.UtcNow;
                                trialsBDtl.Sequence = sequence;
                                _UnitOfWork.IJewelsTrailsAResultDtlRepository.Add(trialsBDtl);
                            }
                        }
                    }

                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error("SaveJewelsTrailsAGame:" + ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Saves the Jewels TrailsB game.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveJewelsTrailsBGame(JewelsTrailsBGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_JewelsTrailsBResult trailsB = new CTest_JewelsTrailsBResult();
                    trailsB.UserID = request.UserID;
                    trailsB.TotalAttempts = request.TotalAttempts;
                    trailsB.StartTime = request.StartTime;
                    trailsB.EndTime = request.EndTime;
                    trailsB.Rating = 3;
                    trailsB.Point = request.Point;
                    trailsB.CreatedOn = DateTime.UtcNow;
                    trailsB.TotalBonusCollected = CryptoUtil.EncryptInfo(request.TotalBonusCollected.ToString());
                    trailsB.TotalJewelsCollected = CryptoUtil.EncryptInfo(request.TotalJewelsCollected.ToString());
                    trailsB.Score = CryptoUtil.EncryptInfo(request.Score.ToString());
                    trailsB.Status = request.StatusType;
                    if (request.IsNotificationGame != null)
                        trailsB.IsNotificationGame = request.IsNotificationGame;
                    if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                        trailsB.AdminBatchSchID = request.AdminBatchSchID;
                    if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                        trailsB.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                    _UnitOfWork.IJewelsTrailsBResultRepository.Add(trailsB);

                    CTest_JewelsTrailsBResultDtl trialsBDtl = new CTest_JewelsTrailsBResultDtl();
                    int sequence = 0;
                    if (request.RoutesList.Count > 0)
                    {
                        foreach (RoutesList routes in request.RoutesList)
                        {
                            sequence += 1;
                            foreach (Route route in routes.Routes)
                            {
                                trialsBDtl = new CTest_JewelsTrailsBResultDtl();
                                trialsBDtl.Alphabet = route.Alphabet;
                                trialsBDtl.TimeTaken = route.TimeTaken;
                                trialsBDtl.Status = route.Status;
                                trialsBDtl.CreatedOn = DateTime.UtcNow;
                                trialsBDtl.Sequence = sequence;
                                _UnitOfWork.IJewelsTrailsBResultDtlRepository.Add(trialsBDtl);
                            }
                        }
                    }

                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error("SaveJewelsTrailsBGame :" + ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save Scratch image game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveScratchImageGame(ScratchImageGameRequest request)
        {
            var response = new APIResponseBase();
            string fileName = string.Empty;
            try
            {
                if (request != null)
                {
                    if (request.DrawnImage != null && request.DrawnImage.Length > 0)
                    {
                        string imagePath = System.Configuration.ConfigurationManager.AppSettings["UserScratchImagePath"];
                        CTest_ScratchImageResult scratchImage = new CTest_ScratchImageResult();
                        scratchImage.UserID = request.UserID;
                        scratchImage.GameName = CryptoUtil.EncryptInfo(request.GameName);
                        scratchImage.ScratchImageID = request.ScratchImageID;
                        //Create Image and save in folder
                        fileName = Helper.UploadImage(request.DrawnImage, request.DrawnImageName, request.UserID.ToString(), imagePath);
                        scratchImage.DrawnFigFileName = CryptoUtil.EncryptInfo(fileName);
                        scratchImage.StartTime = request.StartTime;
                        scratchImage.EndTime = request.EndTime;
                        scratchImage.CreatedOn = DateTime.UtcNow;
                        scratchImage.Point = request.Point;
                        if (request.IsNotificationGame != null)
                            scratchImage.IsNotificationGame = request.IsNotificationGame;
                        if (request.AdminBatchSchID != null && request.AdminBatchSchID > 0)
                            scratchImage.AdminBatchSchID = request.AdminBatchSchID;
                        if (request.SpinWheelScore != null && request.SpinWheelScore.Trim().Length > 0)
                            scratchImage.SpinWheelScore = CryptoUtil.EncryptInfo(request.SpinWheelScore.Trim().ToString());
                        _UnitOfWork.IScratchImageResultRepository.Add(scratchImage);
                        _UnitOfWork.Commit();
                    }
                    else
                    {
                        response.ErrorCode = LAMPConstants.API_EMPTY_IMAGE_STRING;
                        response.ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_EMPTY_IMAGE_STRING);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }

        /// <summary>
        /// Save Spin Wheel Game
        /// </summary>
        /// <param name="request">Game request</param>
        /// <returns>Status</returns>
        public APIResponseBase SaveSpinWheelGame(SpinWheelGameRequest request)
        {
            var response = new APIResponseBase();
            try
            {
                if (request != null)
                {
                    CTest_SpinWheelResult spinWheel = new CTest_SpinWheelResult();
                    spinWheel.UserID = request.UserID;
                    spinWheel.StartTime = request.StartTime;
                    spinWheel.CollectedStars = CryptoUtil.EncryptInfo(request.CollectedStars);
                    spinWheel.DayStreak = request.DayStreak;
                    spinWheel.StrakSpin = request.StrakSpin;
                    spinWheel.GameDate = request.GameDate;
                    spinWheel.CreatedOn = DateTime.UtcNow;
                    _UnitOfWork.ISpinWheelResultRepository.Add(spinWheel);
                    _UnitOfWork.Commit();
                }
                return response;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return new APIResponseBase
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
        }
        #endregion

        #region Graph

        /// <summary>
        ///  Get the average and percentile score of a user for each game
        /// </summary>
        /// <param name="request">Graph request</param>
        /// <returns>Game scored</returns>
        public GraphResponse GetGameScoresforGraph(GraphRequest request)
        {
            var response = new GraphResponse();
            try
            {
                List<GameScore> gameScoreList = new List<GameScore>();

                //--NBack------------------
                GameScore score = GetNBackScore(request);
                gameScoreList.Add(score);

                //--NBack New------------------
                score = GetNBackNewScore(request);
                gameScoreList.Add(score);

                //--TrialsB----------------------
                score = GetTrialsBScore(request);
                gameScoreList.Add(score);

                //--TrialsB New----------------------
                score = GetTrialsBNewScore(request);
                gameScoreList.Add(score);

                //--TrialsB DotTouch----------------------
                score = GetTrialsBDotTouchScore(request);
                gameScoreList.Add(score);

                //--Jewels Trails A----------------------
                score = GetJewelsTrailsAGameScore(request);
                gameScoreList.Add(score);

                //--Jewels Trails B----------------------
                score = GetJewelsTrailsBGameScore(request);
                gameScoreList.Add(score);

                // --Spatial Span-----------------------
                score = GetSpatialSpanScore(request);
                gameScoreList.Add(score);

                //--Simple Memory----------------
                score = GetSimpleMemoryScore(request);
                gameScoreList.Add(score);

                // --Serail7 -------------------------
                score = GetSerial7Score(request);
                gameScoreList.Add(score);

                // -- Visual Association----------
                score = GetVisualAssociationScore(request);
                gameScoreList.Add(score);

                // --Digit Span-------------------
                score = GetDigitSpanScore(request);
                gameScoreList.Add(score);

                // --Cat and Dog New-------------------
                score = GetCatAndDogNewScore(request);
                gameScoreList.Add(score);

                // --Temporal Order-------------------
                score = GetTemporalOrderScore(request);
                gameScoreList.Add(score);

                response.GameScoreList = gameScoreList;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new GraphResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Get all Game total spin wheel score
        /// </summary>
        /// <param name="request">Game total score request</param>
        /// <returns>Game total score</returns>
        public GameTotalScoreResponse GetAllGameTotalSpinWheelScore(GameTotalScoreRequest request)
        {
            var response = new GameTotalScoreResponse();
            Single totalScore = 0;
            try
            {
                DateTime gameDate = Convert.ToDateTime(request.Date).Date;

                List<GameDetailsForGraph> gameDetailsList = new List<GameDetailsForGraph>();
                //survey 
                gameDetailsList = _UnitOfWork.ISurveyResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                   .Select(g => new GameDetailsForGraph()
                   {
                       StartTime = (DateTime)g.StartTime,
                       Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
                   }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                // 3DFigure
                gameDetailsList = _UnitOfWork.I3DFigureResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID).ToList()
                   .Select(g => new GameDetailsForGraph()
                   {
                       StartTime = (DateTime)g.StartTime,
                       Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
                   }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                // CatAndDogNew
                gameDetailsList = _UnitOfWork.ICatAndDogNewResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                // DigitSpan Forward & Backward
                gameDetailsList = _UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //Jewels Trails A
                gameDetailsList = _UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //Jewels Trails B
                gameDetailsList = _UnitOfWork.IJewelsTrailsBResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //NBack New
                gameDetailsList = _UnitOfWork.INBackNewResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //NBack
                gameDetailsList = _UnitOfWork.INBackResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //ScratchImage
                gameDetailsList = _UnitOfWork.IScratchImageResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //Serial7
                gameDetailsList = _UnitOfWork.ISerial7ResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                // SimpleMemory
                gameDetailsList = _UnitOfWork.ISimpleMemoryResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //Spatial Span Forward & Backward
                gameDetailsList = _UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //Temporal Order
                gameDetailsList = _UnitOfWork.ITemporalOrderResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //TrailsB Dot Touch
                gameDetailsList = _UnitOfWork.ITrailsBDotTouchResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //TrailsB New
                gameDetailsList = _UnitOfWork.ITrailsBNewResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //TrailsB
                gameDetailsList = _UnitOfWork.ITrailsBResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                //Visual Association = 9
                gameDetailsList = _UnitOfWork.IVisualAssociationResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.SpinWheelScore != null && g.SpinWheelScore.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.SpinWheelScore)) : 0
            }).ToList();
                totalScore = totalScore + gameDetailsList.Where(g => g.StartTime.Date == gameDate).Sum(g => g.Score);

                response.TotalScore = totalScore;

                // Get Collected stars for user
                gameDetailsList = _UnitOfWork.ISpinWheelResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID).ToList()
            .Select(g => new GameDetailsForGraph()
            {
                StartTime = (DateTime)g.StartTime,
                Score = (g.CollectedStars != null && g.CollectedStars.Length > 0) ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.CollectedStars)) : 0
            }).ToList();

                response.CollectedStars = gameDetailsList.Sum(g => g.Score);

                CTest_SpinWheelResult spinWheelDetials = _UnitOfWork.ISpinWheelResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID).OrderByDescending(o => o.StartTime).FirstOrDefault();
                if (spinWheelDetials != null)
                {
                    if (((DateTime)spinWheelDetials.GameDate).Date == gameDate)
                    {
                        response.GameDate = spinWheelDetials.GameDate;
                        response.DayStreak = spinWheelDetials.DayStreak;
                        response.StrakSpin = spinWheelDetials.StrakSpin;
                    }
                    else if (((DateTime)spinWheelDetials.GameDate).Date == gameDate.AddDays(-1))
                    {
                        response.GameDate = spinWheelDetials.GameDate;
                        response.DayStreak = spinWheelDetials.DayStreak;
                        response.StrakSpin = 0;
                    }
                    else
                    {
                        response.GameDate = null;
                        response.DayStreak = 0;
                        response.StrakSpin = 0;
                    }
                    
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new GameTotalScoreResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Get Game high and low score for Graph
        /// </summary>
        /// <param name="request">Game graph request</param>
        /// <returns>Game high and low score</returns>
        public GameGraphResponse GetGameHighAndLowScoreforGraph(GameGraphRequest request)
        {
            var response = new GameGraphResponse();
            try
            {
                List<GameDetailsForGraph> gameDetailsList = new List<GameDetailsForGraph>();
                switch (request.GameID)
                {
                    //NBack = 1
                    case (int)CognitionType.NBack:
                        gameDetailsList = _UnitOfWork.INBackResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;

                    //TrailsB = 2
                    case (int)CognitionType.TrailsB:
                        gameDetailsList = _UnitOfWork.ITrailsBResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //Spatial Span = 3,
                    case (int)CognitionType.SpatialSpan:
                        gameDetailsList = _UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Type == 1 && u.Status == 2).ToList()
                            .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //Spatial Span New = 4
                    case (int)CognitionType.SpatialSpanNew:
                        gameDetailsList = _UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Type == 2 && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //SimpleMemory = 5
                    case (int)CognitionType.SimpleMemory:
                        gameDetailsList = _UnitOfWork.ISimpleMemoryResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //Serial7 = 6
                    case (int)CognitionType.Serial7:
                        gameDetailsList = _UnitOfWork.ISerial7ResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //Visual Association = 9
                    case (int)CognitionType.VisualAssociation:
                        gameDetailsList = _UnitOfWork.IVisualAssociationResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;

                    //DigitSpan Forward = 10
                    case (int)CognitionType.DigitSpan:
                        gameDetailsList = _UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Type == 1 && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //Cat And Dog New = 11
                    case (int)CognitionType.CatAndDogNew:
                        gameDetailsList = _UnitOfWork.ICatAndDogNewResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //Temporal Order = 12
                    case (int)CognitionType.TemporalOrder:
                        gameDetailsList = _UnitOfWork.ITemporalOrderResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //DigitSpan Backward = 13
                    case (int)CognitionType.DigitSpanBackward:
                        gameDetailsList = _UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Type == 2 && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //NBack New = 14
                    case (int)CognitionType.NBackNew:
                        gameDetailsList = _UnitOfWork.INBackNewResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;

                    //TrailsB New = 15
                    case (int)CognitionType.TrailsBNew:
                        gameDetailsList = _UnitOfWork.ITrailsBNewResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //TrailsB Dot Touch = 16
                    case (int)CognitionType.TrailsBDotTouch:
                        gameDetailsList = _UnitOfWork.ITrailsBDotTouchResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //Jewels Trails A = 17
                    case (int)CognitionType.JewelsTrailsA:
                        gameDetailsList = _UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    //Jewels Trails B = 18
                    case (int)CognitionType.JewelsTrailsB:
                        gameDetailsList = _UnitOfWork.IJewelsTrailsBResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status == 2).ToList()
                    .Select(g => new GameDetailsForGraph()
                    {
                        UserID = request.UserID,
                        GameID = request.GameID,
                        StartTime = (DateTime)g.StartTime,
                        EndTime = (DateTime)g.EndTime,
                        Score = g.Score != string.Empty ? Convert.ToSingle(CryptoUtil.DecryptInfo(g.Score)) : 0
                    }).ToList();
                        break;
                    default:
                        gameDetailsList = null;
                        break;
                }

                Single[] dayTotalScore = new Single[7];
                if (gameDetailsList != null && gameDetailsList.Count > 0)
                {
                    response.HighScore = gameDetailsList.Max(c => c.Score);
                    if (gameDetailsList.Count == 1)
                        response.LowScore = -1;
                    else
                        response.LowScore = gameDetailsList.Min(c => c.Score);

                    DateTime dt = DateTime.UtcNow;
                    DateTime fromDt = dt.AddDays(-6);  // Previous 7 days upto today
                    for (int count = 0; count < 7; count++)
                    {
                        dayTotalScore[count] = gameDetailsList.Where(s => s.StartTime.Date == fromDt.Date).Sum(s => s.Score);
                        fromDt = fromDt.AddDays(1);
                    }
                }
                else
                {
                    response.HighScore = -1;
                    response.LowScore = -1;
                    dayTotalScore = new Single[7] { 0, 0, 0, 0, 0, 0, 0 };
                }
                response.DayTotalScore = dayTotalScore;
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                response = new GameGraphResponse
                {
                    ErrorCode = LAMPConstants.API_UNEXPECTED_ERROR,
                    ErrorMessage = ResourceHelper.GetStringResource(LAMPConstants.API_UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the average and percentile score of a user for NBack
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>NBack Score</returns>
        private GameScore GetNBackScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> NBackResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            NBackResultDetailList = _UnitOfWork.INBackResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                          .Select(c => new GeneralGameScores()
                          {
                              UserID = c.UserID,
                              Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                          });

            // -------User---Average---------------------
            decimal totalScore = NBackResultDetailList.Sum(x => x.Score);
            if (NBackResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / NBackResultDetailList.Count());
            }

            /*-------percentile calculation-------*/
            /* percentile = (average score fo the user / Highest average of the game) * 100*/
            NBackResultDetailList = _UnitOfWork.INBackResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                      .Select(c => new GeneralGameScores()
                      {
                          UserID = c.UserID,
                          Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                      });
            /*-------percentile calculation-------*/


            /*-------Total User average calculation-------*/
            if (NBackResultDetailList.Count() > 0)
            {
                totalAverage = (int)(NBackResultDetailList.Sum(x => x.Score) / NBackResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/

            GameScore nBack = new GameScore
            {
                Game = "n-back",
                average = average,
                totalAverage = totalAverage
            };
            return nBack;
        }

        /// <summary>
        /// Get the average and percentile score of a user for NBack New 
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>NBackNew Score</returns>
        private GameScore GetNBackNewScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> NBackResultDetailList = null;
            int average = 0;
            int totalAverage = 0;

            NBackResultDetailList = _UnitOfWork.INBackNewResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                          .Select(c => new GeneralGameScores()
                          {
                              UserID = c.UserID,
                              Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                          });

            // -------User---Average---------------------
            decimal totalScore = NBackResultDetailList.Sum(x => x.Score);
            if (NBackResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / NBackResultDetailList.Count());
            }

            /*-------percentile calculation-------*/
            NBackResultDetailList = _UnitOfWork.INBackNewResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                      .Select(c => new GeneralGameScores()
                      {
                          UserID = c.UserID,
                          Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                      });
            /*-------percentile calculation-------*/

            /*-------Total User average calculation-------*/
            if (NBackResultDetailList.Count() > 0)
            {
                totalAverage = (int)(NBackResultDetailList.Sum(x => x.Score) / NBackResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/

            GameScore nBack = new GameScore
            {
                Game = "n-back(New)",
                average = average,
                totalAverage = totalAverage
            };
            return nBack;
        }

        /// <summary>
        ///  Get the average and percentile score of a user for TrialsB
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>TrialsB Score</returns>
        private GameScore GetTrialsBScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> TrailsBResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            TrailsBResultDetailList = _UnitOfWork.ITrailsBResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                          .Select(c => new GeneralGameScores()
                          {
                              UserID = c.UserID,
                              Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                          });
            // -------User Average---------------------
            decimal totalScore = TrailsBResultDetailList.Sum(x => x.Score);
            if (TrailsBResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / TrailsBResultDetailList.Count());
            }

            //-------percentile calculation------------------------------------
            TrailsBResultDetailList = _UnitOfWork.ITrailsBResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                     .Select(c => new GeneralGameScores()
                     {
                         UserID = c.UserID,
                         Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                     });
            //-------percentile calculation------------------------------------

            /*-------Total User average calculation-------*/
            if (TrailsBResultDetailList.Count() > 0)
            {
                totalAverage = (int)(TrailsBResultDetailList.Sum(x => x.Score) / TrailsBResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/

            GameScore trialsB = new GameScore
            {
                Game = "trails-b",
                average = average,
                totalAverage = totalAverage
            };

            return trialsB;
        }

        /// <summary>
        /// Gets the trials b new score.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>TrialsBNew Score</returns>
        private GameScore GetTrialsBNewScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> TrailsBResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            TrailsBResultDetailList = _UnitOfWork.ITrailsBNewResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                          .Select(c => new GeneralGameScores()
                          {
                              UserID = c.UserID,
                              Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                          });
            // -------User Average---------------------
            decimal totalScore = TrailsBResultDetailList.Sum(x => x.Score);
            if (TrailsBResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / TrailsBResultDetailList.Count());
            }

            //-------percentile calculation------------------------------------
            TrailsBResultDetailList = _UnitOfWork.ITrailsBNewResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                     .Select(c => new GeneralGameScores()
                     {
                         UserID = c.UserID,
                         Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                     });

            //-------percentile calculation------------------------------------

            /*-------Total User average calculation-------*/
            if (TrailsBResultDetailList.Count() > 0)
            {
                totalAverage = (int)(TrailsBResultDetailList.Sum(x => x.Score) / TrailsBResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/

            GameScore trialsB = new GameScore
            {
                Game = "trails-b(New)",
                average = average,
                totalAverage = totalAverage
            };
            return trialsB;
        }

        /// <summary>
        /// Gets the trials b dot touch score.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>TrialsB Dot Touch Score</returns>
        private GameScore GetTrialsBDotTouchScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> TrailsBDotTouchResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            TrailsBDotTouchResultDetailList = _UnitOfWork.ITrailsBDotTouchResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                          .Select(c => new GeneralGameScores()
                          {
                              UserID = c.UserID,
                              Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                          });
            // -------User Average---------------------
            decimal totalScore = TrailsBDotTouchResultDetailList.Sum(x => x.Score);
            if (TrailsBDotTouchResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / TrailsBDotTouchResultDetailList.Count());
            }

            //-------percentile calculation------------------------------------
            TrailsBDotTouchResultDetailList = _UnitOfWork.ITrailsBDotTouchResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                     .Select(c => new GeneralGameScores()
                     {
                         UserID = c.UserID,
                         Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                     });

            //-------percentile calculation------------------------------------

            /*-------Total User average calculation-------*/
            if (TrailsBDotTouchResultDetailList.Count() > 0)
            {
                totalAverage = (int)(TrailsBDotTouchResultDetailList.Sum(x => x.Score) / TrailsBDotTouchResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/

            GameScore trialsB = new GameScore
            {
                Game = "trails-b(Dot Touch)",
                average = average,
                totalAverage = totalAverage
            };
            return trialsB;
        }

        /// <summary>
        /// Gets the Jewels Trails A score.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Jewels TrailsA Game Score</returns>
        private GameScore GetJewelsTrailsAGameScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> JewelsTrailsAResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            JewelsTrailsAResultDetailList = _UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                          .Select(c => new GeneralGameScores()
                          {
                              UserID = c.UserID,
                              Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                          });
            // -------User Average---------------------
            decimal totalScore = JewelsTrailsAResultDetailList.Sum(x => x.Score);
            if (JewelsTrailsAResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / JewelsTrailsAResultDetailList.Count());
            }

            //-------percentile calculation------------------------------------
            JewelsTrailsAResultDetailList = _UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                     .Select(c => new GeneralGameScores()
                     {
                         UserID = c.UserID,
                         Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                     });

            //-------percentile calculation------------------------------------
            /*-------Total User average calculation-------*/
            if (JewelsTrailsAResultDetailList.Count() > 0)
            {
                totalAverage = (int)(JewelsTrailsAResultDetailList.Sum(x => x.Score) / JewelsTrailsAResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/
            GameScore trialsB = new GameScore
            {
                Game = "Jewels Trails A",
                average = average,
                totalAverage = totalAverage
            };
            return trialsB;
        }

        /// <summary>
        /// Gets the Jewels Trails B score.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Jewels TrailsB Game Score</returns>
        private GameScore GetJewelsTrailsBGameScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> JewelsTrailsBResultDetailList = null;
            int average = 0;
            int totalAverage = 0;

            JewelsTrailsBResultDetailList = _UnitOfWork.IJewelsTrailsBResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                          .Select(c => new GeneralGameScores()
                          {
                              UserID = c.UserID,
                              Score = Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score))
                          });
            // -------User Average---------------------
            decimal totalScore = JewelsTrailsBResultDetailList.Sum(x => x.Score);
            if (JewelsTrailsBResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / JewelsTrailsBResultDetailList.Count());
            }

            //-------percentile calculation------------------------------------
            JewelsTrailsBResultDetailList = _UnitOfWork.IJewelsTrailsAResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                     .Select(c => new GeneralGameScores()
                     {
                         UserID = c.UserID,
                         Score = Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score))
                     });

            //-------percentile calculation------------------------------------

            /*-------Total User average calculation-------*/
            if (JewelsTrailsBResultDetailList.Count() > 0)
            {
                totalAverage = (int)(JewelsTrailsBResultDetailList.Sum(x => x.Score) / JewelsTrailsBResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/

            GameScore trialsB = new GameScore
            {
                Game = "Jewels Trails B",
                average = average,
                totalAverage = totalAverage
            };
            return trialsB;
        }

        /// <summary>
        ///  Get the average and percentile score of a user for Spatial Span
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Spatial Span Score</returns>
        private GameScore GetSpatialSpanScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> SpatialSpanResultDetailList = null;
            int average = 0;
            int totalAverage = 0;

            SpatialSpanResultDetailList = _UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                         .Select(c => new GeneralGameScores()
                         {
                             UserID = c.UserID,
                             Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                         });

            // -------average---------------------
            decimal totalScore = SpatialSpanResultDetailList.Sum(x => x.Score);
            if (SpatialSpanResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / SpatialSpanResultDetailList.Count());
            }

            //-------percentile calculation------------------------------------
            SpatialSpanResultDetailList = _UnitOfWork.ISpatialSpanResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                     .Select(c => new GeneralGameScores()
                     {
                         UserID = c.UserID,
                         Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                     });
            //-------percentile calculation------------------------------------

            /*-------Total User average calculation-------*/
            if (SpatialSpanResultDetailList.Count() > 0)
            {
                totalAverage = (int)(SpatialSpanResultDetailList.Sum(x => x.Score) / SpatialSpanResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/

            GameScore spatialSpan = new GameScore
            {
                Game = "Spatial Span",
                average = average,
                totalAverage = totalAverage
            };
            return spatialSpan;
        }

        /// <summary>
        /// Get the average and percentile score of a user for Spatial Span
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Simple Memory Score</returns>
        private GameScore GetSimpleMemoryScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> SimpleMemoryResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            SimpleMemoryResultDetailList = _UnitOfWork.ISimpleMemoryResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                          .Select(c => new GeneralGameScores()
                          {
                              UserID = c.UserID,
                              Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                          });
            // -------User average---------------------
            decimal totalScore = SimpleMemoryResultDetailList.Sum(x => x.Score);
            if (SimpleMemoryResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / SimpleMemoryResultDetailList.Count());
            }
            //-------percentile calculation------------------------------------
            SimpleMemoryResultDetailList = _UnitOfWork.ISimpleMemoryResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                      .Select(c => new GeneralGameScores()
                      {
                          UserID = c.UserID,
                          Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                      });
            //-------percentile calculation------------------------------------
            /*-------Total User average calculation-------*/
            if (SimpleMemoryResultDetailList.Count() > 0)
            {
                totalAverage = (int)(SimpleMemoryResultDetailList.Sum(x => x.Score) / SimpleMemoryResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/
            GameScore simpleMemory = new GameScore
            {
                Game = "Simple Memory",
                average = average,
                totalAverage = totalAverage
            };
            return simpleMemory;
        }

        /// <summary>
        /// Get the average and percentile score of a user for Visual Association
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Visual Association Score</returns>
        private GameScore GetVisualAssociationScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> VisualAssociationResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            VisualAssociationResultDetailList = _UnitOfWork.IVisualAssociationResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                          .Select(c => new GeneralGameScores()
                          {
                              UserID = c.UserID,
                              Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                          });
            // -------User average---------------------
            decimal totalScore = VisualAssociationResultDetailList.Sum(x => x.Score);
            if (VisualAssociationResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / VisualAssociationResultDetailList.Count());
            }
            //-------percentile calculation------------------------------------
            VisualAssociationResultDetailList = _UnitOfWork.IVisualAssociationResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                     .Select(c => new GeneralGameScores()
                     {
                         UserID = c.UserID,
                         Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                     });
            //-------percentile calculation------------------------------------
            /*-------Total User average calculation-------*/
            if (VisualAssociationResultDetailList.Count() > 0)
            {
                totalAverage = (int)(VisualAssociationResultDetailList.Sum(x => x.Score) / VisualAssociationResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/
            GameScore visualAssociation = new GameScore
            {
                Game = "Visual Association",
                average = average,
                totalAverage = totalAverage
            };
            return visualAssociation;
        }

        /// <summary>
        /// Get the average and percentile score of a user for Temporal Order
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Temporal Order Score</returns>
        private GameScore GetTemporalOrderScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> TemporalOrderResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            TemporalOrderResultDetailList = _UnitOfWork.ITemporalOrderResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                         .Select(c => new GeneralGameScores()
                         {
                             UserID = c.UserID,
                             Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                         });

            // -------User average---------------------
            decimal totalScore = TemporalOrderResultDetailList.Sum(x => x.Score);
            if (TemporalOrderResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / TemporalOrderResultDetailList.Count());
            }
            //-------percentile calculation------------------------------------
            TemporalOrderResultDetailList = _UnitOfWork.ITemporalOrderResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                   .Select(c => new GeneralGameScores()
                   {
                       UserID = c.UserID,
                       Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                   });

            /*-------Total User average calculation-------*/
            if (TemporalOrderResultDetailList.Count() > 0)
            {
                totalAverage = (int)(TemporalOrderResultDetailList.Sum(x => x.Score) / TemporalOrderResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/
            GameScore temporalOrder = new GameScore
            {
                Game = "Temporal Order",
                average = average,
                totalAverage = totalAverage
            };
            return temporalOrder;
        }

        /// <summary>
        /// Get the average and percentile score of a user for Digit Span
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Digit Span Score</returns>
        private GameScore GetDigitSpanScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> DigitSpanResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            DigitSpanResultDetailList = _UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                         .Select(c => new GeneralGameScores()
                         {
                             UserID = c.UserID,
                             Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                         });
            // -------User average---------------------
            decimal totalScore = DigitSpanResultDetailList.Sum(x => x.Score);
            if (DigitSpanResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / DigitSpanResultDetailList.Count());
            }
            //-------percentile calculation------------------------------------  
            DigitSpanResultDetailList = _UnitOfWork.IDigitSpanResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                    .Select(c => new GeneralGameScores()
                    {
                        UserID = c.UserID,
                        Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                    });


            /*-------Total User average calculation-------*/
            if (DigitSpanResultDetailList.Count() > 0)
            {
                totalAverage = (int)(DigitSpanResultDetailList.Sum(x => x.Score) / DigitSpanResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/
            GameScore digitSpan = new GameScore
            {
                Game = "Digit Span",
                average = average,
                totalAverage = totalAverage
            };
            return digitSpan;
        }

        /// <summary>
        /// Get the average and percentile score of a user for Serial7
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Serial7 Score</returns>
        private GameScore GetSerial7Score(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> Serial7ResultDetailList = null;
            int average = 0;
            int totalAverage = 0;
            Serial7ResultDetailList = _UnitOfWork.ISerial7ResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                        .Select(c => new GeneralGameScores()
                        {
                            UserID = c.UserID,
                            Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                        });
            // -------User average---------------------
            decimal totalScore = Serial7ResultDetailList.Sum(x => x.Score);
            if (Serial7ResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / Serial7ResultDetailList.Count());
            }
            //-------percentile calculation------------------------------------
            Serial7ResultDetailList = _UnitOfWork.ISerial7ResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                    .Select(c => new GeneralGameScores()
                    {
                        UserID = c.UserID,
                        Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                    });

            /*-------Total User average calculation-------*/
            if (Serial7ResultDetailList.Count() > 0)
            {
                totalAverage = (int)(Serial7ResultDetailList.Sum(x => x.Score) / Serial7ResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/
            GameScore serial7 = new GameScore
            {
                Game = "Serial 7s",
                average = average,
                totalAverage = totalAverage
            };
            return serial7;
        }

        /// <summary>
        /// Get the average and percentile score of a user for CatandDogNew
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Cat and Dog New Score</returns>
        private GameScore GetCatAndDogNewScore(GraphRequest request)
        {
            IEnumerable<GeneralGameScores> CatnadDogResultDetailList = null;
            int average = 0;
            int totalAverage = 0;

            CatnadDogResultDetailList = _UnitOfWork.ICatAndDogNewResultRepository.RetrieveAll().Where(u => u.UserID == request.UserID && u.Status != 1).ToList() // now we have in-memory query
                        .Select(c => new GeneralGameScores()
                        {
                            UserID = c.UserID,
                            Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                        });
            // -------User average---------------------
            decimal totalScore = CatnadDogResultDetailList.Sum(x => x.Score);
            if (CatnadDogResultDetailList.Count() > 0)
            {
                average = (int)(totalScore / CatnadDogResultDetailList.Count());
            }
            //-------percentile calculation------------------------------------
            CatnadDogResultDetailList = _UnitOfWork.ICatAndDogNewResultRepository.RetrieveAll().Where(u => u.Status != 1).ToList() // now we have in-memory query
                    .Select(c => new GeneralGameScores()
                    {
                        UserID = c.UserID,
                        Score = c.Score != string.Empty ? Convert.ToDecimal(CryptoUtil.DecryptInfo(c.Score)) : 0
                    });
            /*-------Total User average calculation-------*/
            if (CatnadDogResultDetailList.Count() > 0)
            {
                totalAverage = (int)(CatnadDogResultDetailList.Sum(x => x.Score) / CatnadDogResultDetailList.Count());
            }
            /*-------Total User average calculation-------*/
            GameScore catandDog = new GameScore
            {
                Game = "Cat and Dogs(New)",
                average = average,
                totalAverage = totalAverage
            };
            return catandDog;
        }

        /// <summary>
        /// To get the AdminId by the given UserId.
        /// </summary>
        /// <param name="UserId">UserId</param>
        /// <returns>AdminId</returns>
        private long GetAdminId(long UserId)
        {
            long adminId = 0;
            User user = _UnitOfWork.IUserRepository.SingleOrDefault(u => u.UserID == UserId);
            if (user.AdminID != null)
                adminId = Convert.ToInt64(user.AdminID);
            return adminId;
        }
        #endregion
    }
}


