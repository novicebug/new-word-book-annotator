using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Data
{
    public class Example: NWBA.Base.Data
    {
        public class Schema
        {
            public class MainInfo
            {
                public const string _TableName = "MainInfo";

                public const string ExampleId = "example_id";
                public const string WordId = "word_id";
                public const string Value = "value";
                public const string IsPrintIn = "is_print_in";
                public const string OrderNbr = "order_nbr";
            }
        }

        #region Constructors
        public Example(string sConnectionString) 
            : base(sConnectionString)
        { }
        #endregion

        #region Public Methods
        public DataTable Get(int nExampleId)
        {
            DataSet dsResult;
            dsResult = ExecuteDataSet(
                CommandType.StoredProcedure
                , "[dbo].[GetExample]"
                , new SqlCommandParameter("@nExampleId", SqlDbType.BigInt, nExampleId)
                );

            if (dsResult.HasTables())
            {
                dsResult.Tables[0].TableName = Schema.MainInfo._TableName;
                return dsResult.Tables[0];
            }

            return null;
        }
               
        public void Save(
            int nExampleId
            , int nWordId
            , string sValue
            , bool bIsPrintIn
            , int nOrderNbr
            )
        {
            ExecuteNonQuery(
                CommandType.StoredProcedure
                , "[dbo].[SaveExample]"
                , new SqlCommandParameter("@nExampleId", SqlDbType.Int, nExampleId)
                , new SqlCommandParameter("@nWordId", SqlDbType.Int, nWordId)
                , new SqlCommandParameter("@sValue", SqlDbType.NVarChar, sValue)
                , new SqlCommandParameter("@bIsPrintIn", SqlDbType.Bit, bIsPrintIn)
                , new SqlCommandParameter("@nOrderNbr", SqlDbType.Int, nOrderNbr)
                );
        }

        public void Delete(int nExampleId)
        {
            ExecuteNonQuery(
                CommandType.StoredProcedure
                , "[dbo].[DeleteExample]"
                , new SqlCommandParameter("@nExampleId", SqlDbType.Int, nExampleId)
                );
        }
        #endregion
    }
}
