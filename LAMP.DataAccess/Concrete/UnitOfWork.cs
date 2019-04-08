using LAMP.DataAccess.Entities;

namespace LAMP.DataAccess
{
    /// <summary>
    /// UnitOfWork is responsible for handling entity framework acitivities
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        // Context Object
        private LAMPEntities context = new LAMPEntities();

        private readonly GenericRepository<Admin> _adminRepository; 
        private readonly GenericRepository<User> _userRepository;
        private readonly GenericRepository<UserDevice> _userDeviceRepository;
        private readonly GenericRepository<UserSetting> _userSettingRepository;
        private readonly GenericRepository<SurveyResult> _surveyResultRepository;
        private readonly GenericRepository<SurveyResultDtl> _surveyResultDtlRepository;
        private readonly GenericRepository<CTest_CatAndDogResult> _catAndDogResultRepository;
        private readonly GenericRepository<CTest_NBackResult> _nBackResultRepository;
        private readonly GenericRepository<CTest_Serial7Result> _serial7ResultRepository;
        private readonly GenericRepository<CTest_SimpleMemoryResult> _simpleMemoryResultRepository;
        private readonly GenericRepository<CTest_3DFigure> _3DFigureRepository;
        private readonly GenericRepository<CTest_3DFigureResult> _3DFigureResultRepository;
        private readonly GenericRepository<CTest_DigitSpanResult> _digitSpanResultRepository;
        private readonly GenericRepository<CTest_TrailsBResult> _trailsBResultRepository;
        private readonly GenericRepository<CTest_VisualAssociationResult> _visualAssociationResultRepository;
        private readonly GenericRepository<CTest_SpatialResult> _spatialSpanResultRepository;
        private readonly GenericRepository<CTest_SpatialResultDtl> _spatialResultDtlRepository;
        private readonly GenericRepository<CTest_CatAndDogNewResult> _catAndDogNewResultRepository;
        private readonly GenericRepository<CTest_TemporalOrderResult> _temporalOrderResultRepository;
        private readonly GenericRepository<Location> _environmentRepository;
        private readonly GenericRepository<HelpCall> _helpCallRepository;
        private readonly GenericRepository<Tip> _tipRepository;
        private readonly GenericRepository<Blog> _blogRepository;
        private readonly GenericRepository<CTest_TrailsBResultDtl> _trialsBresultDtlRepository;
        private readonly GenericRepository<Survey> _surveyRepository;
        private readonly GenericRepository<SurveyQuestion> _surveyQuestionRepository;
        private readonly GenericRepository<SurveyQuestionOption> _surveyQuestionOptionRepository;
        private readonly GenericRepository<AppHelp> _appHelpRepository;
        private readonly GenericRepository<Slot> _slotRepository;
        private readonly GenericRepository<Repeat> _repeatRepository;
        private readonly GenericRepository<CTest> _cTestRepository;
        private readonly GenericRepository<Admin_CTestSurveySettings> _adminCTestSurveyRepository;
        private readonly GenericRepository<CTest_NBackNewResult> _nBackNewResultRepository;
        private readonly GenericRepository<HealthKit_BasicInfo> _healthKitBasicInfoRepository;
        private readonly GenericRepository<HealthKit_DailyValues> _healthKitDailyValuesRepository;
        private readonly GenericRepository<CTest_TrailsBNewResult> _trailsBNewResultRepository;
        private readonly GenericRepository<CTest_TrailsBNewResultDtl> _trailsBNewResultDtlRepository;
        private readonly GenericRepository<CTest_TrailsBDotTouchResult> _trailsBDotTouchResultRepository;
        private readonly GenericRepository<CTest_TrailsBDotTouchResultDtl> _trailsBDotTouchResultDtlRepository;
        private readonly GenericRepository<CTest_JewelsTrailsAResult> _jewelsTrailsAResultRepository;
        private readonly GenericRepository<CTest_JewelsTrailsAResultDtl> _jewelsTrailsAResultDtlRepository;
        private readonly GenericRepository<CTest_JewelsTrailsBResult> _jewelsTrailsBResultRepository;
        private readonly GenericRepository<CTest_JewelsTrailsBResultDtl> _jewelsTrailsBResultDtlRepository;
        private readonly GenericRepository<Admin_CTestSchedule> _adminCTestScheduleRepository;
        private readonly GenericRepository<Admin_SurveySchedule> _adminSurveyScheduleRepository;
        private readonly GenericRepository<Admin_JewelsTrailsASettings> _adminJewelsTrailsASettingsRepository;
        private readonly GenericRepository<Admin_JewelsTrailsBSettings> _adminJewelsTrailsBSettingsRepository;
        private readonly GenericRepository<Admin_CTestScheduleCustomTime> _adminCTestScheduleCustomTimeRepository;
        private readonly GenericRepository<Admin_SurveyScheduleCustomTime> _adminSurveyScheduleCustomTimeRepository;
        private readonly GenericRepository<Admin_Settings> _adminSettingsRepository;
        private readonly GenericRepository<Admin_BatchSchedule> _adminBatchScheduleRepository;
        private readonly GenericRepository<Admin_BatchScheduleCTest> _adminBatchScheduleCTestRepository;
        private readonly GenericRepository<Admin_BatchScheduleSurvey> _adminBatchScheduleSurveyRepository;
        private readonly GenericRepository<Admin_BatchScheduleCustomTime> _adminBatchScheduleCustomTimeyRepository;
        private readonly GenericRepository<CTest_ScratchImage> _scratchImageRepository;
        private readonly GenericRepository<CTest_ScratchImageResult> _scratchImageResultRepository;
        private readonly GenericRepository<CTest_CatAndDogNewResultDtl> _catAndDogNewResultDtlRepository;
        private readonly GenericRepository<CTest_SpinWheelResult> _spinWheelResultRepository;
        public UnitOfWork()
        {
            _adminRepository = new GenericRepository<Admin>(context);
            _userRepository = new GenericRepository<User>(context);
            _userDeviceRepository = new GenericRepository<UserDevice>(context);
            _userSettingRepository = new GenericRepository<UserSetting>(context);
            _surveyResultRepository = new GenericRepository<SurveyResult>(context);
            _surveyResultDtlRepository = new GenericRepository<SurveyResultDtl>(context);
            _catAndDogResultRepository = new GenericRepository<CTest_CatAndDogResult>(context);
            _nBackResultRepository = new GenericRepository<CTest_NBackResult>(context);
            _serial7ResultRepository = new GenericRepository<CTest_Serial7Result>(context);
            _simpleMemoryResultRepository = new GenericRepository<CTest_SimpleMemoryResult>(context);
            _3DFigureRepository = new GenericRepository<CTest_3DFigure>(context);
            _3DFigureResultRepository = new GenericRepository<CTest_3DFigureResult>(context);
            _digitSpanResultRepository = new GenericRepository<CTest_DigitSpanResult>(context);
            _trailsBResultRepository = new GenericRepository<CTest_TrailsBResult>(context);
            _visualAssociationResultRepository = new GenericRepository<CTest_VisualAssociationResult>(context);
            _spatialSpanResultRepository = new GenericRepository<CTest_SpatialResult>(context);
            _spatialResultDtlRepository = new GenericRepository<CTest_SpatialResultDtl>(context);
            _catAndDogNewResultRepository = new GenericRepository<CTest_CatAndDogNewResult>(context);
            _temporalOrderResultRepository = new GenericRepository<CTest_TemporalOrderResult>(context);
            _environmentRepository = new GenericRepository<Location>(context);
            _helpCallRepository = new GenericRepository<HelpCall>(context);
            _tipRepository = new GenericRepository<Tip>(context);
            _blogRepository = new GenericRepository<Blog>(context);
            _trialsBresultDtlRepository = new GenericRepository<CTest_TrailsBResultDtl>(context);
            _surveyRepository = new GenericRepository<Survey>(context);
            _surveyQuestionRepository = new GenericRepository<SurveyQuestion>(context);
            _surveyQuestionOptionRepository = new GenericRepository<SurveyQuestionOption>(context);
            _appHelpRepository = new GenericRepository<AppHelp>(context);
            _slotRepository = new GenericRepository<Slot>(context);
            _repeatRepository = new GenericRepository<Repeat>(context);
            _cTestRepository = new GenericRepository<CTest>(context);
            _adminCTestSurveyRepository = new GenericRepository<Admin_CTestSurveySettings>(context);
            _nBackNewResultRepository = new GenericRepository<CTest_NBackNewResult>(context);
            _healthKitBasicInfoRepository = new GenericRepository<HealthKit_BasicInfo>(context);
            _healthKitDailyValuesRepository = new GenericRepository<HealthKit_DailyValues>(context);
            _trailsBNewResultRepository = new GenericRepository<CTest_TrailsBNewResult>(context);
            _trailsBNewResultDtlRepository = new GenericRepository<CTest_TrailsBNewResultDtl>(context);
            _trailsBDotTouchResultRepository = new GenericRepository<CTest_TrailsBDotTouchResult>(context);
            _trailsBDotTouchResultDtlRepository = new GenericRepository<CTest_TrailsBDotTouchResultDtl>(context);
            _jewelsTrailsAResultRepository = new GenericRepository<CTest_JewelsTrailsAResult>(context);
            _jewelsTrailsAResultDtlRepository = new GenericRepository<CTest_JewelsTrailsAResultDtl>(context);
            _jewelsTrailsBResultRepository = new GenericRepository<CTest_JewelsTrailsBResult>(context);
            _jewelsTrailsBResultDtlRepository = new GenericRepository<CTest_JewelsTrailsBResultDtl>(context);
            _adminCTestScheduleRepository = new GenericRepository<Admin_CTestSchedule>(context);
            _adminSurveyScheduleRepository = new GenericRepository<Admin_SurveySchedule>(context);
            _adminJewelsTrailsASettingsRepository = new GenericRepository<Admin_JewelsTrailsASettings>(context);
            _adminJewelsTrailsBSettingsRepository = new GenericRepository<Admin_JewelsTrailsBSettings>(context);
            _adminCTestScheduleCustomTimeRepository = new GenericRepository<Admin_CTestScheduleCustomTime>(context);
            _adminSurveyScheduleCustomTimeRepository = new GenericRepository<Admin_SurveyScheduleCustomTime>(context);
            _adminSettingsRepository = new GenericRepository<Admin_Settings>(context);
            _adminBatchScheduleRepository = new GenericRepository<Admin_BatchSchedule>(context);
            _adminBatchScheduleCTestRepository = new GenericRepository<Admin_BatchScheduleCTest>(context);
            _adminBatchScheduleSurveyRepository = new GenericRepository<Admin_BatchScheduleSurvey>(context);
            _adminBatchScheduleCustomTimeyRepository = new GenericRepository<Admin_BatchScheduleCustomTime>(context);
             _scratchImageRepository = new GenericRepository<CTest_ScratchImage>(context);
            _scratchImageResultRepository = new GenericRepository<CTest_ScratchImageResult>(context);
            _catAndDogNewResultDtlRepository = new GenericRepository<CTest_CatAndDogNewResultDtl>(context);
            _spinWheelResultRepository = new GenericRepository<CTest_SpinWheelResult>(context);
        }    


