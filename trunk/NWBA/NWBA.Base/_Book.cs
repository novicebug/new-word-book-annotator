using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NWBA.Base
{
    public class _Book
    {
        public class Schema
        {
            public const string ROOT = "root";

            public const string VERSION = "version";
            public const string BOOK_TITLE = "book_title";
            public const string WORDS = "words";
            public const string WORD = "word";
        }

        private string m_sBookFilePath;

        #region " Properties "
        public int Id { get; set; }       
        public string Version { get; set; }
        public string Title { get; set; }
        public List<_Word> Words { get; set; }
        #endregion

        public _Book(string sBookFilePath)
        {
            m_sBookFilePath = sBookFilePath;

            string sBookFileName = System.IO.Path.GetFileNameWithoutExtension(sBookFilePath);
            XDocument xBook = XDocument.Load(m_sBookFilePath);

            InitBook(xBook, int.Parse(sBookFileName.Substring(4)));
        }

        public _Book(string sBookRootPath, int nId)
        {
            m_sBookFilePath = sBookRootPath + Consts.DIRECTORY_SEPARATOR + "book" + nId.ToString() + ".xml";

            XDocument xBook = XDocument.Load(m_sBookFilePath);

            InitBook(xBook, nId);
        }

        private void InitBook(XDocument xBook, int nId)
        {
            this.Id = nId;
            this.Version = xBook.Root.GetStringValue(Schema.VERSION);
            this.Title = xBook.Root.GetStringValue(Schema.BOOK_TITLE);

            this.Words = new List<_Word>();
            foreach (XElement xWord in xBook.Root.Element(Schema.WORDS).Elements())
            {
                this.Words.Add(new _Word(xWord));
            }
        }

        public List<_Word> FindMatchingWords(string sWordPart)
        {
            List<_Word> result = new List<_Word>();

            foreach(_Word word in this.Words)
            {
                if (word.Value.Contains(sWordPart))
                {
                    result.Add(word);
                }
            }

            return result;
        }

        public void AddWord(_Word oWord)
        {
            _Word oOldWord = this.Words.Where(p => p.Value == oWord.Value).FirstOrDefault();
            this.Words.Remove(oOldWord);
            this.Words.Add(oWord);
        }

        public void Save()
        {
            XDocument xBook = new XDocument();
            XElement xRoot = new XElement(Schema.ROOT);
            XElement xWords = new XElement(Schema.WORDS);

            xRoot.Add(new XElement(Schema.VERSION, this.Version));
            xRoot.Add(new XElement(Schema.BOOK_TITLE, this.Title));

            foreach (_Word item in this.Words)
            {
                xWords.Add(item.GetWordDocument());
            }
            xRoot.Add(xWords);

            xBook.Add(xRoot);
            xBook.Save(m_sBookFilePath);
        }
    }
}
