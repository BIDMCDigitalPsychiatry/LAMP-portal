using LAMP.Service;
using LAMP.Utility;
using LAMP.ViewModel;
using PagedList;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LAMP.Web.Controllers
{
    public class SurveyController : BaseController
    {
        #region PrivateVariables
        private IUserAdminService _userService;
        #endregion

        #region Constructor
        public SurveyController(IUserAdminService userService)
        {
            _userService = userService;
        }
        #endregion

        #region PublicMethods
        /// <summary>
        ///  To get the survey list.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(SurveyListViewModel model)
        {
            Session["CurrentPage"] = "SurveyIndex";
            model.UserId = loggedInUserId;

            if (model != null && model.UserId > 0)
            {
                string[] pageConditions = { };
                if (HttpContext.Session["SurveyPagingCriteria"] != null)
                {
                    pageConditions = HttpContext.Session["SurveyPagingCriteria"].ToString().Split('|');
                    model.SortPageOptions.SortField = pageConditions[(int)PageConditions.SortColumn].ToString();
                    model.SortPageOptions.SortOrder = pageConditions[(int)PageConditions.SortOrder].ToString();
                    model.SortPageOptions.PageSize = Convert.ToInt16(pageConditions[(int)PageConditions.PageSize]);
                    model.SortPageOptions.CurrentPage = Convert.ToInt16(pageConditions[(int)PageConditions.CurrentPage]);
                }
                else
                {
                    model.SortPageOptions.CurrentPage = 1;
                    model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                    model.SortPageOptions.SortField = "SurveyName";
                    model.SortPageOptions.SortOrder = "asc";
                }
            }
            SurveyListViewModel response = new SurveyListViewModel();
            response = _userService.GetSurveys(model);
            GetResponse(model, ref response);

            ModelState.Clear();
            return View(response);
        }

        /// <summary>
        /// To search surveys.
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="command">command</param>
        /// <param name="searchString">searchString</param>
        /// <param name="sortColumn">sortColumn</param>
        /// <param name="sortOrder">sortOrder</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="page">page</param>
        /// <returns>Surveys</returns>
        [HttpGet]
        public ActionResult SearchSurveys(SurveyListViewModel model, string command, string searchString, string sortColumn, string sortOrder, int? pageSize, int? page)
        {
            var response = new SurveyListViewModel();
            model.UserId = loggedInUserId;

            if (model.SortPageOptions != null && model.SortPageOptions.PageSize == 0 && pageSize > 0)
            {
                model.SortPageOptions.CurrentPage = (short)page;
                model.SortPageOptions.PageSize = (short)pageSize;
                model.SortPageOptions.SortField = sortColumn;
                model.SortPageOptions.SortOrder = sortOrder;
            }
            else if (model.SortPageOptions != null && model.SortPageOptions.PageSize > 0)
            {
                model.SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage;
                model.SortPageOptions.PageSize = model.SortPageOptions.PageSize;
                model.SortPageOptions.SortField = model.SortPageOptions.SortField;
                model.SortPageOptions.SortOrder = model.SortPageOptions.SortOrder;
            }

            if (command == "Search")
            {
                model.SortPageOptions.CurrentPage = 1;
                model.SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
                model.SortPageOptions.SortField = "SurveyName";
                model.SortPageOptions.SortOrder = "asc";
            }

            if (searchString != null && searchString.Trim().Length > 0)
                model.SearchText = searchString;

            response = _userService.GetSurveys(model);
            GetResponse(model, ref response);
            response.SearchText = searchString;
            ModelState.Clear();
            return PartialView("_SurveyListPartial", response);
        }

        /// <summary>
        /// To add a new survey.
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddSurvey(SurveyViewModel model)
        {
            model.AdminID = loggedInUserId;
            var questionTypes = (from SurveyQuestionTypes type in Enum.GetValues(typeof(SurveyQuestionTypes))
                                 select new
                                      {
                                          ID = (int)type,
                                          Name = type.ToString()
                                      });
            ViewBag.QuestionTypes = new SelectList(questionTypes, "ID", "Name");
            model.Status = 0;
            model.Message = "";
            model.AnswerType = null;
            ModelState.Clear();
            return View(model);
        }

        /// <summary>
        /// To save a survey
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveSurvey(SurveyViewModel model)
        {
            model.AdminID = loggedInUserId;
            model = _userService.SaveSurvey(model);
            if (model.Status == LAMPConstants.SUCCESS_CODE)
            {
                model.IsSaved = true;
            }

            if (model.SurveyID > 0)
            {
                model.IsEdit = true;
            }
            return View("AddSurvey", model);
        }

        /// <summary>
        /// To edit a survey.
        /// </summary>
        /// <param name="SurveyId">SurveyId</param>
        /// <returns></returns>
        public ActionResult EditSurvey(long SurveyId)
        {
            SurveyViewModel model = new SurveyViewModel();
            model = _userService.GetSurveyBySurveyId(SurveyId);
            model.IsEdit = true;
            return View("AddSurvey", model);
        }

        /// <summary>
        /// To delete a survey.
        /// </summary>
        /// <param name="SurveyId">SurveyId</param>
        /// <returns></returns>
        public ActionResult DeleteSurvey(long SurveyId)
        {
            SurveyListViewModel response = new SurveyListViewModel();
            response = _userService.DeleteSurvey(SurveyId, loggedInUserId);
            return PartialView("_SurveyListPartial", response);
        }

        /// <summary>
        /// To delete a survey question
        /// </summary>
        /// <param name="QuestionId">QuestionId</param>
        /// <returns></returns>
        public JsonResult DeleteSurveyQuestion(long QuestionId)
        {
            SurveyViewModel model = new SurveyViewModel();
            model = _userService.DeleteSurveyQuestion(QuestionId);
            model.IsEdit = true;
            return Json(model);
        }

        /// <summary>
        /// Edit Survey Question
        /// </summary>
        /// <param name="QuestionId">QuestionId</param>
        /// <returns></returns>
        public JsonResult EditSurveyQuestion(long QuestionId)
        {
            SurveyViewModel model = new SurveyViewModel();
            model = _userService.EditSurveyQuestion(QuestionId);
            return Json(model);
        }
        #endregion

        #region PrivateMethods
        /// <summary>
        /// To set paging  and sort page options to the list.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="response"></param>
        private void GetResponse(SurveyListViewModel model, ref SurveyListViewModel response)
        {
            response.SearchText = ((model.SearchText == null || model.SearchText.Length == 0) ? "" : model.SearchText);
            var _SortPageOptions = new SortPageOptions();
            if (model.SortPageOptions == null)
            {
                _SortPageOptions.SortField = "SurveyName";
                _SortPageOptions.SortOrder = "asc";
                _SortPageOptions.CurrentPage = 1;
                _SortPageOptions.PageSize = LAMPConstants.LAMP_PAGE_SIZE;
            }
            else
            {
                _SortPageOptions.SortField = ((model.SortPageOptions.SortField == null || model.SortPageOptions.SortField.Length == 0) ? "SurveyName" : model.SortPageOptions.SortField);
                _SortPageOptions.SortOrder = (model.SortPageOptions.SortOrder == null || model.SortPageOptions.SortOrder.Length == 0) ? "asc" : model.SortPageOptions.SortOrder;    // sortOrder;
                _SortPageOptions.CurrentPage = model.SortPageOptions.CurrentPage == 0 ? Convert.ToInt16(1) : model.SortPageOptions.CurrentPage;   // page;
                _SortPageOptions.PageSize = model.SortPageOptions.PageSize == 0 ? Convert.ToInt16(10) : model.SortPageOptions.PageSize;
            }
            response.SortPageOptions = _SortPageOptions;

            var providerUsers = response.SurveyList.ToPagedList<SurveyForList>((int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize);
            response.PagedSurveyList = new StaticPagedList<SurveyForList>(providerUsers, (int)response.SortPageOptions.CurrentPage, (int)response.SortPageOptions.PageSize, response.SurveyList.Count);
            response.SurveyList = response.SurveyList.Skip((_SortPageOptions.CurrentPage - 1) * _SortPageOptions.PageSize).ToList();
            response.SurveyList = response.SurveyList.Take(_SortPageOptions.PageSize).ToList();
            response.SurveyList.Select(survey =>
            {
                survey.CreatedOnString = Helper.GetDateString(survey.CreatedOn, "MM/dd/yyyy");

                return survey;
            }).ToList();

            // Session variable is used to handle paging while back to users page like edit use, view user.
            HttpContext.Session["SurveyPagingCriteria"] = response.SortPageOptions.SortField + "|" + response.SortPageOptions.SortOrder + "|" + response.SortPageOptions.PageSize.ToString() + "|" + response.SortPageOptions.CurrentPage.ToString();
        }

        #endregion
    }
}