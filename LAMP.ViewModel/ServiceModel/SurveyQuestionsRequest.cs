using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Survey Questions Request
    /// </summary>
    public class SurveyQuestionsRequest 
    {
        public long UserID { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        
    }
}
