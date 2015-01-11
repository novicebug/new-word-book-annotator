using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace NWBA.Base
{
    public class _Word
    {
        public class Schema
        {
            public const string WORD = "word";

            public const string VALUE = "value";
            public const string PRONUNCIATION = "pronunciation";
            public const string TRANSLATION = "translation";
            public const string PAGE_LOCATION = "page_location";
            public const string EXAMPLES = "examples";
        }

        #region " Properties "
        public string Value { get; set; }
        public string Pronunciation { get; set; }
        public string Translation { get; set; }
        public string PageLocation { get; set; }
        public string Examples { get; set; }
        #endregion

        public _Word() {}

        public _Word(XElement xWord)
        {
            this.Value = xWord.GetStringValue(Schema.VALUE);
            this.Pronunciation = xWord.GetStringValue(Schema.PRONUNCIATION);
            this.Translation = xWord.GetStringValue(Schema.TRANSLATION);
            this.PageLocation = xWord.GetStringValue(Schema.PAGE_LOCATION);
            this.Examples = xWord.GetStringValue(Schema.EXAMPLES);
        }

        public XElement GetWordDocument()
        {
            XElement xResult = new XElement(Schema.WORD);

            xResult.Add(new XElement(Schema.VALUE, this.Value));
            xResult.Add(new XElement(Schema.PRONUNCIATION, this.Pronunciation));
            xResult.Add(new XElement(Schema.TRANSLATION, this.Translation));
            xResult.Add(new XElement(Schema.PAGE_LOCATION, this.PageLocation));
            xResult.Add(new XElement(Schema.EXAMPLES, this.Examples));

            return xResult;
        }
    }
}
