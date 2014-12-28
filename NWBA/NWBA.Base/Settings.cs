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
        }

        private string m_sBookPath = "";

        public string BookPath
        {
            get 
            {
                return m_sBookPath;
            }
            set
            {
                m_sBookPath = value;
            }
        }

        private string GetSettingsFilePath()
        {
            return Directory.GetCurrentDirectory() + Consts.DIRECTORY_SEPARATOR + Consts.SETTINGS_FILE_NAME;
        }

        public bool LoadSettings()
        {
            string sSettingsFilePath = GetSettingsFilePath();

            if (!File.Exists(sSettingsFilePath))
            {
                SaveSettings();
                return false;
            }

            XDocument xSettings = XDocument.Load(sSettingsFilePath);
            this.BookPath = xSettings.Root.Element(Schema.BOOK_PATH).Value;
            // TODO: Add the other setting fields.

            return true;
        }

        public void SaveSettings()
        {
            string sSettingsFilePath = GetSettingsFilePath();
            XDocument xSettings = new XDocument();
            XElement xRoot = new XElement(Schema.ROOT);

            xRoot.Add(new XElement(Schema.BOOK_PATH, this.BookPath));
            // TODO: Add the other setting fields.

            xSettings.Add(xRoot);
            xSettings.Save(sSettingsFilePath);
        }
    }
}
