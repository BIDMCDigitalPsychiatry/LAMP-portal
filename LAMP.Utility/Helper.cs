using LAMP.ViewModel;
using System;
using System.Globalization;
using System.IO;

namespace LAMP.Utility
{
    /// <summary>
    /// Helper class
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Get Date time value
        /// </summary>
        /// <param name="date">date format as yyyy/M/d 00:00:00, yyyy/MM/dd 00:00:00, yyyy/M/d, yyyy/MM/dd</param>
        /// <returns></returns>
        public static DateTime GetDateTime(string date)
        {
            CultureInfo cultureUS = new CultureInfo("en-US");
            string[] formats = new string[] { "yyyy/M/d HH:mm:ss", "yyyy/MM/dd", "yyyy/M/d" };
            DateTime result = DateTime.ParseExact(date, formats, cultureUS, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            return result;
        }

        /// <summary>
        /// Get date string specic format
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetDateString(DateTime? dt,string format)
        {
            if (dt == null)
                return string.Empty;
            else
                return ((DateTime)dt).ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get Offset value
        /// </summary>
        /// <param name="offsetValue">Offset Value</param>
        /// <returns>Offset Value</returns>
        public static double GetOffsetValue(double offsetValue)
        {
            return offsetValue * -1;
        }

        /// <summary>
        /// Upload image to a folder
        /// </summary>
        /// <param name="image">Base64 String</param>
        /// <param name="fileName">FileName with extension</param>
        /// <param name="userId">User Id</param>
        /// <param name="targetLocation">Target Location for Save</param>
        /// <returns>FileName</returns>
        public static string UploadImage(string image, string fileName, string userId, string targetLocation)
        {
            string diskFileName = userId + "_" + Guid.NewGuid().ToString();
            try
            {
                if (image != null)
                {
                    byte[] fileDataBytes = Convert.FromBase64String(image);
                     string fileSaveLocation = targetLocation; 
                    diskFileName += Path.GetExtension(fileName);
                    fileSaveLocation = Path.Combine(fileSaveLocation, diskFileName);
                    File.Create(fileSaveLocation).Close();
                    using (FileStream fs = new FileStream(fileSaveLocation, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                    {
                        fs.Write(fileDataBytes, 0, fileDataBytes.Length);
                    }
                }
                return diskFileName;
            }
            catch (Exception ex)
            {
                LogUtil.Error("Helper/UploadImage:- Could not upload image: " + ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Get Uri String
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="baseUriObject">Uri Object</param>
        /// <returns>File Path</returns>
        public static string GetUriString(string fileName, Uri baseUriObject)
        {
            string filePath = string.Empty;
            try
            {
                 return filePath;
            }
            catch (Exception ex)
            {
                LogUtil.Error("Helper/GetUriString:- Image path issue: " + ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="fileName">File name</param>
        public static void DeleteFile(string fileName)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                LogUtil.Error("Helper/DeleteFile:- Error while delete file: " + ex);
            }
        }

        /// <summary>
        /// Get Decimal value
        /// </summary>
        /// <param name="val">Object Value</param>
        /// <returns>Decimal value</returns>
        public static decimal GetDecimal(object val)
        {
            try
            {
                return Convert.ToDecimal(val);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Round Decimal value with Precision
        /// </summary>
        /// <param name="val"></param>
        /// <param name="precision"></param>
        /// <returns>Decimal value</returns>
        public static decimal GetDecimalWithPrecision(object val, int precision)
        {
            try
            {
                decimal value = Convert.ToDecimal(val);
                return Math.Round(value, precision);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Get Exact Division Value
        /// </summary>
        /// <param name="quotient">Quotient</param>
        /// <param name="remainder">Remainder</param>
        /// <returns>Decimal value</returns>
        public static decimal GetExactDivisionValue(decimal quotient, decimal remainder)
        {
            string strValue = string.Format("{0}.{1}", quotient, remainder);
            return Math.Round(Convert.ToDecimal(strValue), 3);
        }

        /// <summary>
        /// To check whether text is email address or not
        /// </summary>
        /// <param name="email">email text</param>
        /// <returns>Status</returns>
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="email">email text</param>
        /// <returns></returns>
        public static void SendEmail(EmailData Edata)
        {            
            string template = Edata.TemplateName;            
            var fileName = System.Web.Hosting.HostingEnvironment.MapPath(template);
            var fileContent ="";
            var reader = new StreamReader(fileName);
            fileContent = reader.ReadToEnd();
            foreach (var data in Edata.Data)
            {
                fileContent = fileContent.Replace("%" + data.Name + "%", data.Value);
            }
            ResourceHelper.SendEmailUsingSMTP(Edata.Email, Edata.Subject, fileContent);
        }

        /// <summary>
        /// Create file in folder
        /// </summary>
        /// <param name="fileFullPath">Folder path</param>
        /// <param name="stream">Image stream</param>
        /// <returns>Status</returns>
        public static bool SaveStreamToFile(string fileFullPath, Stream stream)
        {
            try
            {
                if (stream == null || stream.Length == 0) return false;

                // Create a FileStream object to write a stream to a file
                using (FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
                {
                    // Fill the bytes[] array with the stream data
                    byte[] bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                    // Use FileStream object to write to the specified file
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// GetNemericCode
        /// </summary>
        /// <returns></returns>
        public static string GetNemericCode()
        {
            return "U" + CryptoUtil.CreateNumericSalt(5).ToString();
        }

        /// <summary>
        /// Get Rating on Rating Id
        /// </summary>
        /// <param name="rating">Rating Id</param>
        /// <returns>Rating</returns>
        public static string GetRating(Int16 rating)
        {
            string lastResultRating = string.Empty;
            switch (rating)
            {
                case -1:
                    lastResultRating = "-";
                    break;
                case 0:
                    lastResultRating = "Bad";
                    break;
                case 1:
                    lastResultRating = "Good";
                    break;
                case 2:
                    lastResultRating = "Average";
                    break;
                case 3:
                    lastResultRating = "Very Good";
                    break;
                default:
                    lastResultRating = "-";
                    break;
            }
            return lastResultRating;
        }

    }
}
