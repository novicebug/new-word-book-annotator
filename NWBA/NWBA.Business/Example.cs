using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Business
{
    public class Example: NWBA.Base.Business
    {
        public class Schema : NWBA.Data.Example.Schema { }

        #region Private Members
        private NWBA.Data.Example m_oExample;
        private DataTable m_dtInternalData;
        #endregion

        #region Constructors
        public Example()
            : base()
        {
            m_oExample = new NWBA.Data.Example(base.ConnectionString);
            
            Load(0);
        }
        #endregion

        #region Properties
        public int ExampleId
        {
            get
            {
                return DBConverter.DBValueToInt(m_dtInternalData.Rows[0][Schema.MainInfo.ExampleId]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.ExampleId] = DBConverter.IntToDBValue(value);
            }
        }

        public int WordId
        {
            get
            {
                return DBConverter.DBValueToInt(m_dtInternalData.Rows[0][Schema.MainInfo.WordId]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.WordId] = DBConverter.IntToDBValue(value);
            }
        }

        public string Value
        {
            get
            {
                return DBConverter.DBValueToString(m_dtInternalData.Rows[0][Schema.MainInfo.Value]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.Value] = DBConverter.StringToDBValue(value);
            }
        }

        public bool IsPrintIn
        {
            get
            {
                return DBConverter.DBValueToBool(m_dtInternalData.Rows[0][Schema.MainInfo.IsPrintIn]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.IsPrintIn] = DBConverter.BoolToDBValue(value);
            }
        }

        public int OrderNbr
        {
            get
            {
                return DBConverter.DBValueToInt(m_dtInternalData.Rows[0][Schema.MainInfo.OrderNbr]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.OrderNbr] = DBConverter.IntToDBValue(value);
            }
        }
        #endregion
        
        #region Public Methods
        public void Load(int nExampleId)
        {
            m_dtInternalData = m_oExample.Get(nExampleId);

            if (!m_dtInternalData.HasRows())
            {
                DataRow drNew = m_dtInternalData.NewRow();
                m_dtInternalData.Rows.Add(drNew);
            }
        }

        public void Save()
        {       
            m_oExample.Save(
                this.ExampleId
                , this.WordId
                , this.Value
                , this.IsPrintIn
                , this.OrderNbr
                );
        }
        #endregion
    }
}
