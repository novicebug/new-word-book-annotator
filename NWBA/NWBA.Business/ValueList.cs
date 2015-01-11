using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;
using NWBA.Data;

namespace NWBA.Business
{
    public class ValueList: NWBA.Base.Business
    {
        public class Schema : NWBA.Data.ValueList.Schema { }

        #region Private Members
        private NWBA.Data.ValueList m_oValueList;
        #endregion

        #region Constructors
        public ValueList()
            : base()
        {
            m_oValueList = new NWBA.Data.ValueList(base.ConnectionString);
        }
        #endregion
        
        #region Public Methods
        public List<ValueItem> GetBookList()
        {
            List<ValueItem> arrResult = new List<ValueItem>();
            DataTable dtBookList = m_oValueList.GetBookList();

            if (dtBookList != null)
            {
                arrResult = (
                    from DataRow row in dtBookList.Rows
                    select new ValueItem(
                        DBConverter.DBValueToString(row[Schema.ValueList.DisplayMember])
                        , row[Schema.ValueList.ValueMember]
                        )
                    ).ToList();
            }

            return arrResult;
        }
        #endregion
    }
}
