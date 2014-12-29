using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NWBA.Base
{
    public class Word
    {
        public class Schema
        {
            public const string ROOT = "word";

            public const string VALUE = "value";
        }

        private string m_sValue;
        public string Value
        {
            get
            {
                return m_sValue;
            }
            set
            {
                m_sValue = value;
            }
        }

        public Word(XElement xWord)
        {
            m_sValue = xWord.Element(Schema.VALUE).Value;
        }
    }
}
