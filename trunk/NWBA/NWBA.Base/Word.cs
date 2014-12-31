using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace NWBA.Base
{
    public class Word
    {
        public class Schema
        {
            public const string WORD = "word";

            public const string VALUE = "value";
            public const string PRONUNCIATION = "pronunciation";
            public const string PAGE_LOCATION = "page_location";
        }

        public string Value { get; set; }
        public string Pronunciation { get; set; }
        public string PageLocation { get; set; }

        public Word() {}

        public Word(XElement xWord)
        {
            this.Value = xWord.GetStringValue(Schema.VALUE);
            this.Pronunciation = xWord.GetStringValue(Schema.PRONUNCIATION);
            this.PageLocation = xWord.GetStringValue(Schema.PAGE_LOCATION);
        }

        public XElement GetWordDocument()
        {
            XElement xResult = new XElement(Schema.WORD);

            xResult.Add(new XElement(Schema.VALUE, this.Value));
            xResult.Add(new XElement(Schema.PRONUNCIATION, this.Pronunciation));
            xResult.Add(new XElement(Schema.PAGE_LOCATION, this.PageLocation));

            return xResult;
        }
    }
}
