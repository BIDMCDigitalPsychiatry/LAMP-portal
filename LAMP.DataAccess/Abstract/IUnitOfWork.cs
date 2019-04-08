using System;
using LAMP.DataAccess.Entities;

namespace LAMP.DataAccess
{
    /// <summary>
    /// Interface IUnitOfWork for capable of class UnitOfWork
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Admin> IAdminRepository { get; }
        IGenericRepository<User> IUserRepository { get; }
        IGenericRepository<UserDevice> IUserDeviceRepository { get; }
        IGenericRepository<UserSetting> IUserSettingRepository { get; }
        IGenericRepository<SurveyResult> ISurveyResultRepository { get; }
        IGenericRepository<SurveyResultDtl> ISurveyResultDtlRepository { get; }
        IGenericRepository<CTest_CatAndDogResult> ICatAndDogResultRepository { get; }
        IGenericRepository<CTest_NBackResult> INBackResultRepository { get; }
        IGenericRepository<CTest_Serial7Result> ISerial7ResultRepository { get; }
        IGenericRepository<CTest_SimpleMemoryResult> ISimpleMemoryResultRepository { get; }
        IGenericRepository<CTest_3DFigure> I3DFigureRepository { get; }
        IGenericRepository<CTest_3DFigureResult> I3DFigureResultRepository { get; }
        IGenericRepository<CTest_DigitSpanResult> IDigitSpanResultRepository { get; }
        IGenericRepository<CTest_TrailsBResult> ITrailsBResultRepository { get; }
        IGenericRepository<CTest_VisualAssociationResult> IVisualAssociationResultRepository { get; }
        IGenericRepository<CTest_SpatialResult> ISpatialSpanResultRepository { get; }
        IGenericRepository<CTest_SpatialResultDtl> ISpatialResultDtlRepository { get; }
        IGenericRepository<CTest_CatAndDogNewResult> ICatAndDogNewResultRepository { get; }
        IGenericRepository<CTest_TemporalOrderResult> ITemporalOrderResultRepository { get; }
        IGenericRepository<Location> ILocationRepository { get; }
        IGenericRepository<HelpCall> IHelpCallRepository { get; }
        IGenericRepository<Tip> ITipRepository { get; }
        IGenericRepository<Blog> IBlogRepository { get; }
        IGenericRepository<CTest_TrailsBResultDtl> ITrailsBResultDtlRepository { get; }
        IGenericRepository<Survey> ISurveyRepository { get; }
        IGenericRepository<SurveyQuestion> ISurveyQuestionRepository { get; }
        IGenericRepository<SurveyQuestionOption> ISurveyQuestionOptionRepository { get; }
        IGenericRepository<AppHelp> IAppHelpRepository { get; }
        IGenericRepository<Slot> ISlotRepository { get; }
        IGenericRepository<Repeat> IRepeatRepository { get; }
        IGenericRepository<CTest> ICTestRepository { get; }
        IGenericRepository<Admin_CTestSurveySettings> IAdminCTestSurveyRepository { get; }
        IGenericRepository<CTest_NBackNewResult> INBackNewResultRepository { get; }
        IGenericRepository<HealthKit_BasicInfo> IHealthKitBasicInfoRepository { get; }
        IGenericRepository<HealthKit_DailyValues> IHealthKitDailyValuesRepository { get; }
        IGenericRepository<CTest_TrailsBNewResult> ITrailsBNewResultRepository { get; }
        IGenericRepository<CTest_TrailsBNewResultDtl> ITrailsBNewResultDtlRepository { get; }
        IGenericRepository<CTest_TrailsBDotTouchResult> ITrailsBDotTouchResultRepository { get; }
        IGenericRepository<CTest_TrailsBDotTouchResultDtl> ITrailsBDotTouchResultDtlRepository { get; }
        IGenericRepository<CTest_JewelsTrailsAResult> IJewelsTrailsAResultRepository { get; }
        IGenericRepository<CTest_JewelsTrailsAResultDtl> IJewelsTrailsAResultDtlRepository { get; }
        IGenericRepository<CTest_JewelsTrailsBResult> IJewelsTrailsBResultRepository { get; }
        IGenericRepository<CTest_JewelsTrailsBResultDtl> IJewelsTrailsBResultDtlRepository { get; }
        IGenericRepository<Admin_CTestSchedule> IAdminCTestScheduleRepository { get; }
        IGenericRepository<Admin_SurveySchedule> IAdminSurveyScheduleRepository { get; }
        IGenericRepository<Admin_JewelsTrailsASettings> IAdminJewelsTrailsASettingsRepository { get; }
        IGenericRepository<Admin_JewelsTrailsBSettings> IAdminJewelsTrailsBSettingsRepository { get; }
        IGenericRepository<Admin_CTestScheduleCustomTime> IAdminCTestScheduleCustomTimeRepository { get; }
        IGenericRepository<Admin_SurveyScheduleCustomTime> IAdminSurveyScheduleCustomTimeRepository { get; }
        IGenericRepository<Admin_Settings> IAdminSettingsRepository { get; }
        IGenericRepository<Admin_BatchSchedule> IAdminBatchScheduleRepository { get; }
        IGenericRepository<Admin_BatchScheduleCTest> IAdminBatchScheduleCTestRepository { get; }
        IGenericRepository<Admin_BatchScheduleSurvey> IAdminBatchScheduleSurveyRepository { get; }
        IGenericRepository<Admin_BatchScheduleCustomTime> IAdminBatchScheduleCustomTimeRepository { get; }
        IGenericRepository<CTest_ScratchImage> IScratchImageRepository { get; }
        IGenericRepository<CTest_ScratchImageResult> IScratchImageResultRepository { get; }        
        IGenericRepository<CTest_CatAndDogNewResultDtl> ICatAndDogNewResultDtlRepository { get; }
        IGenericRepository<CTest_SpinWheelResult> ISpinWheelResultRepository { get; }

        void Commit();
    }

}
