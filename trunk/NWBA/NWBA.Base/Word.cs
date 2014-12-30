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
            public const string ROOT = "word";

            public const string VALUE = "value";
        }

        public string Value { get; set; }

        public Word(XElement xWord)
        {
            this.Value = xWord.Element(Schema.VALUE).Value;
        }
    }
}
