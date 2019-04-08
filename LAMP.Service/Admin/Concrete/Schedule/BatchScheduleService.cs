
using LAMP.DataAccess;
using LAMP.DataAccess.Entities;
using LAMP.Utility;
using LAMP.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace LAMP.Service
{
    public class BatchScheduleService : IScheduleService
    {
        #region Variables
        private IUnitOfWork _UnitOfWork;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to initialize the member variables
        /// </summary>
        /// <param name="UnitOfWork"></param>
        public BatchScheduleService(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get Scheduled List for batch 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ScheduleListViewModel GetBatchScheduledList(ScheduleListViewModel model)
        {
            var response = new ScheduleListViewModel();
            try
            {
                IEnumerable<AdminScheduleViewModel> enumBatchScheduleList = (from scheduledBatch in _UnitOfWork.IAdminBatchScheduleRepository.RetrieveAll()
                                                                             join slots in _UnitOfWork.ISlotRepository.RetrieveAll() on scheduledBatch.SlotID equals slots.SlotID into ps
                                                                             from p in ps.DefaultIfEmpty()
                                                                             join repeats in _UnitOfWork.IRepeatRepository.RetrieveAll() on scheduledBatch.RepeatID equals repeats.RepeatID into rs
                                                                             from r in rs.DefaultIfEmpty()
                                                                             where scheduledBatch.IsDeleted != true && scheduledBatch.AdminID == model.LoggedInAdminId
                                                                             select new AdminScheduleViewModel
                                                                             {
                                                                                 AdminSchID = scheduledBatch.AdminBatchSchID,
                                                                                 AdminID = scheduledBatch.AdminID,
                                                                                 CreatedOn = scheduledBatch.CreatedOn,
                                                                                 RepeatID = scheduledBatch.RepeatID,
                                                                                 RepeatInterval = r.RepeatInterval,
                                                                                 ScheduleDate = scheduledBatch.ScheduleDate,
                                                                                 SlotID = scheduledBatch.SlotID,
                                                                                 SlotName = p.SlotName,
                                                                                 BatchName = scheduledBatch.BatchName,
                                                                                 Time = scheduledBatch.Time
                                                                             }).ToList();
                foreach (var item in enumBatchScheduleList)
                {
                    if (item.Time != null)
                    {
                        DateTime surveyDisplayTime = (DateTime)(item.Time);
                        var Timestamp = surveyDisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                        item.SlotTimeStamp = Timestamp.ToString();
                    }
                    item.SlotTimeOptions = GetSlotTimeOptionsForBatch(item.AdminSchID);
                }

                List<AdminScheduleViewModel> gameList = new List<AdminScheduleViewModel>();
                if (model.SearchId != null)
                    gameList = enumBatchScheduleList.ToList().FindAll(w => CryptoUtil.DecryptInfo(w.Name).StartsWith(model.SearchId));

                else
                    gameList = enumBatchScheduleList.ToList();


                string sortField = String.IsNullOrEmpty(model.SortPageOptions.SortField) ? "Name" : model.SortPageOptions.SortField;
                string sortDirection = String.IsNullOrEmpty(model.SortPageOptions.SortOrder) ? "asc" : model.SortPageOptions.SortOrder;
                switch (sortField)
                {
                    case "BatchName":
                        if (sortDirection == "asc")
                            gameList = gameList.OrderBy(c => CryptoUtil.DecryptInfo(c.Name)).ToList();
                        else
                            gameList = gameList.OrderByDescending(c => CryptoUtil.DecryptInfo(c.Name)).ToList();
                        break;
                    case "CreatedOn":
                        if (sortDirection == "asc")
                            gameList = gameList.OrderBy(c => c.CreatedOn).ToList();
                        else
                            gameList = gameList.OrderByDescending(c => c.CreatedOn).ToList();
                        break;

                    default:
                        gameList = gameList.OrderBy(c => CryptoUtil.DecryptInfo(c.Name)).ToList();
                        break;
                }

                response.AdminScheduleViewModelList = gameList;
                response.TotalRows = gameList.Count;
            }
            catch (Exception ex)
            {
                LogUtil.Error("BatchScheduleService/GetScheduledList: " + ex);
                response = new ScheduleListViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return response;
        }

        /// <summary>
        /// Save Shedule Game
        /// </summary>
        /// <param name="scheduleViewModel"></param>
        /// <returns></returns>
        public ScheduleGameSurveyViewModel SaveBatchShedule(ScheduleBatchViewModel scheduleViewModel)
        {
            var scheduleViewModelResponse = new ScheduleGameSurveyViewModel();
            try
            {
                CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                scheduleViewModelResponse.LoggedInUserId = scheduleViewModel.LoggedInUserId;
                scheduleViewModelResponse.AdminId = scheduleViewModel.AdminId;
                scheduleViewModelResponse.AdminCTestSchID = scheduleViewModel.AdminCTestSchID;
                LAMPEntities context = new LAMPEntities();
                scheduleViewModelResponse.GameScheduleDateString = scheduleViewModel.GameScheduleDateString;
                scheduleViewModelResponse.CognitionTestSlotTimeString = scheduleViewModel.CognitionTestSlotTimeString;
                scheduleViewModelResponse.CognitionTestSlotId = scheduleViewModel.CognitionTestSlotId;
                var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                scheduleViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                scheduleViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                scheduleViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();

                var fisrtCtestselected = scheduleViewModelResponse.CognitionTestList.ElementAt(0).Value;
                long repeatId = (long)scheduleViewModel.SurveyRepeatId;
                Admin_SaveCTestSchedule_sp_Result result = new Admin_SaveCTestSchedule_sp_Result();
                var command = context.Database.Connection.CreateCommand();
                command.CommandText = "Admin_SaveBatchSchedule_sp";
                command.CommandType = CommandType.StoredProcedure;
                DateTime dt1;
                DateTime dt2;
                command.Parameters.Add(new SqlParameter("@p_AdminBatchSchID", scheduleViewModel.AdminBatchSchID));
                command.Parameters.Add(new SqlParameter("@p_AdminID", scheduleViewModel.AdminId));
                command.Parameters.Add(new SqlParameter("@p_BatchName", scheduleViewModel.BatchName));
                if (repeatId == 11 || repeatId == 2 || repeatId == 3 || repeatId == 4)
                {
                    command.Parameters.Add(new SqlParameter("@p_ScheduleDate", null));
                }
                else
                {
                    dt1 = DateTime.ParseExact(scheduleViewModel.GameScheduleDateString, @"dd/MM/yyyy", CultureInfo.InvariantCulture);
                    command.Parameters.Add(new SqlParameter("@p_ScheduleDate", dt1));
                }
                if (repeatId == 11 || repeatId == 2 || repeatId == 3 || repeatId == 4)
                {
                    command.Parameters.Add(new SqlParameter("@p_Time", null));
                }
                else
                {
                    dt2 = Convert.ToDateTime(scheduleViewModel.BatchSlotTimeString).ToUniversalTime();
                    command.Parameters.Add(new SqlParameter("@p_Time", dt2));
                    LogUtil.Error("SaveBatchShedule for @p_Time: BatchSlotTimeString" + scheduleViewModel.BatchSlotTimeString + " dt2: " + dt2.ToString() + " for batch name: " + scheduleViewModel.BatchName);
                }
                command.Parameters.Add(new SqlParameter("@p_SlotID", null));
                command.Parameters.Add(new SqlParameter("@p_RepeatID", scheduleViewModel.SurveyRepeatId));
                XElement xmlCognitionElements = null;
                xmlCognitionElements = new XElement("CTests",
                    from cognition in scheduleViewModel.CognitionTest
                    select new XElement("CTest",
                        new XElement("CTestID", cognition.CognitionTestId),
                        new XElement("Version", cognition.CognitionVersionId),
                        new XElement("Order", cognition.Order)
                        ));
                command.Parameters.Add(new SqlParameter("@p_CTestXML", xmlCognitionElements.ToString()));

                XElement xmlSurveyElements = null;
                xmlSurveyElements = new XElement("Surveys",
                    from survey in scheduleViewModel.BatchSurvey
                    select new XElement("Survey",
                        new XElement("SurveyID", survey.SurveyId),
                        new XElement("Order", survey.Order)
                        ));
                command.Parameters.Add(new SqlParameter("@p_SurveyXML", xmlSurveyElements.ToString()));

                if (repeatId == 11)
                {
                    XElement xmlOptionElements = null;
                    List<string> optionList = new List<string>();
                    foreach (string item in scheduleViewModel.OptionsStringList.ToList())
                    {
                        DateTime dto = Convert.ToDateTime(item).ToUniversalTime();
                        string item2 = dto.ToString("yyyy-MM-dd HH:mm");
                        optionList.Add(item2);
                    }
                    xmlOptionElements = new XElement("CustomTimes", optionList.Select(i => new XElement("CustomTime", i)));
                    command.Parameters.Add(new SqlParameter("@p_CustomTimeXML", xmlOptionElements.ToString()));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("@p_CustomTimeXML", null));
                }

                var outputErrorParameter = command.CreateParameter();
                outputErrorParameter.ParameterName = "@p_ErrID";
                outputErrorParameter.DbType = DbType.Int64;
                outputErrorParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(outputErrorParameter);
                context.Database.Connection.Open();
                long ErrorCode = 0;
                int val = command.ExecuteNonQuery();
                if (outputErrorParameter.Value != null)
                {
                    ErrorCode = Convert.ToInt32(outputErrorParameter.Value);
                }
                if (ErrorCode == 0)
                {
                    scheduleViewModelResponse.Status = LAMPConstants.SUCCESS_CODE;
                    scheduleViewModelResponse.Message = "The Game schedule have been saved successfully.";
                    scheduleViewModelResponse.IsSaved = true;
                }
                else if (ErrorCode == 1003)
                {
                    scheduleViewModelResponse.Status = LAMPConstants.ERROR_CODE;
                    scheduleViewModelResponse.Message = "Batch name Already Exists.";
                    scheduleViewModelResponse.IsSaved = false;
                }
                else
                {
                    scheduleViewModelResponse.Status = LAMPConstants.UNEXPECTED_ERROR;
                    scheduleViewModelResponse.Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR);
                    scheduleViewModelResponse.IsSaved = false;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("BatchScheduleService/SaveSheduleGame: " + ex);
                scheduleViewModelResponse = new ScheduleGameSurveyViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleViewModelResponse;
        }

        /// <summary>
        /// Get batch ScheduleViewModel Details For Admin
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel"></param>
        /// <returns></returns>
        public ScheduleBatchViewModel GetScheduleViewModelDetailsForAdmin(ScheduleBatchViewModel ScheduleBatchSurveyViewModel)
        {
            var scheduleBatchSurveyViewModelResponse = new ScheduleBatchViewModel();
            try
            {
                scheduleBatchSurveyViewModelResponse.LoggedInUserId = ScheduleBatchSurveyViewModel.LoggedInUserId;
                scheduleBatchSurveyViewModelResponse.AdminId = ScheduleBatchSurveyViewModel.AdminId;
                //filling dropdownlist
                var slotList = _UnitOfWork.ISlotRepository.RetrieveAll().ToList();
                scheduleBatchSurveyViewModelResponse.SlotList = slotList.Select(x => new SelectListItem { Text = x.SlotName, Value = x.SlotID.ToString() }).ToList();
                var repeatList = _UnitOfWork.IRepeatRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                scheduleBatchSurveyViewModelResponse.RepeatList = repeatList.Select(x => new SelectListItem { Text = x.RepeatInterval, Value = x.RepeatID.ToString() }).ToList();
                var surveyList = _UnitOfWork.ISurveyRepository.RetrieveAll().Where(u => u.AdminID == ScheduleBatchSurveyViewModel.LoggedInUserId && u.IsDeleted != true).ToList();
                scheduleBatchSurveyViewModelResponse.SurveyList = surveyList.Select(x => new SelectListItem { Text = x.SurveyName, Value = x.SurveyID.ToString() }).ToList();
                scheduleBatchSurveyViewModelResponse.SurveyList.Insert(0, new SelectListItem { Text = "Select", Value = "0" });

                var cognitionTestList = _UnitOfWork.ICTestRepository.RetrieveAll().Where(u => u.IsDeleted != true).OrderBy(u => u.SortOrder).ToList();
                scheduleBatchSurveyViewModelResponse.CognitionTestList = cognitionTestList.Select(x => new SelectListItem { Text = x.CTestName, Value = x.CTestID.ToString(), Selected = true }).ToList();


                var fisrtCtestselected = scheduleBatchSurveyViewModelResponse.CognitionTestList.ElementAt(0).Value;
                var cognitionTestVerisonList = GetCognitionVersion(Convert.ToInt64(fisrtCtestselected)).ToList();
                scheduleBatchSurveyViewModelResponse.CognitionTestList.Insert(0, new SelectListItem { Text = "Select", Value = "0" });

                scheduleBatchSurveyViewModelResponse.CognitionVersionList = cognitionTestVerisonList.Select(x => new SelectListItem { Text = x.Text.ToString(), Value = x.Value.ToString(), Selected = true }).ToList();
                scheduleBatchSurveyViewModelResponse.GameScheduleDateString = DateTime.Now.ToString("dd/MM/yyyy");
                scheduleBatchSurveyViewModelResponse.SurveyScheduleDateString = DateTime.Now.ToString("dd/MM/yyyy");

            }
            catch (Exception ex)
            {
                LogUtil.Error("BatchScheduleService/GetScheduleViewModelDetailsForAdmin: " + ex);
                scheduleBatchSurveyViewModelResponse = new ScheduleBatchViewModel
                {
                    Status = LAMPConstants.UNEXPECTED_ERROR,
                    Message = ResourceHelper.GetStringResource(LAMPConstants.UNEXPECTED_ERROR)
                };
            }
            return scheduleBatchSurveyViewModelResponse;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the slot time options for Batch.
        /// </summary>
        /// <param name="adminCTestSchID"></param>
        /// <returns></returns>
        private List<string> GetSlotTimeOptionsForBatch(long adminSchID)
        {
            List<string> options = new List<string>();
            var SlotTimeOptionsForSurveyList = (from questions in _UnitOfWork.IAdminBatchScheduleCustomTimeRepository.GetAll().Where(d => d.AdminBatchSchID == adminSchID)
                                                select new Admin_BatchScheduleCustomTime
                                                {
                                                    Time = questions.Time
                                                }).ToList();
            foreach (var item in SlotTimeOptionsForSurveyList)
            {
                DateTime surveyDisplayTime = (DateTime)(item.Time);
                var Timestamp = surveyDisplayTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                options.Add(Timestamp.ToString());
            }
            return options;

        }

        /// <summary>
        /// Gets the cognition version.
        /// </summary>
        /// <param name="cognitionId">The cognition identifier.</param>
        /// <returns></returns>
        private List<SelectListItem> GetCognitionVersion(long cognitionId)
        {
            var cTest = _UnitOfWork.ICTestRepository.SingleOrDefault(u => u.CTestID == cognitionId);
            List<int> versions = new List<int>();
            if (cTest.MaxVersions != null)
            {
                versions = Enumerable.Range(1, cTest.MaxVersions.Value).ToList();
            }
            List<SelectListItem> item = new List<SelectListItem>();
            if (versions.Count == 0)
            {

                SelectListItem select = new SelectListItem(); select.Text = "No Version"; select.Value = "-1";
                item.Insert(0, select);
            }
            else
            {
                item = versions.ConvertAll(a =>
                {
                    return new SelectListItem()
                    {
                        Text = a.ToString(),
                        Value = a.ToString(),
                        Selected = false
                    };
                });
            }
            return item;
        }
        #endregion
    }
}
