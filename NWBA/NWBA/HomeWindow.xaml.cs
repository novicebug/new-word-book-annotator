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
        #region Private Members
        private SettingExtended m_oSettingExtended;
        private BookExtended m_oCurrentBookExtended;
        private WordExtended m_oCurrentWord;
        #endregion

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

            m_oSettingExtended = new SettingExtended();
            m_oSettingExtended.Load();

            if (!m_oSettingExtended.LastBookId.HasValue)
            {
                tiHome.IsEnabled = false;
                tiAdd.IsEnabled = false;

                tcMenu.SelectedItem = tiSettings;
            }

            LoadHomeTab();
            LoadSettingsTab();
        }

        #region " Home Tab "
        private void LoadBookList()
        {
            ValueList oValueList = new ValueList();
            List<ValueItem> arrBooks = oValueList.GetBookList();
            
            this.BookList.Add(new ValueItem("---ALL---", 0));          
            foreach (ValueItem item in arrBooks)
            {
                this.BookList.Add(item);
            }

            lstBook.SelectedValue = m_oSettingExtended.LastBookId; 
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

                m_oSettingExtended.LastBookId = nBookId;
                m_oSettingExtended.Save();
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
            lblPageLocation.Text = "Page (Location): " + m_oCurrentWord.GetLocation(m_oCurrentBookExtended.BookId).PageLocation;
            // TODO:
            //lblExamples.Text = oSelectedWord.Examples;

            lblExamplesLabel.Visibility = Visibility.Visible;
        }

        private void cmdEditWord_Click(object sender, RoutedEventArgs e)
        {            
            if (lstMatchingWords.SelectedValue == null)
            {
                return;
            }

            txtWord.Text = m_oCurrentWord.Value;
            txtPronunciation.Text = m_oCurrentWord.Pronunciation;
            txtTranslation.Text = m_oCurrentWord.Translation;           
            txtPageLocation.Text = m_oCurrentWord.GetLocation(m_oCurrentBookExtended.BookId).PageLocation;
            // TODO: Examples
            //txtExamples.Text = oSelectedWord.Examples;

            tcMenu.SelectedItem = tiAdd;
        }
        
        private void cmdDeleteWord_Click(object sender, RoutedEventArgs e)
        {
            if (lstMatchingWords.SelectedValue == null)
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
                int nWordId = (int)lstMatchingWords.SelectedValue;
                m_oCurrentBookExtended.DeleteWord(nWordId);
                
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
            txtPageLocation.Text = "";
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
            
            m_oCurrentWord.Value = txtWord.Text;
            m_oCurrentWord.Pronunciation = txtPronunciation.Text;
            m_oCurrentWord.Translation = txtTranslation.Text;
            
            m_oCurrentWord.Save(
                m_oCurrentBookExtended.BookId
                , txtPageLocation.Text
                );
            // TODO: Examples
            //oNewWord.Examples = txtExamples.Text;

            tcMenu.SelectedItem = tiHome;
            SearchWords();
        }
        #endregion

        #region " Settings Tab "
        private void LoadSettingsTab()
        {
        }

        private void cmdSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add other settings here before save.

            //m_oSettings.Save();

            //string sBookRootPath = GetBookRootPath();
            //if (!Directory.Exists(sBookRootPath))
            //{
            //    Directory.CreateDirectory(sBookRootPath);
            //}
        }
        #endregion        
    }
}
