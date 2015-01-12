using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NWBA.Base;

namespace NWBA.Business
{
    public class BookExtended: NWBA.Base.Business
    {
        public class Schema : NWBA.Data.Book.Schema { }

        #region Private Members
        private NWBA.Data.Book m_oBook;
        private DataTable m_dtInternalData;
        #endregion

        #region Constructors
        public BookExtended()
            : base()
        {
            m_oBook = new NWBA.Data.Book(base.ConnectionString);
            
            this.Words = new List<WordExtended>();

            Load(0);
        }
        #endregion

        #region Properties
        public bool IsNewBookIn
        {
            get
            {
                return this.BookId == 0;
            }
        }

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

        public string Title
        {
            get
            {
                return DBConverter.DBValueToString(m_dtInternalData.Rows[0][Schema.MainInfo.Title]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.Title] = DBConverter.StringToDBValue(value);
            }
        }

        public string Version
        {
            get
            {
                return DBConverter.DBValueToString(m_dtInternalData.Rows[0][Schema.MainInfo.Version]);
            }
            set
            {
                m_dtInternalData.Rows[0][Schema.MainInfo.Version] = DBConverter.StringToDBValue(value);
            }
        }

        public List<WordExtended> Words { get; set; }
        #endregion
        
        #region Public Methods
        public void Load(int nBookId)
        {
            m_dtInternalData = m_oBook.Get(nBookId);

            if (!m_dtInternalData.HasRows())
            {
                DataRow drNew = m_dtInternalData.NewRow();
                m_dtInternalData.Rows.Add(drNew);
            }

            if (!this.IsNewBookIn)
            {
                FilterWords(""); // Apply no filter.
            }
        }

        public void FilterWords(string sWordPart)
        {
            DataTable dtWordId = m_oBook.GetMatchingWordIdList(
                this.BookId
                , sWordPart
                );

            this.Words.Clear();

            foreach (DataRow row in dtWordId.Rows)
            {
                WordExtended oWord = new WordExtended();
                oWord.Load(DBConverter.DBValueToInt(row[Schema.WordIdList.WordId]));

                this.Words.Add(oWord);
            }
        }

        public void DeleteWord(int nWordId)
        {
            this.Words = this.Words.Where(p => p.WordId != nWordId).ToList();
           
            m_oBook.DeleteWord(
                this.BookId
                , nWordId
                );
        }
        #endregion
    }
}
