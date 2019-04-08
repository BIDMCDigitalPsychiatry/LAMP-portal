using System;
using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Distraction Survey Response
    /// </summary>
    public class DistractionSurveyResponse : APIResponseBase
    {
        public List<DistractionSurvey> Surveys { get; set; }
    }

    public class DistractionSurvey
    {
        public long? SurveyId { get; set; }
    }
}
