using LAMP.ViewModel;

namespace LAMP.Service
{
    public interface IScheduleService
    {
        /// <summary>
        /// Get Scheduled List
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ScheduleListViewModel GetBatchScheduledList(ScheduleListViewModel model);

        /// <summary>
        ///  Save schedule
        /// </summary>
        /// <param name="ScheduleGameSurveyViewModel"></param>
        /// <returns></returns>
        //ScheduleBatchViewModel SaveBatchShedule(ScheduleBatchViewModel ScheduleBatchViewModel);
        ScheduleGameSurveyViewModel SaveBatchShedule(ScheduleBatchViewModel ScheduleBatchViewModel);

        /// <summary>
        /// Get batch ScheduleViewModel Details For Admin
        /// </summary>
        /// <param name="ScheduleBatchSurveyViewModel"></param>
        /// <returns></returns>
        ScheduleBatchViewModel GetScheduleViewModelDetailsForAdmin(ScheduleBatchViewModel ScheduleBatchSurveyViewModel);

    }
}
