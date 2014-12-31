using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace NWBA.Base
{
    public class Settings
    {
        public class Schema
        {
            public const string ROOT = "root";

            public const string BOOK_PATH = "book_path";
            public const string LAST_BOOK_ID = "last_book_id";
        }

        #region " Properties "
        public int LastBookId { get; set; }
        public string BookPath { get; set; }
        #endregion

        private string GetSettingsFilePath()
        {
            return Directory.GetCurrentDirectory() + Consts.DIRECTORY_SEPARATOR + Consts.SETTINGS_FILE_NAME;
        }

        public bool LoadSettings()
        {
            string sSettingsFilePath = GetSettingsFilePath();

            if (!File.Exists(sSettingsFilePath))
            {
                Save();
                return false;
            }

            XDocument xSettings = XDocument.Load(sSettingsFilePath);
            this.BookPath = xSettings.Root.GetStringValue(Schema.BOOK_PATH);
            this.LastBookId = int.Parse(xSettings.Root.GetStringValue(Schema.LAST_BOOK_ID));
            // TODO: Add the other setting fields.

            return true;
        }

        public void Save()
        {
            string sSettingsFilePath = GetSettingsFilePath();
            XDocument xSettings = new XDocument();
            XElement xRoot = new XElement(Schema.ROOT);

            xRoot.Add(new XElement(Schema.BOOK_PATH, this.BookPath));
            xRoot.Add(new XElement(Schema.LAST_BOOK_ID, this.LastBookId));

            xSettings.Add(xRoot);
            xSettings.Save(sSettingsFilePath);
        }
    }
}
