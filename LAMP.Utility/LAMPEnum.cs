

namespace LAMP.Utility
{
    /// <summary>
    /// LoginType
    /// </summary>
    public enum LoginType
    {
        Email = 1,
        StudyId = 2
    };
    /// <summary>
    /// UserType
    /// </summary>
    public enum UserType
    {
        User = 0,
        Guest = 1
    };
    /// <summary>
    /// EntryMode
    /// </summary>
    public enum EntryMode
    {
        New = 0,
        Update = 1
    }
    /// <summary>
    /// PageConditions
    /// </summary>
    public enum PageConditions
    {
        SortColumn = 0,
        SortOrder = 1,
        PageSize = 2,
        CurrentPage = 3
    }

    /// <summary>
    /// CognitionType
    /// </summary>
    public enum CognitionType
    {
        NBack = 1,
        TrailsB = 2,
        SpatialSpan = 3,
        SpatialSpanNew = 4,
        SimpleMemory = 5,
        Serial7 = 6,
        CatAndDog = 7,
        ThreeDFigure = 8,
        VisualAssociation = 9,
        DigitSpan = 10,
        CatAndDogNew = 11,
        TemporalOrder = 12,
        DigitSpanBackward = 13,
        NBackNew = 14,
        TrailsBNew = 15,
        TrailsBDotTouch = 16,
        JewelsTrailsA = 17,
        JewelsTrailsB=18,
        ScratchImage=19,
        SpinWheel=20
    }

    /// <summary>
    /// EntryMode
    /// </summary>
    public enum SpatialType
    {
        Forward = 1,
        Backward = 2
    }

    /// <summary>
    /// EntryMode
    /// </summary>
    public enum DigitalSpanType
    {
        Forward = 1,
        Backward = 2
    }

    /// <summary>
    /// Admin Role Types
    /// </summary>
    public enum AdminRoles
    {
        SuperAdmin = 1,
        Admin = 2,
    }

    public enum ExitOrCompleteStatus
    {
        Exited = 1,
        Completed = 2
    }

    public enum ReminderClearInterval
    {
        None = 1,
        OneHour = 2,
        SixHours = 3
    }

    public enum HealthKitParameter
    {
        Height = 1,
        Weight,
        HeartRate,
        BloodPressure,
        RespiratoryRate,
        Sleep,
        Steps,
        FlightClimbed,
        Segment,
        Distance
    }

}



