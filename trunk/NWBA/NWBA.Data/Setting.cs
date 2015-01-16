using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Data
{
    public class Setting: NWBA.Base.Data
    {
        public class Schema
        {
            public class MainInfo
            {
                public const string _TableName = "MainInfo";

                public const string LastBookId = "last_book_id";
                public const string FontSize = "font_size";
            }
        }

        #region Constructors
        public Setting(string sConnectionString) 
            : base(sConnectionString)
        { }
        #endregion

        #region Public Methods
        public DataTable Get()
        {
            DataSet dsResult;
            dsResult = ExecuteDataSet(
                CommandType.StoredProcedure
                , "[dbo].[GetSetting]"
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.MainInfo._TableName;
                return dsResult.Tables[0];
            }

            return null;
        }
               
        public void Save(
            int nLastBookId
            , double nFontSize
            )
        {
            ExecuteNonQuery(
                CommandType.StoredProcedure
                , "[dbo].[SaveSetting]"
                , new SqlCommandParameter("@nLastBookId", SqlDbType.Int, nLastBookId)
                , new SqlCommandParameter("@nFontSize", SqlDbType.Decimal, nFontSize)
                );
        }
        #endregion
    }
}