        public IGenericRepository<Admin> IAdminRepository
        {
            get { return _adminRepository; }
        }
        public IGenericRepository<User> IUserRepository
        {
            get { return _userRepository; }
        }
        public IGenericRepository<UserDevice> IUserDeviceRepository
        {
            get { return _userDeviceRepository; }
        }
        public IGenericRepository<UserSetting> IUserSettingRepository
        {
            get { return _userSettingRepository; }
        }

        public IGenericRepository<SurveyResult> ISurveyResultRepository
        {
            get { return _surveyResultRepository; }
        }
        public IGenericRepository<SurveyResultDtl> ISurveyResultDtlRepository
        {
            get { return _surveyResultDtlRepository; }
        }
        public IGenericRepository<CTest_CatAndDogResult> ICatAndDogResultRepository
        {
            get { return _catAndDogResultRepository; }
        }
        public IGenericRepository<CTest_NBackResult> INBackResultRepository
        {
            get { return _nBackResultRepository; }
        }
        public IGenericRepository<CTest_Serial7Result> ISerial7ResultRepository
        {
            get { return _serial7ResultRepository; }
        }
        public IGenericRepository<CTest_SimpleMemoryResult> ISimpleMemoryResultRepository
        {
            get { return _simpleMemoryResultRepository; }
        }
        public IGenericRepository<CTest_3DFigure> I3DFigureRepository
        {
            get { return _3DFigureRepository; }
        }
        public IGenericRepository<CTest_3DFigureResult> I3DFigureResultRepository
        {
            get { return _3DFigureResultRepository; }
        }
        public IGenericRepository<CTest_DigitSpanResult> IDigitSpanResultRepository
        {
            get { return _digitSpanResultRepository; }
        }
        public IGenericRepository<CTest_TrailsBResult> ITrailsBResultRepository
        {
            get { return _trailsBResultRepository; }
        }
        public IGenericRepository<CTest_VisualAssociationResult> IVisualAssociationResultRepository
        {
            get { return _visualAssociationResultRepository; }
        }
        public IGenericRepository<CTest_SpatialResult> ISpatialSpanResultRepository
        {
            get { return _spatialSpanResultRepository; }
        }
        public IGenericRepository<CTest_SpatialResultDtl> ISpatialResultDtlRepository
        {
            get { return _spatialResultDtlRepository; }
        }
        public IGenericRepository<CTest_CatAndDogNewResult> ICatAndDogNewResultRepository
        {
            get { return _catAndDogNewResultRepository; }
        }
        public IGenericRepository<CTest_TemporalOrderResult> ITemporalOrderResultRepository
        {
            get { return _temporalOrderResultRepository; }
        }
        public IGenericRepository<Location> ILocationRepository
        {
            get { return _environmentRepository; }
        }
        public IGenericRepository<HelpCall> IHelpCallRepository
        {
            get { return _helpCallRepository; }
        }
        public IGenericRepository<Tip> ITipRepository
        {
            get { return _tipRepository; }
        }
        public IGenericRepository<Blog> IBlogRepository
        {
            get { return _blogRepository; }
        }
        public IGenericRepository<CTest_TrailsBResultDtl> ITrailsBResultDtlRepository
        {
            get { return _trialsBresultDtlRepository; }
        }
        public IGenericRepository<Survey> ISurveyRepository
        {
            get { return _surveyRepository; }
        }
        public IGenericRepository<SurveyQuestion> ISurveyQuestionRepository
        {
            get { return _surveyQuestionRepository; }
        }
        public IGenericRepository<SurveyQuestionOption> ISurveyQuestionOptionRepository
        {
            get { return _surveyQuestionOptionRepository; }
        }
        public IGenericRepository<AppHelp> IAppHelpRepository
        {
            get { return _appHelpRepository; }
        }
        public IGenericRepository<Slot> ISlotRepository
        {
            get { return _slotRepository; }
        }
        public IGenericRepository<Repeat> IRepeatRepository
        {
            get { return _repeatRepository; }
        }
        public IGenericRepository<CTest> ICTestRepository
        {
            get { return _cTestRepository; }
        }
        public IGenericRepository<Admin_CTestSurveySettings> IAdminCTestSurveyRepository
        {
            get { return _adminCTestSurveyRepository; }
        }
        public IGenericRepository<CTest_NBackNewResult> INBackNewResultRepository
        {
            get { return _nBackNewResultRepository; }
        }
        public IGenericRepository<HealthKit_BasicInfo> IHealthKitBasicInfoRepository
        {
            get { return _healthKitBasicInfoRepository; }
        }
        public IGenericRepository<HealthKit_DailyValues> IHealthKitDailyValuesRepository
        {
            get { return _healthKitDailyValuesRepository; }
        }
        public IGenericRepository<CTest_TrailsBNewResult> ITrailsBNewResultRepository
        {
            get { return _trailsBNewResultRepository; }
        }
        public IGenericRepository<CTest_TrailsBNewResultDtl> ITrailsBNewResultDtlRepository
        {
            get { return _trailsBNewResultDtlRepository; }
        }
        public IGenericRepository<CTest_TrailsBDotTouchResult> ITrailsBDotTouchResultRepository
        {
            get { return _trailsBDotTouchResultRepository; }
        }
        public IGenericRepository<CTest_TrailsBDotTouchResultDtl> ITrailsBDotTouchResultDtlRepository
        {
            get { return _trailsBDotTouchResultDtlRepository; }
        }
        public IGenericRepository<CTest_JewelsTrailsAResult> IJewelsTrailsAResultRepository
        {
            get { return _jewelsTrailsAResultRepository; }
        }
        public IGenericRepository<CTest_JewelsTrailsAResultDtl> IJewelsTrailsAResultDtlRepository
        {
            get { return _jewelsTrailsAResultDtlRepository; }
        }
        public IGenericRepository<CTest_JewelsTrailsBResult> IJewelsTrailsBResultRepository
        {
            get { return _jewelsTrailsBResultRepository; }
        }
        public IGenericRepository<CTest_JewelsTrailsBResultDtl> IJewelsTrailsBResultDtlRepository
        {
            get { return _jewelsTrailsBResultDtlRepository; }
        }
        public IGenericRepository<Admin_CTestSchedule> IAdminCTestScheduleRepository
        {
            get { return _adminCTestScheduleRepository; }
        }
        public IGenericRepository<Admin_SurveySchedule> IAdminSurveyScheduleRepository
        {
            get { return _adminSurveyScheduleRepository; }
        }
        public IGenericRepository<Admin_JewelsTrailsASettings> IAdminJewelsTrailsASettingsRepository
        {
            get { return _adminJewelsTrailsASettingsRepository; }
        }
        public IGenericRepository<Admin_JewelsTrailsBSettings> IAdminJewelsTrailsBSettingsRepository
        {
            get { return _adminJewelsTrailsBSettingsRepository; }
        }
        public IGenericRepository<Admin_CTestScheduleCustomTime> IAdminCTestScheduleCustomTimeRepository
        {
            get { return _adminCTestScheduleCustomTimeRepository; }
        }
        public IGenericRepository<Admin_SurveyScheduleCustomTime> IAdminSurveyScheduleCustomTimeRepository
        {
            get { return _adminSurveyScheduleCustomTimeRepository; }
        }
        public IGenericRepository<Admin_Settings> IAdminSettingsRepository
        {
            get { return _adminSettingsRepository; }
        }
        public IGenericRepository<Admin_BatchSchedule> IAdminBatchScheduleRepository
        {
            get { return _adminBatchScheduleRepository; }
        }
        public IGenericRepository<Admin_BatchScheduleCTest> IAdminBatchScheduleCTestRepository
        {
            get { return _adminBatchScheduleCTestRepository; }
        }
        public IGenericRepository<Admin_BatchScheduleSurvey> IAdminBatchScheduleSurveyRepository
        {
            get { return _adminBatchScheduleSurveyRepository; }
        }
        public IGenericRepository<Admin_BatchScheduleCustomTime> IAdminBatchScheduleCustomTimeRepository
        {
            get { return _adminBatchScheduleCustomTimeyRepository; }
        }
        public IGenericRepository<CTest_ScratchImage> IScratchImageRepository
        {
            get { return _scratchImageRepository; }
        }
        public IGenericRepository<CTest_ScratchImageResult> IScratchImageResultRepository
        {
            get { return _scratchImageResultRepository; }
        }
        public IGenericRepository<CTest_CatAndDogNewResultDtl> ICatAndDogNewResultDtlRepository
        {
            get { return _catAndDogNewResultDtlRepository; }
        }
        public IGenericRepository<CTest_SpinWheelResult> ISpinWheelResultRepository
        {
            get { return _spinWheelResultRepository; }
        }
        public void Commit()
        {
            context.SaveChanges();
        }
        public void Dispose()
        {
            context.Dispose();
        }
       
    }
}
