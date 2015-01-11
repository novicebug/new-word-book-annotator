using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Data
{
    public class Location: NWBA.Base.Data
    {
        public class Schema
        {
            public class MainInfo
            {
                public const string _TableName = "MainInfo";

                public const string BookId = "book_id";
                public const string WordId = "word_id";
                public const string PageLocation = "page_location";
            }
        }

        #region Constructors
        public Location(string sConnectionString) 
            : base(sConnectionString)
        { }
        #endregion

        #region Public Methods
        public DataTable Get(
            int nBookId
            , int nWordId
            )
        {
            DataSet dsResult;
            dsResult = ExecuteDataSet(
                CommandType.StoredProcedure
                , "[dbo].[GetLocation]"
                , new SqlCommandParameter("@nBookId", SqlDbType.BigInt, nBookId)
                , new SqlCommandParameter("@nWordId", SqlDbType.BigInt, nWordId)
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.MainInfo._TableName;
                return dsResult.Tables[0];
            }

            return null;
        }
               
        public void Save(
            int nBookId
            , int nWordId
            , string sPageLocation
            )
        {
            ExecuteNonQuery(
                CommandType.StoredProcedure
                , "[dbo].[SaveWordLocation]"
                , new SqlCommandParameter("@nBookId", SqlDbType.Int, nBookId)
                , new SqlCommandParameter("@nWordId", SqlDbType.Int, nWordId)
                , new SqlCommandParameter("@sPageLocation", SqlDbType.NVarChar, sPageLocation)
                );
        }
        #endregion
    }
}
