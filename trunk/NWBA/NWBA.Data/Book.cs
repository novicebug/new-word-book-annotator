using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Data
{
    public class Book: NWBA.Base.Data
    {
        public class Schema
        {
            public class MainInfo
            {
                public const string _TableName = "MainInfo";

                public const string BookId = "book_id";
                public const string Title = "title";
                public const string Version = "version";
            }

            public class WordIdList
            {
                public const string _TableName = "WordIdList";

                public const string WordId = "word_id";
            }

            public class MatchingWordIdList
            {
                public const string _TableName = "MatchingWordIdList";

                public const string WordId = "word_id";
            }
        }

        #region Constructors
        public Book(string sConnectionString) 
            : base(sConnectionString) 
        {}
        #endregion
        
        #region Public Methods
        public DataTable Get(int nBookId)
        {
            DataSet dsResult;
            dsResult = ExecuteDataSet(
                CommandType.StoredProcedure
                , "[dbo].[GetBook]"
                , new SqlCommandParameter("@nBookId", SqlDbType.Int, nBookId)
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.MainInfo._TableName;
                return dsResult.Tables[0];
            }

            return null;
        }

        public DataTable GetWordIdList(int nBookId)
        {
            DataSet dsResult;
            dsResult = ExecuteDataSet(
                CommandType.StoredProcedure
                , "[dbo].[GetBookWordIdList]"
                , new SqlCommandParameter("@nBookId", SqlDbType.Int, nBookId)
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.WordIdList._TableName;
                return dsResult.Tables[0];
            }

            return null;
        }

        public DataTable GetMatchingWordIdList(
            int nBookId
            , string sWordPart
            )
        {
            DataSet dsResult;
            dsResult = ExecuteDataSet(
                CommandType.StoredProcedure
                , "[dbo].[GetMatchingWordIdList]"
                , new SqlCommandParameter("@nBookId", SqlDbType.Int, nBookId)
                , new SqlCommandParameter("@sWordPart", SqlDbType.NVarChar, sWordPart)
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.MatchingWordIdList._TableName;
                return dsResult.Tables[0];
            }

            return null;
        }

        public void DeleteWord(
            int nBookId
            , int nWordId
            )
        {
            ExecuteNonQuery(
                CommandType.StoredProcedure
                , "[dbo].[DeleteWordFromBook]"
                , new SqlCommandParameter("@nBookId", SqlDbType.Int, nBookId)
                , new SqlCommandParameter("@nWordId", SqlDbType.Int, nWordId)
                );
        }
        #endregion
    }
}
