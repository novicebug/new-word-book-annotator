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
using System.Collections.ObjectModel;

namespace NWBA
{
    /// <summary>
    /// Interaction logic for AddExamplesWindow.xaml
    /// </summary>
    public partial class AddExamplesWindow : Window
    {
        private const string VOCABULARY_URL = "http://corpus.vocabulary.com/api/1.0/examples.json?query={0}&maxResults={1}&startOffset={2}";
        private const int VOCABULARY_PAGE_SIZE = 24;
        private const string VOCABULARY_SENTENCE_START_SEPARATOR = "\"sentence\":\"";
        private const string VOCABULARY_SENTENCE_END_SEPARATOR = "\",\"volumeOffset\":";
        private const string VOCABULARY_QUOTE_ENCODE = "\\\"";
        private const string VOCABULARY_OPEN_QUOTE_ENCODE = "\\u201c";
        private const string VOCABULARY_CLOSE_QUOTE_ENCODE = "\\u201d";
        private const string VOCABULARY_APOSTROPHE_ENCODE = "\\u2019";
        private const string VOCABULARY_DASH_ENCODE = "\\u2014";

        private string m_sSearchedWord;
        private int m_nNeededExamplesCount;
        private int m_nCurrentPage = 0;
        private ObservableCollection<ExampleItem> m_arrExamples;
                
        public event EventHandler ExamplesSelected;

        public class ExampleItem
        {
            public bool IsSelectedIn { get; set; }
            public string Sentence { get; set; }
        }

        public AddExamplesWindow(
            string sSearchedWord
            , int nNeededExamplesCount
            )
        {
            InitializeComponent();

            m_sSearchedWord = sSearchedWord;
            m_nNeededExamplesCount = nNeededExamplesCount;
            m_arrExamples = new ObservableCollection<ExampleItem>();

            LoadExamples();

            SetWindowFontSize();
        }

        private void SetWindowFontSize()
        {
            NWBA.Business.Setting oSetting = new NWBA.Business.Setting();
            oSetting.Load();
            this.FontSize = oSetting.FontSize;
        }

        private void cmdShowMoreExamples_Click(object sender, RoutedEventArgs e)
        {
            m_nCurrentPage += 1;
            LoadExamples();
        }

        private void cmdSaveExamples_Click(object sender, RoutedEventArgs e)
        {
            NWBA.Base.ExamplesSelectedEventArgs args = new NWBA.Base.ExamplesSelectedEventArgs();
            args.Examples =
                (from item in m_arrExamples
                 where item.IsSelectedIn
                 select item.Sentence                 
                 ).ToList();
            
            ExamplesSelected.Invoke(this, args);

            this.Close();
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

                            if (nStartPosition >= 0)
                            {
                                nSentenceEndPosition = sReadData.IndexOf(VOCABULARY_SENTENCE_END_SEPARATOR, nStartPosition);

                                string sSentence = sReadData.Substring(
                                    nStartPosition + VOCABULARY_SENTENCE_START_SEPARATOR.Length
                                    , nSentenceEndPosition - nStartPosition - VOCABULARY_SENTENCE_START_SEPARATOR.Length
                                    );
                                sSentence = sSentence
                                    .Replace(VOCABULARY_QUOTE_ENCODE, "\"")
                                    .Replace(VOCABULARY_OPEN_QUOTE_ENCODE, "\"")
                                    .Replace(VOCABULARY_CLOSE_QUOTE_ENCODE, "\"")
                                    .Replace(VOCABULARY_DASH_ENCODE, "-")
                                    .Replace(VOCABULARY_APOSTROPHE_ENCODE, "'");

                                m_arrExamples.Add(new ExampleItem()
                                {
                                    IsSelectedIn = false
                                    , Sentence = sSentence
                                });

                                nStartPosition = nSentenceEndPosition;
                            }
                        }
                        while (nStartPosition >= 0);

                        icExamples.ItemsSource = m_arrExamples;
                    }
                }
            }
        }

        private void Sentence_Checked(object sender, RoutedEventArgs e)
        {
            m_nNeededExamplesCount--;

            if (m_nNeededExamplesCount <= 0)
            {
                cmdSaveExamples.Background = new SolidColorBrush(Colors.Green);
            }
        }

        private void Sentence_Unchecked(object sender, RoutedEventArgs e)
        {
            m_nNeededExamplesCount++;

            if (m_nNeededExamplesCount > 0)
            {
                cmdSaveExamples.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
