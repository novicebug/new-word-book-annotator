using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Data
{
    public class Word: NWBA.Base.Data
    {
        public class Schema
        {
            public class Word
            {
                public const string _TableName = "Word";

                public const string WordId = "word_id";
                public const string Value = "value";
                public const string Pronunciation = "pronunciation";
                public const string Translation = "translation";
            }
            
            public class WordBookIdList
            {
                public const string _TableName = "WordBookIdList";

                public const string BookId = "book_id";
            }

            public class WordExampleIdList
            {
                public const string _TableName = "WordExampleIdList";

                public const string ExampleId = "example_id";
            }
        }

        #region Constructors
        public Word(string sConnectionString) 
            : base(sConnectionString) 
        {}
        #endregion
        
        #region Public Methods
        public DataTable Get(int nWordId)
        {
            DataSet dsResult;
            dsResult = ExecuteDataSet(
                CommandType.StoredProcedure
                , "[dbo].[GetWord]"
                , new SqlCommandParameter("@nWordId", SqlDbType.Int, nWordId)
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.Word._TableName;
                return dsResult.Tables[0];
            }

            return null;
        }

        public void Save(
            int nWordId
            , string sValue
            , string sPronunciation
            , string sTranslation
            , ref int nWordId_NEW
            )
        {
            SqlCommandParameter p_nWordId = new SqlCommandParameter(
                "@nWordId_NEW"
                , SqlDbType.Int
                , nWordId_NEW
                , ParameterDirection.Output
                );

            ExecuteNonQuery(
                CommandType.StoredProcedure
                , "[dbo].[SaveWord]"
                , new SqlCommandParameter("@nWordId", SqlDbType.Int, nWordId)
                , new SqlCommandParameter("@sValue", SqlDbType.NVarChar, sValue)
                , new SqlCommandParameter("@sPronunciation", SqlDbType.NVarChar, sPronunciation)
                , new SqlCommandParameter("@sTranslation", SqlDbType.NVarChar, sTranslation)
                , p_nWordId
                );

            nWordId_NEW = int.Parse(p_nWordId.ParameterValue.ToString());
        }

        public void SaveLocation(
            int nRecordId
            , int nBookId
            , int nWordId
            , int nPageNbr
            , string sLocation
            , ref int nRecordId_NEW
            )
        {
            SqlCommandParameter p_nRecordId = new SqlCommandParameter(
                "@nRecordId_NEW"
                , SqlDbType.Int
                , nRecordId_NEW
                , ParameterDirection.Output
                );

            ExecuteNonQuery(
                CommandType.StoredProcedure
                , "[dbo].[SaveWordLocation]"
                , new SqlCommandParameter("@nRecordId", SqlDbType.Int, nRecordId)
                , new SqlCommandParameter("@nBookId", SqlDbType.Int, nBookId)
                , new SqlCommandParameter("@nWordId", SqlDbType.Int, nWordId)
                , new SqlCommandParameter("@nPageNbr", SqlDbType.Int, nPageNbr)
                , new SqlCommandParameter("@sLocation", SqlDbType.NVarChar, sLocation)
                , p_nRecordId
                );

            nRecordId_NEW = int.Parse(p_nRecordId.ParameterValue.ToString());
        }

        public DataTable GetWordBookIdList(int nWordId)
        {
            DataSet dsResult;
            dsResult = ExecuteDataSet(CommandType.StoredProcedure
                , "[dbo].[GetWordBookIdList]"
                , new SqlCommandParameter("@nWordId", SqlDbType.Int, nWordId)
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.WordBookIdList._TableName;
                return dsResult.Tables[0];
            }

            return null;
        }

        public DataTable GetWordExampleIdList(int nWordId)
        {
            DataSet dsResult;
            dsResult = ExecuteDataSet(CommandType.StoredProcedure
                , "[dbo].[GetWordExampleIdList]"
                , new SqlCommandParameter("@nWordId", SqlDbType.Int, nWordId)
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.WordExampleIdList._TableName;
                return dsResult.Tables[0];
            }

            return null;
        }
        #endregion
    }
}
