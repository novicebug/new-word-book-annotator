using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Data;
using NWBA.Base;
using NWBA.Business;

namespace NWBA
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        private Settings m_oSettings;
        private _Book m_oCurrentBook;

        private BookExtended m_oCurrentBookExtended;
        private WordExtended m_oCurrentWord;

        #region " Properties "
        public ObservableCollection<ValueItem> BookList { get; set; }
        public ObservableCollection<WordExtended> MatchingWords { get; set; }
        #endregion

        public HomeWindow()
        {
            InitializeComponent();

            this.BookList = new ObservableCollection<ValueItem>();
            this.MatchingWords = new ObservableCollection<WordExtended>();

            LayoutRoot.DataContext = this;

            m_oSettings = new Settings();

            if (!m_oSettings.LoadSettings())
            {
                tiHome.IsEnabled = false;
                tiAdd.IsEnabled = false;

                tcMenu.SelectedItem = tiSettings;
            }

            LoadHomeTab();
            LoadSettingsTab();
        }

        private string GetBookRootPath()
        {
            return m_oSettings.BookPath + Consts.DIRECTORY_SEPARATOR + Consts.BOOK_DIRECTORY_PATH;
        }

        #region " Home Tab "
        private void LoadBookList()
        {
            //string sBookRootPath = GetBookRootPath();
            //string[] books = new string[] { };

            //if (Directory.Exists(sBookRootPath))
            //{
            //    books = Directory.GetFiles(sBookRootPath);
            //}

            //this.BookList.Add(new ValueItem("---ALL---", 0));
            
            //foreach (string sBookFilePath in books)
            //{
            //    _Book oBook = new _Book(sBookFilePath);

            //    this.BookList.Add(new ValueItem(oBook.Title, oBook.Id));
            //}

            //lstBook.SelectedValue = m_oSettings.LastBookId;


            ValueList oValueList = new ValueList();
            List<ValueItem> arrBooks = oValueList.GetBookList();
            
            this.BookList.Add(new ValueItem("---ALL---", 0));          
            foreach (ValueItem item in arrBooks)
            {
                this.BookList.Add(item);
            }

            lstBook.SelectedValue = m_oSettings.LastBookId;
        }

        private void LoadHomeTab()
        {
            LoadBookList();
        }

        private void SearchWords()
        {
            this.MatchingWords.Clear();

            if (m_oCurrentBookExtended.IsNewBookIn)
            {
                return;
            }

            m_oCurrentBookExtended.FilterWords(txtSearch.Text);
                       
            foreach (WordExtended item in m_oCurrentBookExtended.Words)
            {
                this.MatchingWords.Add(item);
            }
        }

        private void lstBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nBookId = (int)lstBook.SelectedValue;

            txtSearch.Focus();

            if (nBookId == 0) // All
            {
                tiAdd.IsEnabled = false;
            }
            else
            {
                tiAdd.IsEnabled = true;

                BookExtended oBook = new BookExtended();
                oBook.Load(nBookId);
                m_oCurrentBookExtended = oBook;

                SearchWords();

                m_oSettings.LastBookId = nBookId;
                m_oSettings.Save();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchWords();
        }

        private void lstMatchingWords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMatchingWords.SelectedValue == null)
            {
                return;
            }

            int nWordId = (int)lstMatchingWords.SelectedValue;
            m_oCurrentWord = new WordExtended();
            m_oCurrentWord.Load(nWordId);

            lblWord.Text = m_oCurrentWord.Value;
            lblPronunciation.Text = m_oCurrentWord.Pronunciation;
            lblTranslation.Text = m_oCurrentWord.Translation;
            // TODO:
            lblPageLocation.Text = "Page (Location): " + m_oCurrentWord.GetLocation(m_oCurrentBookExtended.BookId).PageLocation;
            //lblExamples.Text = oSelectedWord.Examples;

            lblExamplesLabel.Visibility = Visibility.Visible;
        }

        private void cmdEditWord_Click(object sender, RoutedEventArgs e)
        {            
            if (lstMatchingWords.SelectedValue == null)
            {
                return;
            }
            
            //int nWordId = (int)lstMatchingWords.SelectedValue;
            //WordExtended oSelectedWord = new WordExtended();
            //oSelectedWord.LoadWord(nWordId); 

           // _Word oSelectedWord = (_Word)lstMatchingWords.SelectedValue;

            if (m_oCurrentWord == null)
            {
                return;
            }

            txtWord.Text = m_oCurrentWord.Value;
            txtPronunciation.Text = m_oCurrentWord.Pronunciation;
            txtTranslation.Text = m_oCurrentWord.Translation;
            // TODO:
            //txtPageLocation.Text = oSelectedWord.PageLocation;
            //txtExamples.Text = oSelectedWord.Examples;

            tcMenu.SelectedItem = tiAdd;

        //    m_oCurrentBookExtended.Words.Remove(oSelectedWord);
        }
        
        private void cmdDeleteWord_Click(object sender, RoutedEventArgs e)
        {
            _Word oSelectedWord = (_Word)lstMatchingWords.SelectedValue;

            if (oSelectedWord == null)
            {
                return;
            }

            MessageBoxResult oResult = MessageBox.Show(
                "Are you sure you want to delete this word?"
                , "NWBA"
                , MessageBoxButton.YesNo
                , MessageBoxImage.Warning
                );

            if (oResult == MessageBoxResult.Yes)
            {
                m_oCurrentBook.Words.Remove(oSelectedWord);
                m_oCurrentBook.Save();
                
                SearchWords();
            }
        }

        private void cmdAddWord_Click(object sender, RoutedEventArgs e)
        {
            m_oCurrentWord = new WordExtended();

            txtWord.Text = m_oCurrentWord.Value;
            txtPronunciation.Text = m_oCurrentWord.Pronunciation;
            txtTranslation.Text = m_oCurrentWord.Translation;
            // TODO:
            //txtPageLocation.Text = oSelectedWord.PageLocation;
            //txtExamples.Text = oSelectedWord.Examples;

            tcMenu.SelectedItem = tiAdd;
        }
        #endregion

        #region " Add Tab "
        private void cmdSaveWord_Click(object sender, RoutedEventArgs e)
        {
            if (m_oCurrentWord == null)
            {
                return;
            }

            //_Word oNewWord = new _Word();
            m_oCurrentWord.Value = txtWord.Text;
            m_oCurrentWord.Pronunciation = txtPronunciation.Text;
            m_oCurrentWord.Translation = txtTranslation.Text;

            //string sPageLocation = txtPageLocation.Text;
            //string[] arrLocations = sPageLocation.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (string item in arrLocations)
            //{
            //    string[] arrLocationParts = item.Split(new string[] { "(" }, StringSplitOptions.RemoveEmptyEntries);
            //    Location oLocation = new Location();
            //    oLocation.PageNbr = int.Parse(arrLocationParts[0]);
            //    oLocation.PageLocation = arrLocationParts[1].Substring(0, arrLocationParts[1].Length - 1);
            //}
            
            m_oCurrentWord.Save(m_oCurrentBookExtended.BookId);

            m_oCurrentWord.SaveLocation(
                m_oCurrentBookExtended.BookId
                , txtPageLocation.Text
                );
            //oNewWord.PageLocation = txtPageLocation.Text;
            //oNewWord.Examples = txtExamples.Text;

            //m_oCurrentBook.AddWord(oNewWord);
            //m_oCurrentBook.Save();

            //oNewWord.s

            tcMenu.SelectedItem = tiHome;
            SearchWords();
        }
        #endregion

        #region " Settings Tab "
        private void LoadSettingsTab()
        {
            txtBookPath.Text = m_oSettings.BookPath;
        }

        private void cmdSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            m_oSettings.BookPath = txtBookPath.Text;
            // TODO: Add other settings here before save.

            m_oSettings.Save();

            string sBookRootPath = GetBookRootPath();
            if (!Directory.Exists(sBookRootPath))
            {
                Directory.CreateDirectory(sBookRootPath);
            }
        }
        #endregion        
    }
}
