using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Data
{
    public class ValueList : NWBA.Base.Data
    {
        public class Schema
        {
            public class ValueList
            {
                public const string _TableName = "ValueList";

                public const string ValueMember = "value_member";
                public const string DisplayMember = "display_member";
            }
        }

        #region Constructors
        public ValueList(string sConnectionString) 
            : base(sConnectionString) 
        {}
        #endregion
        
        #region Public Methods
        public DataTable GetBookList()
        {
            DataSet dsResult = ExecuteDataSet(CommandType.StoredProcedure
                , "[dbo].[GetBookList]"
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.ValueList._TableName;
                return dsResult.Tables[Schema.ValueList._TableName];
            }

            return null;
        }
        #endregion
    }
}
