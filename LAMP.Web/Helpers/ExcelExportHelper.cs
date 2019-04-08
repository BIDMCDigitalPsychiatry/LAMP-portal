using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using LAMP.ViewModel;
using System.Reflection;
using LAMP.Utility;
using System.IO;


namespace LAMP.Web
{
    public class ExcelExportHelper
    {
        public static string ExcelContentType
        {
            get
            { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        }

        /// <summary>
        /// ListToDataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();

            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        /// <summary>
        /// ExportExcel
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="heading"></param>
        /// <param name="showSrNo"></param>
        /// <param name="columnsToTake"></param>
        /// <returns></returns>
        public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
        {

            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading));
                int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 3;

                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }


                // add the content into the Excel file  
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                using (ExcelRange Rng = workSheet.Cells[20, 1])
                {
                    Rng.Value = "Sample Picture";
                    Rng.Style.Font.Size = 16;
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Italic = true;
                }
                int rowIndex = 21;
                int colIndex = 2;
                int Height = 320;
                int Width = 200;
                Image img = Image.FromFile("C:\\Users\\Public\\Pictures\\Sample Pictures\\Tulips.jpg");
                ExcelPicture pic = workSheet.Drawings.AddPicture("Sample", img);
                pic.SetPosition(rowIndex, 0, colIndex, 0);
                pic.SetSize(Height, Width);
                workSheet.Protection.IsProtected = false;
                workSheet.Protection.AllowSelectLockedCells = false;

