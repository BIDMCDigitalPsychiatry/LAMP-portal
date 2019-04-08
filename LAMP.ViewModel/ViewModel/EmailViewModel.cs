using System.Collections.Generic;

namespace LAMP.ViewModel
{
    /// <summary>
    /// Class EmailViewModel
    /// </summary>
    public class EmailViewModel
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string TemplateName { get; set; }
    }

    /// <summary>
    /// Class EmailData 
    /// </summary>
    public class EmailData : EmailViewModel
    {
        public List<replaceingData> Data { get; set; }
        public EmailData()
        {
            Data = new List<replaceingData>();
        }  
    }

    /// <summary>
    /// Class replaceingData
    /// </summary>
    public class replaceingData
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public replaceingData()
        {
            Name = "";
            Value = "";
        }  
    }
}
