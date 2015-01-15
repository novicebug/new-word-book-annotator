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
        private const int MAX_EXAMPLES_COUNT = 6;

        private Setting m_oSetting;
        private Book m_oCurrentBook;
        private Word m_oCurrentWord;
        #endregion

        #region " Properties "
        public ObservableCollection<ValueItem> BookList { get; set; }
        public ObservableCollection<Word> MatchingWords { get; set; }
        #endregion

        public HomeWindow()
        {
            InitializeComponent();

            this.BookList = new ObservableCollection<ValueItem>();
            this.MatchingWords = new ObservableCollection<Word>();

            LayoutRoot.DataContext = this;

            m_oSetting = new Setting();
            m_oSetting.Load();

            if (!m_oSetting.LastBookId.HasValue)
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

            lstBook.SelectedValue = m_oSetting.LastBookId; 
        }

        private void LoadHomeTab()
        {
            LoadBookList();
        }

        private void SearchWords()
        {
            this.MatchingWords.Clear();

            if (m_oCurrentBook.IsNewBookIn)
            {
                return;
            }

            m_oCurrentBook.FilterWords(txtSearch.Text);
                       
            foreach (Word item in m_oCurrentBook.Words)
            {
                this.MatchingWords.Add(item);
            }

            if (m_oCurrentWord != null)
            {
                lstMatchingWords.SelectedValue = m_oCurrentWord.WordId;
                InitializeWordControls(m_oCurrentWord.WordId);
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

                Book oBook = new Book();
                oBook.Load(nBookId);
                m_oCurrentBook = oBook;

                SearchWords();

                m_oSetting.LastBookId = nBookId;
                m_oSetting.Save();
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
            
            InitializeWordControls(nWordId);      
        }

        private void cmdEditWord_Click(object sender, RoutedEventArgs e)
        {            
            if (lstMatchingWords.SelectedValue == null)
            {
                return;
            }

            InitializeAddTabControls();

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
                m_oCurrentBook.DeleteWord(nWordId);
                
                SearchWords();
            }
        }

        private void cmdAddWord_Click(object sender, RoutedEventArgs e)
        {
            m_oCurrentWord = new Word();

            InitializeAddTabControls();

            tcMenu.SelectedItem = tiAdd;
        }

        private void InitializeWordControls(int nWordId)
        {
            m_oCurrentWord = new Word(nWordId);

            lblWord.Text = m_oCurrentWord.Value;
            lblPronunciation.Text = m_oCurrentWord.Pronunciation;
            lblTranslation.Text = m_oCurrentWord.Translation;
            lblPageLocation.Text = "Page (Location): " + m_oCurrentWord.GetPageLocation(m_oCurrentBook.BookId);

            StringBuilder sbExamples = new StringBuilder();
            foreach (Example item in m_oCurrentWord.Examples)
            {
                sbExamples.AppendLine(item.Value);
            }
            lblExamples.Text = sbExamples.ToString();
            lblExamplesLabel.Visibility = Visibility.Visible;  
        }
        #endregion

        #region " Add Tab "
        private void InitializeAddTabControls()
        {
            txtWord.Text = m_oCurrentWord.Value;
            txtPronunciation.Text = m_oCurrentWord.Pronunciation;
            txtTranslation.Text = m_oCurrentWord.Translation;
            txtPageLocation.Text = m_oCurrentWord.GetPageLocation(m_oCurrentBook.BookId);

            for (int nLoop = 1; nLoop <= MAX_EXAMPLES_COUNT; nLoop++)
            {
                TextBox txtCurrentExample = (TextBox)tiAdd.FindName("txtExample" + nLoop.ToString());
                txtCurrentExample.Text = m_oCurrentWord.GetExampleValue(nLoop);
            }
        }

        private void cmdSaveWord_Click(object sender, RoutedEventArgs e)
        {
            if (m_oCurrentWord == null)
            {
                return;
            }
            
            m_oCurrentWord.Value = txtWord.Text;
            m_oCurrentWord.Pronunciation = txtPronunciation.Text;
            m_oCurrentWord.Translation = txtTranslation.Text;

            m_oCurrentWord.AddLocation(
                m_oCurrentBook.BookId
                , txtPageLocation.Text
                );

            for (int nLoop = 1; nLoop <= MAX_EXAMPLES_COUNT; nLoop++)
            {
                TextBox txtCurrentExample = (TextBox)tiAdd.FindName("txtExample" + nLoop.ToString());

                m_oCurrentWord.AddExample(
                    txtCurrentExample.Text
                    , false
                    , nLoop
                    );
            }

            m_oCurrentWord.SaveInBook(m_oCurrentBook.BookId);

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
