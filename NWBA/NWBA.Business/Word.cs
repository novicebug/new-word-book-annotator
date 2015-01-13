using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Business
{
    public class Word: NWBA.Base.Business
    {
        public class Schema : NWBA.Data.Word.Schema { }

        #region Private Members
        private NWBA.Data.Word m_oWord;
        private DataTable m_dtInternalData;
        #endregion

        #region Constructors
        public Word()
            : base()
        {
            m_oWord = new NWBA.Data.Word(base.ConnectionString);

            this.Locations = new List<Location>();
            
            Load(0);
        }
        #endregion

        #region Properties
        public int WordId
        {
            get
            {
                return DBConverter.DBValueToInt(m_dtInternalData.Rows[0][Schema.Word.WordId]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.Word.WordId] = DBConverter.IntToDBValue(value);
            }
        }

        public string Value
        {
            get
            {
                return DBConverter.DBValueToString(m_dtInternalData.Rows[0][Schema.Word.Value]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.Word.Value] = DBConverter.StringToDBValue(value);
            }
        }

        public string Pronunciation
        {
            get
            {
                return DBConverter.DBValueToString(m_dtInternalData.Rows[0][Schema.Word.Pronunciation]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.Word.Pronunciation] = DBConverter.StringToDBValue(value);
            }
        }

        public string Translation
        {
            get
            {
                return DBConverter.DBValueToString(m_dtInternalData.Rows[0][Schema.Word.Translation]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.Word.Translation] = DBConverter.StringToDBValue(value);
            }
        }

        public List<Location> Locations { get; set; }
        #endregion
        
        #region Public Methods
        public void Load(int nWordId)
        {
            m_dtInternalData = m_oWord.Get(nWordId);

            if (!m_dtInternalData.HasRows())
            {
                DataRow drNew = m_dtInternalData.NewRow();
                m_dtInternalData.Rows.Add(drNew);

                return;
            }

            LoadLocations();
        }

        public void Save(
            int nBookId
            , string sPageLocation
            )
        {
            int nNewWordId = 0;

            m_oWord.Save(
                this.WordId
                , this.Value
                , this.Pronunciation
                , this.Translation
                , ref nNewWordId
                );

            this.WordId = nNewWordId;

            Location oLocation = GetLocation(nBookId);

            oLocation.PageLocation = sPageLocation;
            oLocation.Save();
        }

        //public void SaveLocation(
        //    int nBookId
        //    , string sPageLocation
        //    )
        //{
        //    Location oLocation = GetLocation(nBookId);
                      
        //    oLocation.PageLocation = sPageLocation;
        //    oLocation.Save();
        //}

        public void LoadLocations()
        {
            DataTable dtBookIdList = m_oWord.GetWordBookIdList(this.WordId);

            this.Locations.Clear();

            foreach (DataRow item in dtBookIdList.Rows)
            {
                Location oLocation = new Location();
                oLocation.Load(
                    DBConverter.DBValueToInt(item[Schema.WordBookIdList.BookId])
                    , this.WordId
                    );

                this.Locations.Add(oLocation);
            }
        }

        public Location GetLocation(int nBookId)
        {
            Location oResult = (
                from item in this.Locations
                where item.BookId == nBookId
                    && item.WordId == this.WordId
                select item
                ).FirstOrDefault();

            if (oResult == null)
            {
                oResult = new Location();
                oResult.BookId = nBookId;
                oResult.WordId = this.WordId;
            }

            return oResult;
        }
        #endregion
    }
}
