using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NWBA.Base
{
    public class Book
    {
        public class Schema
        {
            public const string ROOT = "root";

            public const string VERSION = "version";
            public const string BOOK_TITLE = "book_title";
            public const string WORDS = "words";
        }

        private int m_nId;
        public int Id
        {
            get
            {
                return m_nId;
            }
            set 
            {
                m_nId = value;
            }
        }
                
        private string m_sVersion;
        public string Version
        {
            get
            {
                return m_sVersion;
            }
            set 
            {
                m_sVersion = value;
            }
        }

        private string m_sTitle;
        public string Title
        {
            get
            {
                return m_sTitle;
            }
            set 
            {
                m_sTitle = value;
            }
        }

        private List<Word> m_arrWords;
        public List<Word> Words
        {
            get
            {
                return m_arrWords;
            }
            set 
            {
                m_arrWords = value;
            }
        }

        public Book(string sBookFilePath)
        {
            string sBookFileName = System.IO.Path.GetFileNameWithoutExtension(sBookFilePath);
            XDocument xBook = XDocument.Load(sBookFilePath);

            InitBook(xBook, int.Parse(sBookFileName.Substring(4)));
        }

        public Book(string sBookRootPath, int nId)
        {
            string sBookFilePath = sBookRootPath + Consts.DIRECTORY_SEPARATOR + "book" + nId.ToString() + ".xml";
            XDocument xBook = XDocument.Load(sBookFilePath);

            InitBook(xBook, nId);
        }

        private void InitBook(XDocument xBook, int nId)
        {
            m_nId = nId;
            m_sVersion = xBook.Root.Element(Schema.VERSION).Value;
            m_sTitle = xBook.Root.Element(Schema.BOOK_TITLE).Value;

            m_arrWords = new List<Word>();
            foreach (XElement xWord in xBook.Root.Element(Schema.WORDS).Elements())
            {
                m_arrWords.Add(new Word(xWord));
            }
        }

        public List<Word> FindMatchingWords(string sWordPart)
        {
            List<Word> result = new List<Word>();

            foreach(Word word in m_arrWords)
            {
                if (word.Value.Contains(sWordPart))
                {
                    result.Add(word);
                }
            }

            return result;
        }
    }
}
