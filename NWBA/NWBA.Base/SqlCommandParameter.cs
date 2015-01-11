using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NWBA.Base
{
    public class SqlCommandParameter
    {
        #region Properties
        public ParameterDirection ParameterDirection { get; set; }       
        public string ParameterName { get; set; }
        public SqlDbType ParameterType { get; set; }
        public object ParameterValue { get; set; }
        #endregion

        #region Constructors
        public SqlCommandParameter(
            string sParameterName
            , SqlDbType oParameterType
            , object oParameterValue
            )
            : this(sParameterName, oParameterType, oParameterValue, ParameterDirection.Input)
        {
        }

        public SqlCommandParameter(
            string sParameterName
            , SqlDbType oParameterType
            , ParameterDirection oParameterDirection
            )
            : this(sParameterName, oParameterType, null, oParameterDirection)
        {
        }

        public SqlCommandParameter(
            string sParameterName
            , SqlDbType oParameterType
            , object oParameterValue
            , ParameterDirection oParameterDirection
            )
        {
            this.ParameterName = sParameterName;
            this.ParameterType = oParameterType;
            this.ParameterValue = oParameterValue;
            this.ParameterDirection = oParameterDirection;
        }
        #endregion
    }
}
