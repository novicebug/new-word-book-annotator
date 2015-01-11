using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWBA.Base
{
    public class DBConverter
    {
        #region DB Value to .NET
        public static int DBValueToInt(object oValue)
        {
            if (oValue == null
                || oValue == DBNull.Value
                )
            {
                return 0;
            }

            return (int)oValue;
        }
        public static int? DBValueToIntNullable(object oValue)
        {
            if (oValue == null
                || oValue == DBNull.Value
                )
            {
                return null;
            }

            return (int)oValue;
        }
        
        public static long DBValueToLong(object oValue)
        {
            if (oValue == null
                || oValue == DBNull.Value
                )
            {
                return 0;
            }

            return (long)oValue;
        }
        public static long? DBValueToLongNullable(object oValue)
        {
            if (oValue == null
                || oValue == DBNull.Value
                )
            {
                return null;
            }

            return (long)oValue;
        }

        public static string DBValueToString(object oValue)
        {
            if (oValue == null
                || oValue == DBNull.Value
                )
            {
                return "";
            }

            return (string)oValue;
        }
        public static string DBValueToStringNullable(object oValue)
        {
            if (oValue == null
                || oValue == DBNull.Value
                )
            {
                return null;
            }

            return (string)oValue;
        }
        #endregion

        #region .NET to DB Value
        public static object IntToDBValue(int? nData)
        {
            if (nData.HasValue)
            {
                return nData.Value;
            }
            else
            {
                return DBNull.Value;
            }
        }

        public static object LongToDBValue(long? nData)
        {
            if (nData.HasValue)
            {
                return nData.Value;
            }
            else
            {
                return DBNull.Value;
            }
        }

        public static object StringToDBValue(string sData)
        {
            if (sData != null)
            {
                return sData;
            }
            else
            {
                return DBNull.Value;
            }
        }
        #endregion
    }
}
