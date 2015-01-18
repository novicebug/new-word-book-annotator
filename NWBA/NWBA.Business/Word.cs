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
            InitWord(0);
        }

        public Word(int nWordId)
            : base()
        {
            InitWord(nWordId);
        }
        
        public Word(
            string sValue
            , string sPronunciation
            , string sTranslation
            )
            : base()
        {         
            InitWord(0);

            this.Value = sValue;
            this.Pronunciation = sPronunciation;
            this.Translation = sTranslation;
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

        public string Explanation
        {
            get
            {
                return DBConverter.DBValueToString(m_dtInternalData.Rows[0][Schema.Word.Explanation]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.Word.Explanation] = DBConverter.StringToDBValue(value);
            }
        }

        public List<Location> Locations { get; set; }

        public List<Example> Examples { get; set; }
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
            LoadExamples();
        }

        public void SaveInBook(int nBookId)
        {
            int nNewWordId = 0;

            m_oWord.Save(
                this.WordId
                , this.Value
                , this.Pronunciation
                , this.Translation
                , this.Explanation
                , ref nNewWordId
                );

            this.WordId = nNewWordId;

            Location oLocation = GetLocation(nBookId);
            oLocation.WordId = this.WordId;
            oLocation.Save();

            foreach (Example item in this.Examples)
            {
                item.WordId = this.WordId;
                item.Save();
            }
        }

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

        public void LoadExamples()
        {
            DataTable dtExampleIdList = m_oWord.GetWordExampleIdList(this.WordId);

            this.Examples.Clear();

            foreach (DataRow item in dtExampleIdList.Rows)
            {
                Example oExample = new Example();
                oExample.Load(DBConverter.DBValueToInt(item[Schema.WordExampleIdList.ExampleId]));

                this.Examples.Add(oExample);
            }
        }

        public void AddLocation(
            int nBookId
            , string sPageLocation
            )
        {
            Location oLocation = (
                from item in this.Locations
                where item.BookId == nBookId
                    && item.WordId == this.WordId
                select item
                ).FirstOrDefault();

            if (oLocation == null)
            {
                oLocation = new Location(
                    nBookId
                    , this.WordId
                    , sPageLocation
                    );

                this.Locations.Add(oLocation);
            }
            else
            {
                oLocation.PageLocation = sPageLocation;
            }
        }

        public string GetPageLocation(int nBookId)
        {
            Location oLocation = GetLocation(nBookId);

            return oLocation.PageLocation;
        }

        public void AddExample(
            string sValue
            , bool bIsPrintIn
            , int nOrderNbr
            )
        {
            Example oExample = (
                from item in this.Examples
                where item.WordId == this.WordId
                    && item.OrderNbr == nOrderNbr
                select item
                ).FirstOrDefault();

            if (oExample == null)
            {
                oExample = new Example(
                    this.WordId
                    , sValue
                    , bIsPrintIn
                    , nOrderNbr
                    );
                this.Examples.Add(oExample);
            }
            else
            {
                oExample.Value = sValue;
                oExample.IsPrintIn = bIsPrintIn;
            }
        }
        
        public Example GetExample(int nOrderNbr)
        {
            Example oResult = (
                from item in this.Examples
                where item.WordId == this.WordId
                    && item.OrderNbr == nOrderNbr
                select item
                ).FirstOrDefault();

            if (oResult == null)
            {
                oResult = new Example();
            }

            return oResult;
        }

        public string GetPrintableExamples(int nMaxExampleToReturn)
        {
            StringBuilder sbResult = new StringBuilder();

            foreach (Example item in this.Examples)
            {
                if (item.IsPrintIn)
                {
                    sbResult.AppendLine("- " + item.Value);
                }
            }

            return sbResult.ToString();
        }

        public string GetPrintableExamples()
        {
            return this.GetPrintableExamples(3);
        }
        #endregion

        #region Private Methods
        private void InitWord(int nWordId) 
        {
            m_oWord = new NWBA.Data.Word(base.ConnectionString);

            this.Locations = new List<Location>();
            this.Examples = new List<Example>();

            Load(nWordId);
        }

        private Location GetLocation(int nBookId)
        {
            Location oResult = (
                from item in this.Locations
                where item.BookId == nBookId
                select item
                ).FirstOrDefault();

            if (oResult == null)
            {
                oResult = new Location(
                    nBookId
                    , this.WordId
                    , ""
                    );
            }

            return oResult;
        }
        #endregion
    }
}
