using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
    }
}
