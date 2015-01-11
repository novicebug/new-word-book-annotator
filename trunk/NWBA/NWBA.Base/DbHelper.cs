using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace NWBA.Base
{
    public class DbHelper
    {
        #region Private Members
        private const int DATA_MAX_COLUMN_SIZE = 4000;
        private const int VALUE_MAX_PRECISION = 18;

        private string m_sConnectionString;

        private enum DBMethod
        {
            EXECUTE_DATA_SET
            , EXECUTE_NON_QUERY
            , EXECUTE_SCALAR 
        }
        #endregion

        #region Constructors
        public DbHelper(string sConnectionString)
        {
            m_sConnectionString = sConnectionString;
        }
        #endregion     
  
        #region Private Methods
        private static int AssignCommandParameters(
            SqlCommandParameter[] arrParams
            , SqlDatabase oDB
            , DbCommand oDBCommand
            , Hashtable hshOutParameter
            )
        {
            int nLoop = 0; // Tracks the index of the output parameters.

            foreach (SqlCommandParameter parameter in arrParams)
            {
                switch (parameter.ParameterDirection)
                {
                    case ParameterDirection.Input:
                        oDB.AddInParameter(
                            oDBCommand
                            , parameter.ParameterName
                            , parameter.ParameterType
                            , parameter.ParameterValue
                            );
                        break;

                    case ParameterDirection.Output:
                    case ParameterDirection.InputOutput:
                        hshOutParameter.Add(parameter.ParameterName, nLoop);
                        oDB.AddParameter(
                            oDBCommand
                            , parameter.ParameterName
                            , parameter.ParameterType
                            , DATA_MAX_COLUMN_SIZE
                            , parameter.ParameterDirection
                            , false
                            , VALUE_MAX_PRECISION
                            , 2
                            , ""
                            , DataRowVersion.Original
                            , parameter.ParameterValue
                            );
                        break;

                    default:
                        oDB.AddParameter(
                            oDBCommand
                            , parameter.ParameterName
                            , parameter.ParameterType
                            , parameter.ParameterDirection
                            , ""
                            , DataRowVersion.Original
                            , parameter.ParameterValue
                            );
                        break;
                }

                nLoop++;
            }
            return nLoop;
        }
        
        private DbCommand GetInitializedDbCommand(
            CommandType oCommandType
            , SqlDatabase oDB
            , string sCommandString
            , Hashtable hshOutParameter
            , params SqlCommandParameter[] arrParams
            )
        {
            DbCommand oResult;

            if (oCommandType == CommandType.StoredProcedure)
            {
                oResult = oDB.GetStoredProcCommand(sCommandString);
            }
            else
            {
                oResult = oDB.GetSqlStringCommand(sCommandString);
            }
            
            AssignCommandParameters(
                arrParams
                , oDB
                , oResult
                , hshOutParameter
                );

            return oResult;
        }

        private void SetOutputParametersValue(
            DbCommand oCommand
            , Hashtable hshOutParameter
            , params SqlCommandParameter[] arrParams
            )
        {
            foreach (DictionaryEntry entry in hshOutParameter)
            {
                arrParams[(int)entry.Value].ParameterValue = oCommand.Parameters[entry.Key.ToString()].Value;
            }
        }

        private object ExecuteDBCommand(
            DBMethod eMethod
            , CommandType oCommandType
            , string sCommandString
            , params SqlCommandParameter[] arrParams
            )
        {
            SqlDatabase oDB = new SqlDatabase(m_sConnectionString);
            Hashtable hshOutParameter = new Hashtable();

            DbCommand oCommand = GetInitializedDbCommand(
                oCommandType
                , oDB
                , sCommandString
                , hshOutParameter
                , arrParams
                );
            
            object oResult = null;
            switch (eMethod)
            {
                case DBMethod.EXECUTE_DATA_SET:
                    oResult = oDB.ExecuteDataSet(oCommand);
                    break;

                case DBMethod.EXECUTE_NON_QUERY:
                    oResult = oDB.ExecuteNonQuery(oCommand);
                    break;

                case DBMethod.EXECUTE_SCALAR:
                    oResult = oDB.ExecuteDataSet(oCommand);
                    break;
            }

            SetOutputParametersValue(
                oCommand
                , hshOutParameter
                , arrParams
                );

            return oResult;
        }
        #endregion

        #region Public Methods
        public DataSet ExecuteDataSet(
            CommandType oCommandType
            , string sCommandString
            , params SqlCommandParameter[] arrParams
            )
        {
            object oResult = ExecuteDBCommand(
                DBMethod.EXECUTE_DATA_SET
                , oCommandType
                , sCommandString
                , arrParams
                );

            return (DataSet)oResult;
        }

        public int ExecuteNonQuery(
            CommandType oCommandType
            , string sCommandString
            , params SqlCommandParameter[] arrParams
            )
        {
            object oResult = ExecuteDBCommand(
                DBMethod.EXECUTE_NON_QUERY
                , oCommandType
                , sCommandString
                , arrParams
                );

            return (int)oResult;
        }

        public object ExecuteScalar(
            CommandType oCommandType
            , string sCommandString
            , params SqlCommandParameter[] arrParams
            )
        {
            object oResult = ExecuteDBCommand(
                DBMethod.EXECUTE_NON_QUERY
                , oCommandType
                , sCommandString
                , arrParams
                );
            
            return oResult;
        }
        #endregion        
    }
}
