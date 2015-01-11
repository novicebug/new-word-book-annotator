using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data;

namespace NWBA.Base
{
    public static class Extensions
    {
        public static string GetStringValue(this XElement value, string sElementName)
        {
            if (value.Element(sElementName) != null)
            {
                return value.Element(sElementName).Value;
            }

            return null;
        }

        public static bool HasTables(this DataSet value)
        {
            return value.Tables.Count > 0;
        }

        public static bool HasRows(this DataTable value)
        {
            return value.Rows.Count > 0;
        }
    }
}
