using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Business
{
    public class Setting: NWBA.Base.Business
    {
        public class Schema : NWBA.Data.Setting.Schema { }

        #region Private Members
        private NWBA.Data.Setting m_oSetting;
        private DataTable m_dtInternalData;
        #endregion

        #region Constructors
        public Setting()
            : base()
        {
            m_oSetting = new NWBA.Data.Setting(base.ConnectionString);
            
            Load();
        }
        #endregion

        #region Properties
        public int? LastBookId
        {
            get
            {
                return DBConverter.DBValueToIntNullable(m_dtInternalData.Rows[0][Schema.MainInfo.LastBookId]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.LastBookId] = DBConverter.IntToDBValue(value);
            }
        }
        #endregion
        
        #region Public Methods
        public void Load()
        {
            m_dtInternalData = m_oSetting.Get();

            if (!m_dtInternalData.HasRows())
            {
                DataRow drNew = m_dtInternalData.NewRow();
                m_dtInternalData.Rows.Add(drNew);
            }
        }

        public void Save()
        {       
            m_oSetting.Save(this.LastBookId.Value);
        }
        #endregion
    }
}
