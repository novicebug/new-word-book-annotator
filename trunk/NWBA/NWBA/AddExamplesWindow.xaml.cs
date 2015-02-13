using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.IO;

namespace NWBA
{
    /// <summary>
    /// Interaction logic for AddExamplesWindow.xaml
    /// </summary>
    public partial class AddExamplesWindow : Window
    {
        private string VOCABULARY_URL = "http://corpus.vocabulary.com/api/1.0/examples.json?query={0}&maxResults={1}&startOffset={2}";
        private int VOCABULARY_PAGE_SIZE = 24;
        private string VOCABULARY_SENTENCE_START_SEPARATOR = "\"sentence\":\"";
        private string VOCABULARY_SENTENCE_END_SEPARATOR = "\",\"volumeOffset\":";
        private string VOCABULARY_QUOTE_ENCODE = "\\\"";

        private string m_sSearchedWord;
        private int m_nNeededExamplesCount;
        private int m_nCurrentPage = 0;
        private List<string> m_arrExamples;
        
        public event EventHandler ExamplesSelected;

        public AddExamplesWindow(
            string sSearchedWord
            , int nNeededExamplesCount
            )
        {
            InitializeComponent();

            m_sSearchedWord = sSearchedWord;
            m_nNeededExamplesCount = nNeededExamplesCount;
            m_arrExamples = new List<string>();

            LoadExamples();
        }

        private void cmdShowMoreExamples_Click(object sender, RoutedEventArgs e)
        {
            m_nCurrentPage += 1;
            LoadExamples();
        }

        private void cmdSaveExamples_Click(object sender, RoutedEventArgs e)
        {
            NWBA.Base.ExamplesSelectedEventArgs args = new NWBA.Base.ExamplesSelectedEventArgs();
            args.Examples = m_arrExamples; // TODO: Return just the checked examples.
            
            ExamplesSelected.Invoke(this, args);
        }

        private void LoadExamples()
        {
            using (WebClient client = new WebClient())
            {
                using (Stream data = client.OpenRead(string.Format(VOCABULARY_URL, m_sSearchedWord, VOCABULARY_PAGE_SIZE, m_nCurrentPage * VOCABULARY_PAGE_SIZE)))
                {
                    using (StreamReader reader = new StreamReader(data))
                    {
                        string sReadData = reader.ReadToEnd();
                        int nStartPosition = 0;
                        int nSentenceEndPosition = 0;

                        do
                        {
                            nStartPosition = sReadData.IndexOf(VOCABULARY_SENTENCE_START_SEPARATOR, nStartPosition);
                            nSentenceEndPosition = sReadData.IndexOf(VOCABULARY_SENTENCE_END_SEPARATOR, nStartPosition);

                            if (nStartPosition < nSentenceEndPosition)
                            {
                                m_arrExamples.Add(
                                    sReadData.Substring(
                                        nStartPosition + VOCABULARY_SENTENCE_START_SEPARATOR.Length
                                        , nSentenceEndPosition - nStartPosition - VOCABULARY_SENTENCE_START_SEPARATOR.Length
                                        ).Replace(VOCABULARY_QUOTE_ENCODE, "\"")
                                    );
                            }

                            nStartPosition = nSentenceEndPosition;
                        }
                        while (nStartPosition >= 0);
                        
                        // TODO: Render the words in a template control with checkboxes. On each check loar the number of left
                        // examples to add at the top label.
                    }
                }
            }
        }
    }
}
