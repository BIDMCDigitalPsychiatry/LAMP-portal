using System;
using System.Data;

namespace LAMP.Utility
{
    /// <summary>
    /// Class DataAccessHelper
    /// </summary>
    public static class DataAccessHelper
    {
        #region Public Methods
        /// <summary>
        /// Validate the value for type Boolean
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static bool GetBoolean(IDataReader dr, int i)
        {
            if (!dr.IsDBNull(i))
                return dr.GetBoolean(i);
            else
                return false;
        }

        /// <summary>
        /// Validate the value for type Boolean
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static bool GetBoolean(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return (bool)dr[fieldName];
            else
                return false;
        }

        /// <summary>
        /// Validate the value for type Double
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static double GetDouble(IDataReader dr, int i)
        {
            if (!dr.IsDBNull(i))
                return dr.GetDouble(i);
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type Double
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static double GetDouble(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return Convert.ToDouble(dr[fieldName]);
            else
                return 0;
        }


        /// <summary>
        /// Validate the value for type Float
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static float GetFloat(IDataReader dr, int i)
        {
            if (!dr.IsDBNull(i))
                return dr.GetFloat(i);
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type Float
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static float GetFloat(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return (float)dr[fieldName];
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type Decimal
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static decimal GetDecimal(IDataReader dr, int i)
        {
            if (!dr.IsDBNull(i))
                return dr.GetDecimal(i);
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type Decimal
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static decimal GetDecimal(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return (decimal)dr[fieldName];
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type DateTime
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static DateTime GetDateTime(IDataReader dr, int i)
        {
            if (!dr.IsDBNull(i))
                return dr.GetDateTime(i);
            else
                return DateTime.MinValue;
        }

        /// <summary>
        /// Validate the value for type DateTime
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static DateTime GetDateTime(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return (DateTime)dr[fieldName];
            else
                return DateTime.MinValue;
        }

        /// <summary>
        /// Validate the value for type Int16
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static Int16 GetInt16(IDataReader dr, int i)
        {
            if (!dr.IsDBNull(i))
                return dr.GetInt16(i);
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type Int16
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static short GetInt16(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return Convert.ToInt16(dr[fieldName].ToString());
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type Int32
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static int GetInt32(IDataReader dr, int i)
        {
            if (!dr.IsDBNull(i))
                return dr.GetInt32(i);
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type Int32
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static int GetInt32(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return Convert.ToInt32(dr[fieldName].ToString());
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type Int64
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static Int64 GetInt64(IDataReader dr, int i)
        {
            if (!dr.IsDBNull(i))
                return dr.GetInt64(i);
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type Int64
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>Numeric</returns>
        public static Int64 GetInt64(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return (Int64)dr[fieldName];
            else
                return 0;
        }

        /// <summary>
        /// Validate the value for type String
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>String value</returns>
        public static string GetString(IDataReader dr, int i)
        {
            if (!dr.IsDBNull(i))
                return dr.GetString(i);
            else
                return string.Empty;
        }

        /// <summary>
        /// Validate the value for type String
        /// </summary>
        /// <param name="dr">dr</param>
        /// <param name="i">i</param>
        /// <returns>String value</returns>
        public static string GetString(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return dr[fieldName].ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Validate the value for type DBNull or 0
        /// </summary>
        /// <param name="param">Object value</param>
        /// <returns>Status</returns>
        public static bool IsNull(object param)
        {
            if (param == DBNull.Value)
                return true;

            return false;
        }

        /// <summary>
        /// Validate the value and return int
        /// </summary>
        /// <param name="dataObject">Object value</param>
        /// <returns>Numeric</returns>
        public static int NullToInt(object dataObject)
        {
            if (dataObject is System.DBNull)
                return 0;
            else if (dataObject == null)
                return 0;
            else
                return Convert.ToInt32(dataObject);
        }

        /// <summary>
        /// Validate the value and return string
        /// </summary>
        /// <param name="value">Object value</param>
        /// <returns>String value</returns>
        public static String NullToEmptyString(Object value)
        {
            return (value != null && !(value is System.DBNull)) ? (String)value : "";
        }

        /// <summary>
        /// Validate the value and return double
        /// </summary>
        /// <param name="inObj">Object value</param>
        /// <returns>Numeric</returns>
        public static double NullToDouble(Object inObj)
        {
            return (inObj != null) ? Convert.ToDouble(inObj) : 0.00;
        }

        /// <summary>
        /// Validate the value and return byte
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static byte GetByte(IDataReader dr, string fieldName)
        {
            if (!(dr[fieldName] is System.DBNull))
                return (Byte)dr[fieldName];
            else
                return 0;
        }
        #endregion

    }
}