                // autofit width of cells with small content  
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                    if (maxLength < 150)
                    {
                        workSheet.Column(columnIndex).AutoFit();
                    }
                    columnIndex++;
                }

                // format header - bold, yellow on black  
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
                }

                // format cells - add borders  
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // removed ignored columns  
                for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                {
                    if (i == 0 && showSrNo)
                    {
                        continue;
                    }
                    if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                    {
                        workSheet.DeleteColumn(i + 1);
                    }
                }

                if (!String.IsNullOrEmpty(heading))
                {
                    //Merge cells
                    workSheet.Cells["A1:E1"].Merge = true;
                    workSheet.Cells["A1"].Value = heading;
                    workSheet.Cells["A1"].Style.Font.Size = 20;

                    workSheet.InsertColumn(1, 1);
                    workSheet.InsertRow(1, 1);
                    workSheet.Column(1).Width = 5;
                }

                result = package.GetAsByteArray();
            }

            return result;
        }

        /// <summary>
        /// ExportExcel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="Heading"></param>
        /// <param name="showSlno"></param>
        /// <param name="ColumnsToTake"></param>
        /// <returns></returns>
        public static byte[] ExportExcel<T>(List<T> data, string Heading = "", bool showSlno = false, params string[] ColumnsToTake)
        {
            return ExportExcel(ListToDataTable<T>(data), Heading, showSlno, ColumnsToTake);
        }

        /// <summary>
        /// ToDataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        /// <summary>
        /// ExportExcelViewModel
        /// </summary>
        /// <param name="data"></param>
        /// <param name="heading"></param>
        /// <param name="showSrNo"></param>
        /// <param name="ColumnsToTake"></param>
        /// <returns></returns>
        public static byte[] ExportExcelViewModel(UserDataExportViewModel data, string heading = "", bool showSrNo = false, params string[] ColumnsToTake)
        {
            byte[] result = null;
            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#64b5fe");
            string DateCellFormat = "mm/dd/yyyy";
            string DateTimeCellFormat = "mm/dd/yyyy hh:mm:ss";

            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    foreach (var user in data.UserDetails)
                    {
                        string sheetName = user.FirstName + "_" + user.StudyId;
                        int rowCount = 1;
                        ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0}", sheetName));
                        workSheet.Column(1).Width = 60;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 30;
                        workSheet.Column(6).Width = 30;

                        using (ExcelRange r = workSheet.Cells[1, 1, 1, 6])
                        {
                            r.Value = "USER DETAILS";
                            r.Merge = true;
                            r.Style.Font.Size = 12;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(colFromHex);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            r.Style.Font.Bold = true;
                        }

                        rowCount++;
                        using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                        {
                            r.Style.Font.Bold = true;
                        }
                        workSheet.Cells["A" + rowCount].Value = "STUDY ID";
                        workSheet.Cells["B" + rowCount].Value = "NAME";
                        workSheet.Cells["C" + rowCount].Value = "EMAIL";
                        workSheet.Cells["D" + rowCount].Value = "PHONE";
                        workSheet.Cells["E" + rowCount].Value = "REGISTERED ON";
                        workSheet.Cells["F" + rowCount].Value = "LAST SURVEY DATE";


                        rowCount++;

                        workSheet.Cells["E" + rowCount].Style.Numberformat.Format = DateCellFormat;
                        workSheet.Cells["F" + rowCount].Style.Numberformat.Format = DateCellFormat;

                        workSheet.Cells["E" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells["F" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        workSheet.Cells["A" + rowCount].Value = user.StudyId;
                        workSheet.Cells["B" + rowCount].Value = user.FirstName + " " + user.LastName;
                        workSheet.Cells["C" + rowCount].Value = user.Email;
                        workSheet.Cells["D" + rowCount].Value = user.Phone;
                        workSheet.Cells["E" + rowCount].Value = user.RegisteredOn;
                        workSheet.Cells["F" + rowCount].Value = user.LastSurveyDate;

                        rowCount++;
                        rowCount++;

                        using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                        {
                            r.Value = "SYMPTOM SURVEYS";
                            r.Merge = true;
                            r.Style.Font.Size = 12;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(colFromHex);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            r.Style.Font.Bold = true;
                        }

                        rowCount++;
                        using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                        {
                            r.Style.Font.Bold = true;
                        }
                        workSheet.Cells["A" + rowCount].Value = "LAST SURVEY";
                        workSheet.Cells["B" + rowCount].Value = "OVERALL RATING";
                        workSheet.Cells["C" + rowCount].Value = "POINTS";

                        rowCount++;
                        workSheet.Cells["A" + rowCount].Style.Numberformat.Format = DateTimeCellFormat;
                        workSheet.Cells["A" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells["B" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells["C" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells["A" + rowCount].Value = user.LastSurveyDateString;
                        workSheet.Cells["B" + rowCount].Value = data.SurveyHeader.Where(u => u.UserId == user.UserID).Count() + " Survey(s)";
                        workSheet.Cells["C" + rowCount].Value = user.Points;

                        rowCount++;
                        rowCount++;

                        /************Surveys Listing***********/

                        Color SurveyNameHeaderColor = System.Drawing.ColorTranslator.FromHtml("#9ec8ef");
                        int surveyCount = 0;
                        string headerText = string.Empty;
                        foreach (var survey in data.SurveyHeader.Where(s => s.UserId == user.UserID).ToList())
                        {
                            surveyCount++;
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                            {
                                headerText = string.Empty;
                                headerText = surveyCount + ".  " + survey.SurveyName + "    Survey Date : " + survey.CreatedOn_string + "  | Last Time Taken (Hours) : " + survey.Duration;
                                if (survey.IsDistraction != null)
                                {
                                    headerText = headerText + "  | Is Distraction : " + (survey.IsDistraction == true ? " Yes" : " No");
                                }
                                if (survey.IsNotificationGame != null)
                                {
                                    headerText = headerText + "  | Is NotificationGame : " + (survey.IsNotificationGame == true ? " Yes" : " No");
                                }
                                if (survey.SpinWheelScore != null && survey.SpinWheelScore.Length > 0)
                                {
                                    headerText = headerText + "  | SpinWheel Score : " + survey.SpinWheelScore;
                                }
                                r.Value = headerText;
                                r.Merge = true;
                                r.Style.Font.Size = 12;
                                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                r.Style.Fill.BackgroundColor.SetColor(SurveyNameHeaderColor);
                                r.Style.Font.Bold = true;
                            }

                            rowCount++;
                            var surveyResults = data.Surveys.Where(s => s.UserId == user.UserID && s.SurveyResultID == survey.SurveyResultID).Select(x => new { x.Question, x.CorrectAnswer, x.Time_Taken, x.ClickRange }).ToList();
                            workSheet.Cells["A" + rowCount].LoadFromCollection(surveyResults, true);
                            workSheet.Cells["A" + rowCount].Value = "QUESTIONS";
                            workSheet.Cells["B" + rowCount].Value = "ANSWERS";
                            workSheet.Cells["C" + rowCount].Value = "TIME TAKEN";
                            workSheet.Cells["D" + rowCount].Value = "CLICK RANGE";

                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                            {
                                r.Style.Font.Bold = true;
                            }
                            rowCount = rowCount + surveyResults.Count;
                            rowCount++;
                        }
                        rowCount++;

                        /************Cogition Tests***********/
                        using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                        {
                            r.Value = "COGNITION TESTS";
                            r.Merge = true;
                            r.Style.Font.Size = 12;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(colFromHex);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            r.Style.Font.Bold = true;
                        }                        

                        rowCount++;
                        using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                        {
                            r.Style.Font.Bold = true;
                        }
                        workSheet.Cells["A" + rowCount].Value = "LAST RESULT";
                        workSheet.Cells["B" + rowCount].Value = "OVERALL RATING";


                        rowCount++;
                        workSheet.Cells["A" + rowCount].Style.Numberformat.Format = DateTimeCellFormat;
                        workSheet.Cells["A" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells["B" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells["A" + rowCount].Value = data.CognitionOverallPoints.Where(u => u.UserId == user.UserID).Select(x => (x.LastResultString)).FirstOrDefault();//doubt
                        workSheet.Cells["B" + rowCount].Value = data.CognitionOverallPoints.Where(u => u.UserId == user.UserID).Select(x => (x.OverallRating)).FirstOrDefault() + " Game(s)";

                        rowCount++;
                        rowCount++;

                        /************Cogition Listing***********/
                        int cognitionCount = 0;
                        foreach (var ctest in data.CognitionTestHeader.Where(s => s.UserId == user.UserID).ToList())
                        {
                            cognitionCount++;
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                            {
                                headerText = string.Empty;
                                if (ctest.CTestID == 8)
                                {
                                    headerText = cognitionCount + ". " + ctest.CTestName + "  " + (ctest.TotalGames / 4) + " Game(s) | " + ctest.TotalPoints + " points | Last Cognition Date :  " + ctest.LastTestDateString + " | Last Time Taken (Hours) :  " + ctest.Duration_string;
                                }
                                else
                                {
                                    headerText = cognitionCount + ". " + ctest.CTestName + "  " + ctest.TotalGames + " Game(s) | " + ctest.TotalPoints + " points | Last Cognition Date :  " + ctest.LastTestDateString + " | Last Time Taken (Hours) :  " + ctest.Duration_string;
                                }
                                r.Value = headerText;
                                r.Merge = true;
                                r.Style.Font.Size = 12;
                                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                r.Style.Fill.BackgroundColor.SetColor(SurveyNameHeaderColor);
                                r.Style.Font.Bold = true;
                            }

                            rowCount++;
                            //n-back Test
                            if (ctest.CTestID == 1)
                            {
                                var nbackResults = data.NBackCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalQuestions, x.CorrectAnswers, x.WrongAnswers, x.StartTimeString, x.Duration_String, x.Version }).ToList();
                                if (nbackResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(nbackResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL NO OF QUESTIONS";
                                    workSheet.Cells["B" + rowCount].Value = "CORRECT ANSWERS";
                                    workSheet.Cells["C" + rowCount].Value = "WRONG ANSWERS";
                                    workSheet.Cells["D" + rowCount].Value = "START TIME";
                                    workSheet.Cells["E" + rowCount].Value = "DURATION";
                                    workSheet.Cells["F" + rowCount].Value = "VERSION";//ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6]
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + nbackResults.Count, 6])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }

                                    rowCount = rowCount + nbackResults.Count;
                                    rowCount++;
                                }
                            }
                            //trails-b Test
                            if (ctest.CTestID == 2)
                            {
                                var trialsBResults = data.TrailsBCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalAttempts, x.StartTimeString, x.Duration_String, x.DetailString }).ToList();
                                if (trialsBResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(trialsBResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL ATTEMPTS";
                                    workSheet.Cells["B" + rowCount].Value = "START TIME";
                                    workSheet.Cells["C" + rowCount].Value = "DURATION";
                                    workSheet.Cells["D" + rowCount].Value = "DETAILS";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + trialsBResults.Count, 4])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }

                                    rowCount = rowCount + trialsBResults.Count;
                                    rowCount++;
                                }
                            }

                            //Spatial Span Forward
                            if (ctest.CTestID == 3)
                            {
                                var _spatialResults = data.SpatialCTestForwardList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.CorrectAnswers, x.WrongAnswers, x.StartTimeString, x.Duration_String }).ToList();
                                if (_spatialResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_spatialResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "CORRECT ANSWERS";
                                    workSheet.Cells["B" + rowCount].Value = "WRONG ANSWERS";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _spatialResults.Count, 4])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _spatialResults.Count;
                                    rowCount++;
                                }
                            }
                            
                            // Spatial Span Backward
                            if (ctest.CTestID == 4)
                            {
                                var _spatialResults = data.SpatialCTestBackwardList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.CorrectAnswers, x.WrongAnswers, x.StartTimeString, x.Duration_String }).ToList();
                                if (_spatialResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_spatialResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "CORRECT ANSWERS";
                                    workSheet.Cells["B" + rowCount].Value = "WRONG ANSWERS";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount + 1, 1, rowCount + _spatialResults.Count, 4])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _spatialResults.Count;
                                    rowCount++;
                                }
                            }

                            // Simple Memory Test
                            if (ctest.CTestID == 5)
                            {
                                var _simpleMemoryResults = data.SimpleMemoryCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalQuestions, x.CorrectAnswers, x.WrongAnswers, x.StartTimeString, x.Duration_String, x.Version }).ToList();
                                if (_simpleMemoryResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_simpleMemoryResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL NO OF QUESTIONS";
                                    workSheet.Cells["B" + rowCount].Value = "CORRECT ANSWERS";
                                    workSheet.Cells["C" + rowCount].Value = "WRONG ANSWERS";
                                    workSheet.Cells["D" + rowCount].Value = "START TIME";
                                    workSheet.Cells["E" + rowCount].Value = "DURATION";
                                    workSheet.Cells["F" + rowCount].Value = "VERSION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount + 1, 1, rowCount + _simpleMemoryResults.Count, 6])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _simpleMemoryResults.Count;
                                    rowCount++;
                                }
                            }

                            // Serial 7s
                            if (ctest.CTestID == 6)
                            {
                                var _serial7CTestResults = data.Serial7CTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalQuestions, x.TotalAttempts, x.StartTimeString, x.Duration_String, x.Version }).ToList();
                                if (_serial7CTestResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_serial7CTestResults, true);//version add
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL NO OF QUESTIONS";
                                    workSheet.Cells["B" + rowCount].Value = "TOTAL ATTEMPTS";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    workSheet.Cells["E" + rowCount].Value = "VERSION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount + 1, 1, rowCount + _serial7CTestResults.Count, 5])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _serial7CTestResults.Count;
                                    rowCount++;
                                }
                            }                            

                            if (ctest.CTestID == 8)
                            {

                                var _threeDFigureResults = data.ThreeDFigureCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.ActualImageFileName, x.DrawnFigFileName, x.StartTimeString, x.Duration_String }).ToList();
                                if (_threeDFigureResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].Value = "ACTUAL IMAGE";
                                    workSheet.Cells["B" + rowCount].Value = "DRAWN IMAGE";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount + 1, 3, rowCount + _threeDFigureResults.Count, 6])
                                    {
                                        r.Style.Numberformat.Format = DateTimeCellFormat;
                                    }
                                    rowCount++;
                                    for (int i = 0; i < _threeDFigureResults.Count; i++)
                                    {
                                        // Actual Image
                                        int rowIndex = rowCount - 1;
                                        int colIndex = 0;
                                        int height = 50;
                                        int width = 50;
                                        if (File.Exists(HttpContext.Current.Server.MapPath("~/Games/Base3DFigures/" + _threeDFigureResults[i].ActualImageFileName)))
                                        {
                                            // Actual Image
                                            Image actualImage = Image.FromFile(HttpContext.Current.Server.MapPath("~/Games/Base3DFigures/") + _threeDFigureResults[i].ActualImageFileName);
                                            ExcelPicture actualPicture = workSheet.Drawings.AddPicture("actualImage_" + i, actualImage);
                                            actualPicture.SetPosition(rowIndex, 5, colIndex, 0);
                                            actualPicture.SetSize(height, width);
                                            workSheet.Row(rowCount).Height = height;
                                        }
                                        if (File.Exists(HttpContext.Current.Server.MapPath("~/Games/User3DFigures/" + _threeDFigureResults[i].DrawnFigFileName)))
                                        {
                                            // Drawn Image
                                            colIndex = 1;
                                            Image drawnImage = Image.FromFile(HttpContext.Current.Server.MapPath("~/Games/User3DFigures/") + _threeDFigureResults[i].DrawnFigFileName);
                                            ExcelPicture drawnPicture = workSheet.Drawings.AddPicture("drawnImage_" + i, drawnImage);
                                            drawnPicture.SetPosition(rowIndex, 5, colIndex, 0);
                                            drawnPicture.SetSize(height, width);
                                        }
                                        workSheet.Cells["C" + rowCount].Value = _threeDFigureResults[i].StartTimeString;
                                        workSheet.Cells["D" + rowCount].Value = _threeDFigureResults[i].Duration_String;
                                        rowCount++;
                                    }

                                }
                            }

                            // Visual Association task
                            if (ctest.CTestID == 9)
                            {
                                var _visualAssociationtaskResults = data.VisualAssociationCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalQuestions, x.TotalAttempts, x.StartTimeString, x.Duration_String, x.Version }).ToList();
                                if (_visualAssociationtaskResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_visualAssociationtaskResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL NO OF QUESTIONS";
                                    workSheet.Cells["B" + rowCount].Value = "TOTAL ATTEMPTS";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    workSheet.Cells["E" + rowCount].Value = "VERSION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _visualAssociationtaskResults.Count, 5])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _visualAssociationtaskResults.Count;
                                    rowCount++;
                                }
                            }

                            // Digital Span Forward
                            if (ctest.CTestID == 10)
                            {
                                var _digitSpanCTestForwardResults = data.DigitSpanCTestForwardList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.CorrectAnswers, x.WrongAnswers, x.StartTimeString, x.Duration_String }).ToList();
                                if (_digitSpanCTestForwardResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_digitSpanCTestForwardResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "CORRECT ANSWERS";
                                    workSheet.Cells["B" + rowCount].Value = "INCORRECT ANSWERS";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _digitSpanCTestForwardResults.Count, 4])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _digitSpanCTestForwardResults.Count;
                                    rowCount++;
                                }
                            }

                            // Cats and Dogs(New)
                            if (ctest.CTestID == 11)
                            {
                                var _catAndDogNewResults = data.CatAndDogNewCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.CorrectAnswers, x.WrongAnswers, x.StartTimeString, x.Duration_String }).ToList();
                                if (_catAndDogNewResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_catAndDogNewResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "CORRECT ANSWERS";
                                    workSheet.Cells["B" + rowCount].Value = "INCORRECT ANSWERS";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _catAndDogNewResults.Count, 4])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _catAndDogNewResults.Count;
                                    rowCount++;
                                }
                            }

                            // Temporal Order
                            if (ctest.CTestID == 12)
                            {
                                var _temporalOrderCTestResults = data.TemporalOrderCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.CorrectAnswers, x.WrongAnswers, x.StartTimeString, x.Duration_String, x.Version }).ToList();
                                if (_temporalOrderCTestResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_temporalOrderCTestResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "CORRECT ANSWERS";
                                    workSheet.Cells["B" + rowCount].Value = "INCORRECT ANSWERS";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    workSheet.Cells["E" + rowCount].Value = "VERSION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _temporalOrderCTestResults.Count, 5])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _temporalOrderCTestResults.Count;
                                    rowCount++;
                                }
                            }

                            // Digital Span Backward
                            if (ctest.CTestID == 13)
                            {
                                var _digitSpanCTestBackwardResults = data.DigitSpanCTestBackwardList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.CorrectAnswers, x.WrongAnswers, x.StartTimeString, x.Duration_String }).ToList();
                                if (_digitSpanCTestBackwardResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_digitSpanCTestBackwardResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "CORRECT ANSWERS";
                                    workSheet.Cells["B" + rowCount].Value = "INCORRECT ANSWERS";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _digitSpanCTestBackwardResults.Count, 4])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _digitSpanCTestBackwardResults.Count;
                                    rowCount++;
                                }
                            }

                            // n-back Test (New)
                            if (ctest.CTestID == 14)
                            {
                                var _nBackNewCTestResults = data.NBackNewCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalQuestions, x.CorrectAnswers, x.WrongAnswers, x.StartTimeString, x.Duration_String }).ToList();
                                if (_nBackNewCTestResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_nBackNewCTestResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL NO OF QUESTIONS";
                                    workSheet.Cells["B" + rowCount].Value = "CORRECT ANSWERS";
                                    workSheet.Cells["C" + rowCount].Value = "WRONG ANSWERS";
                                    workSheet.Cells["D" + rowCount].Value = "START TIME";
                                    workSheet.Cells["E" + rowCount].Value = "DURATION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _nBackNewCTestResults.Count, 5])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _nBackNewCTestResults.Count;
                                    rowCount++;
                                }
                            }

                            // trails-b Test(New)
                            if (ctest.CTestID == 15)
                            {
                                var _trailsBNewCTestResults = data.TrailsBNewCTest.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalAttempts, x.StartTimeString, x.Duration_String, x.DetailString, x.Version }).ToList();
                                if (_trailsBNewCTestResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_trailsBNewCTestResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL ATTEMPTS";
                                    workSheet.Cells["B" + rowCount].Value = "START TIME";
                                    workSheet.Cells["C" + rowCount].Value = "DURATION";
                                    workSheet.Cells["D" + rowCount].Value = "DETAIL";
                                    workSheet.Cells["E" + rowCount].Value = "VERSION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _trailsBNewCTestResults.Count, 5])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _trailsBNewCTestResults.Count;
                                    rowCount++;
                                }
                            }

                            // trails-b Test(DotTouch)
                            if (ctest.CTestID == 16)
                            {
                                var _trailsBDotTouchResults = data.TrailsBDotTouchCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalAttempts, x.StartTimeString, x.Duration_String, x.DetailString }).ToList();
                                if (_trailsBDotTouchResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_trailsBDotTouchResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL ATTEMPTS";
                                    workSheet.Cells["B" + rowCount].Value = "START TIME";
                                    workSheet.Cells["C" + rowCount].Value = "DURATION";
                                    workSheet.Cells["D" + rowCount].Value = "DETAILS";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount + 1, 2, rowCount + _trailsBDotTouchResults.Count, 6])
                                    {
                                        r.Style.Numberformat.Format = DateTimeCellFormat;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _trailsBDotTouchResults.Count, 4])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _trailsBDotTouchResults.Count;
                                    rowCount++;
                                }
                            }

                            // Jewels Trails A
                            if (ctest.CTestID == 17)
                            {
                                var _jewelsTrailsAResults = data.JewelsTrailsACTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalAttempts, x.StartTimeString, x.Duration_String, x.TotalBonusCollected, x.TotalJewelsCollected, x.DetailString }).ToList();//bonus and jewels
                                if (_jewelsTrailsAResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_jewelsTrailsAResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL ATTEMPTS";
                                    workSheet.Cells["B" + rowCount].Value = "START TIME";
                                    workSheet.Cells["C" + rowCount].Value = "DURATION";
                                    workSheet.Cells["D" + rowCount].Value = "BONUS COLLECTED";
                                    workSheet.Cells["E" + rowCount].Value = "JEWELS COLLECTED";
                                    workSheet.Cells["F" + rowCount].Value = "DETAILS";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount + 1, 2, rowCount + _jewelsTrailsAResults.Count, 6])
                                    {
                                        r.Style.Numberformat.Format = DateTimeCellFormat;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _jewelsTrailsAResults.Count, 6])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _jewelsTrailsAResults.Count;
                                    rowCount++;
                                }
                            }

                            //Jewels Trails B
                            if (ctest.CTestID == 18)
                            {
                                var _jewelsTrailsBResults = data.JewelsTrailsBCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.TotalAttempts, x.StartTimeString, x.Duration_String, x.TotalBonusCollected, x.TotalJewelsCollected, x.DetailString }).ToList();
                                if (_jewelsTrailsBResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_jewelsTrailsBResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "TOTAL ATTEMPTS";
                                    workSheet.Cells["B" + rowCount].Value = "START TIME";
                                    workSheet.Cells["C" + rowCount].Value = "DURATION";
                                    workSheet.Cells["D" + rowCount].Value = "BONUS COLLECTED";
                                    workSheet.Cells["E" + rowCount].Value = "JEWELS COLLECTED";
                                    workSheet.Cells["F" + rowCount].Value = "DETAILS";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount + 1, 2, rowCount + _jewelsTrailsBResults.Count, 6])
                                    {
                                        r.Style.Numberformat.Format = DateTimeCellFormat;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _jewelsTrailsBResults.Count, 6])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _jewelsTrailsBResults.Count;
                                    rowCount++;
                                }
                            }

                            // Scratch Image                        
                            if (ctest.CTestID == 19)
                            {

                                var _scratchImageResults = data.ScratchImageCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.FileName, x.DrawnFigFileName, x.StartTimeString, x.Duration_String }).ToList();
                                if (_scratchImageResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].Value = "ACTUAL IMAGE";
                                    workSheet.Cells["B" + rowCount].Value = "DRAWN IMAGE";
                                    workSheet.Cells["C" + rowCount].Value = "START TIME";
                                    workSheet.Cells["D" + rowCount].Value = "DURATION";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount + 1, 3, rowCount + _scratchImageResults.Count, 6])
                                    {
                                        r.Style.Numberformat.Format = DateTimeCellFormat;
                                    }
                                    rowCount++;
                                    for (int i = 0; i < _scratchImageResults.Count; i++)
                                    {
                                        // Actual Image
                                        int rowIndex = rowCount - 1;
                                        int colIndex = 0;
                                        int height = 50;
                                        int width = 50;
                                        if (File.Exists(HttpContext.Current.Server.MapPath("~/Games/BaseScratchImages/" + _scratchImageResults[i].FileName)))
                                        {
                                            // Actual Image
                                            Image actualImage = Image.FromFile(HttpContext.Current.Server.MapPath("~/Games/BaseScratchImages/") + _scratchImageResults[i].FileName);
                                            ExcelPicture actualPicture = workSheet.Drawings.AddPicture("actualScratchImage_" + i, actualImage);
                                            actualPicture.SetPosition(rowIndex, 5, colIndex, 0);
                                            actualPicture.SetSize(height, width);
                                            workSheet.Row(rowCount).Height = height;
                                        }
                                        if (File.Exists(HttpContext.Current.Server.MapPath("~/Games/UserScratchImages/" + _scratchImageResults[i].DrawnFigFileName)))
                                        {
                                            // Drawn Image
                                            colIndex = 1;
                                            Image drawnImage = Image.FromFile(HttpContext.Current.Server.MapPath("~/Games/UserScratchImages/") + _scratchImageResults[i].DrawnFigFileName);
                                            ExcelPicture drawnPicture = workSheet.Drawings.AddPicture("drawnScratchImage_" + i, drawnImage);
                                            drawnPicture.SetPosition(rowIndex, 5, colIndex, 0);
                                            drawnPicture.SetSize(height, width);
                                        }
                                        workSheet.Cells["C" + rowCount].Value = _scratchImageResults[i].StartTimeString;
                                        workSheet.Cells["D" + rowCount].Value = _scratchImageResults[i].Duration_String;
                                        rowCount++;
                                    }

                                }
                            }

                            // Spin Wheel game
                            if (ctest.CTestID == 20)
                            {
                                var _spinWheelResults = data.SpinWheelCTestList.Where(s => s.UserId == user.UserID && s.CTestID == ctest.CTestID).Select(x => new { x.StartTime, x.CollectedStars }).ToList();
                                if (_spinWheelResults.Count > 0)
                                {
                                    workSheet.Cells["A" + rowCount].LoadFromCollection(_spinWheelResults, true);
                                    workSheet.Cells["A" + rowCount].Value = "START TIME";
                                    workSheet.Cells["B" + rowCount].Value = "COLLECTED STARS";
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                                    {
                                        r.Style.Font.Bold = true;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount + 1, 2, rowCount + _spinWheelResults.Count, 6])
                                    {
                                        r.Style.Numberformat.Format = DateTimeCellFormat;
                                    }
                                    using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _spinWheelResults.Count, 2])
                                    {
                                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    }
                                    rowCount = rowCount + _spinWheelResults.Count;
                                    rowCount++;
                                }
                            }                            
                        }
                        rowCount++;
                        rowCount++;
                        /************Help Call HistoryListing***********/
                        using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                        {
                            r.Value = "CALL HISTORY";
                            r.Merge = true;
                            r.Style.Font.Size = 12;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(colFromHex);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            r.Style.Font.Bold = true;
                        }
                        rowCount++;

                        var _CallHistory = data.CallHistory.Where(s => s.UserId == user.UserID).Select(x => new { x.CallType, x.CalledNumber, x.CallDateTimeString, x.CallDuraion }).ToList();
                        if (_CallHistory.Count > 0)
                        {
                            workSheet.Cells["A" + rowCount].LoadFromCollection(_CallHistory, true);
                            workSheet.Cells["A" + rowCount].Value = "CALL TYPE";
                            workSheet.Cells["B" + rowCount].Value = "HELP CALL HISTORY";
                            workSheet.Cells["C" + rowCount].Value = "DATE AND TIME";
                            workSheet.Cells["D" + rowCount].Value = "DURATION";
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                            {
                                r.Style.Font.Bold = true;
                            }
                            using (ExcelRange r = workSheet.Cells[rowCount + 1, 3, rowCount + _CallHistory.Count, 6])
                            {
                                r.Style.Numberformat.Format = DateTimeCellFormat;
                            }
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _CallHistory.Count, 4])
                            {
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }
                            rowCount = rowCount + _CallHistory.Count;

                        }
                        else
                        {
                            workSheet.Cells["A" + rowCount].Value = "No Results";
                        }

                        rowCount++;
                        /************ Locations ***********/
                        using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                        {
                            r.Value = "LOCATION";
                            r.Merge = true;
                            r.Style.Font.Size = 12;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(colFromHex);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            r.Style.Font.Bold = true;
                        }

                        rowCount++;

                        var _locations = data.Locations.Where(s => s.UserId == user.UserID).Select(x => new { x.Address, x.CreatedOnString, x.Latitude, x.Longitude }).ToList();
                        if (_locations.Count > 0)
                        {
                            workSheet.Cells["A" + rowCount].LoadFromCollection(_locations, true);
                            workSheet.Cells["A" + rowCount].Value = "ACTUAL ADDRESS";
                            workSheet.Cells["B" + rowCount].Value = "DATE & TIME";
                            workSheet.Cells["C" + rowCount].Value = "LATITUDE";
                            workSheet.Cells["D" + rowCount].Value = "LONGITUDE";
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                            {
                                r.Style.Font.Bold = true;
                            }
                            using (ExcelRange r = workSheet.Cells[rowCount + 1, 2, rowCount + _locations.Count, 6])
                            {
                                r.Style.Numberformat.Format = DateTimeCellFormat;
                            }
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _locations.Count, 2])
                            {
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }
                            rowCount = rowCount + _locations.Count;

                        }
                        else
                        {
                            workSheet.Cells["A" + rowCount].Value = "No Results";
                        }

                        rowCount++;

                        /************ Environment ***********/
                        using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                        {
                            r.Value = "ENVIRONMENT";
                            r.Merge = true;
                            r.Style.Font.Size = 12;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(colFromHex);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            r.Style.Font.Bold = true;
                        }

                        rowCount++;

                        var _environment = data.Environment.Where(s => s.UserId == user.UserID).Select(x => new { x.LocationName, x.Address, x.CreatedOnString }).ToList();
                        if (_environment.Count > 0)
                        {
                            workSheet.Cells["A" + rowCount].LoadFromCollection(_environment, true);
                            workSheet.Cells["A" + rowCount].Value = "LOCATION";
                            workSheet.Cells["B" + rowCount].Value = "ACTUAL ADDRESS";
                            workSheet.Cells["C" + rowCount].Value = "DATE & TIME";
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 6])
                            {
                                r.Style.Font.Bold = true;
                            }
                            using (ExcelRange r = workSheet.Cells[rowCount + 1, 3, rowCount + _environment.Count, 6])
                            {
                                r.Style.Numberformat.Format = DateTimeCellFormat;
                            }
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _environment.Count, 3])
                            {
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }
                            rowCount = rowCount + _environment.Count;
                        }
                        else
                        {
                            workSheet.Cells["A" + rowCount].Value = "No Results";
                        }
                        rowCount++;

                        /************ Health Kit ***********/
                        var _healthKitDetails = data.HealthKitData.Where(s => s.UserId == user.UserID).Select(x => new { x.CreatedOnString, x.Height, x.Weight, x.HeartRate, x.BloodPressure, x.RespiratoryRate, x.Sleep, x.Steps, x.FlightClimbed, x.Segment, x.Distance }).ToList();
                        int toRange = _healthKitDetails.Count > 0 ? 11 : 6;
                        using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, toRange])
                        {
                            r.Value = "HEALTH KIT DATA";
                            r.Merge = true;
                            r.Style.Font.Size = 12;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(colFromHex);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            r.Style.Font.Bold = true;
                        }

                        rowCount++;
                        var _healthKitData = data.HealthKitHeader.Where(s => s.UserId == user.UserID).Select(x => new { x.BloodType, x.DOBString, x.Sex }).SingleOrDefault();
                        if (_healthKitData != null)
                        {
                            string dob = "";
                            if (_healthKitData.DOBString == "")
                            {
                                dob = "Not set";
                            }
                            else
                            {
                                dob = _healthKitData.DOBString;
                            }
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 11])
                            {
                                r.Value = "Date of Birth: " + dob + " | Sex: " + _healthKitData.Sex + " | Blood Group: " + _healthKitData.BloodType;
                                r.Merge = true;
                                r.Style.Font.Size = 12;
                                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                r.Style.Fill.BackgroundColor.SetColor(SurveyNameHeaderColor);
                                r.Style.Font.Bold = true;
                            }
                            rowCount++;
                        }

                        
                        if (_healthKitDetails.Count > 0)
                        {
                            workSheet.Cells["A" + rowCount].LoadFromCollection(_healthKitDetails, true);
                            workSheet.Cells["A" + rowCount].Value = "DATE";
                            workSheet.Cells["B" + rowCount].Value = "TIME | HEIGHT";
                            workSheet.Cells["C" + rowCount].Value = "TIME | WEIGHT";
                            workSheet.Cells["D" + rowCount].Value = "TIME | HEART RATE";
                            workSheet.Cells["E" + rowCount].Value = "TIME | BLOOD PRESSURE";
                            workSheet.Cells["F" + rowCount].Value = "TIME | RESPIRATORY RATE";
                            workSheet.Cells["G" + rowCount].Value = "TIME | SLEEP";
                            workSheet.Cells["H" + rowCount].Value = "TIME | STEPS";
                            workSheet.Cells["I" + rowCount].Value = "TIME | FLIGHT CLIMBED";
                            workSheet.Cells["J" + rowCount].Value = "TIME | SEGMENT";
                            workSheet.Cells["K" + rowCount].Value = "TIME | DISTANCE";
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount, 11])
                            {
                                r.Style.Font.Bold = true;
                            }
                            using (ExcelRange r = workSheet.Cells[rowCount + 1, 1, rowCount + _healthKitDetails.Count, 11])
                            {
                                r.Style.Numberformat.Format = DateTimeCellFormat;
                            }
                            using (ExcelRange r = workSheet.Cells[rowCount, 1, rowCount + _healthKitDetails.Count, 11])
                            {
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }
                            rowCount = rowCount + _healthKitDetails.Count;
                        }
                        else
                        {
                            workSheet.Cells["A" + rowCount].Value = "No Results";
                        }
                    }
                    result = package.GetAsByteArray();
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("ExcelExportHelper - ExportExcelViewModel: " + ex);
                return null;
            }
        }

    }
}