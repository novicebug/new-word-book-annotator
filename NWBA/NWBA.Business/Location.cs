using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Business
{
    public class Location: NWBA.Base.Business
    {
        public class Schema : NWBA.Data.Location.Schema { }

        #region Private Members
        private NWBA.Data.Location m_oLocation;
        private DataTable m_dtInternalData;
        #endregion

        #region Constructors
        public Location()
            : base()
        {
            InitLocation(0, 0);
        }

        public Location(
            int nBookId
            , int nWordId
            )
            : base()
        {
            InitLocation(
                nBookId
                , nWordId
                );
        }

        public Location(
            int nBookId
            , int nWordId
            , string sPageLocation
            )
            : base()
        {
            InitLocation(0, 0);

            this.BookId = nBookId;
            this.WordId = nWordId;
            this.PageLocation = sPageLocation;
        }
        #endregion

        #region Properties
        public int BookId
        {
            get
            {
                return DBConverter.DBValueToInt(m_dtInternalData.Rows[0][Schema.MainInfo.BookId]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.BookId] = DBConverter.IntToDBValue(value);
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

        public string PageLocation
        {
            get
            {
                return DBConverter.DBValueToString(m_dtInternalData.Rows[0][Schema.MainInfo.PageLocation]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.PageLocation] = DBConverter.StringToDBValue(value);
            }
        }
        #endregion
        
        #region Public Methods
        public void Load(
            int nBookId
            , int nWordId
            )
        {
            m_dtInternalData = m_oLocation.Get(
                nBookId
                , nWordId
                );

            if (!m_dtInternalData.HasRows())
            {
                DataRow drNew = m_dtInternalData.NewRow();
                m_dtInternalData.Rows.Add(drNew);
            }
        }

        public void Save()
        {       
            m_oLocation.Save(
                this.BookId
                , this.WordId
                , this.PageLocation
                );
        }
        #endregion

        private void InitLocation(
            int nBookId
            , int nWordId
            )
        {
            m_oLocation = new NWBA.Data.Location(base.ConnectionString);

            this.Load(
                nBookId
                , nWordId
                );        
        }
    }
}
