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
        private const int MAX_EXAMPLES_COUNT = 10;
        private const string DYNAMIC_EXAMPLE_TEXTBOX_NAME = "txtExample";
        private const string DYNAMIC_PRINT_EXAMPLE_CHECKBOX_NAME = "chkPrintExample";

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
            
            InitAddTabControls();

            this.BookList = new ObservableCollection<ValueItem>();
            this.MatchingWords = new ObservableCollection<Word>();

            LayoutRoot.DataContext = this;

            m_oSetting = new Setting();
            m_oSetting.Load();

            txtFontSize.Text = m_oSetting.FontSize.ToString();
            this.FontSize = m_oSetting.FontSize;

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

            LoadAddTabControls();

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

            LoadAddTabControls();

            tcMenu.SelectedItem = tiAdd;
        }

        private void InitializeWordControls(int nWordId)
        {
            m_oCurrentWord = new Word(nWordId);

            lblWord.Text = m_oCurrentWord.Value;
            lblPronunciation.Text = m_oCurrentWord.Pronunciation;
            lblTranslation.Text = m_oCurrentWord.Translation;
            lblPageLocationLabel.Visibility = Visibility.Visible;
            lblPageLocation.Text = m_oCurrentWord.GetPageLocation(m_oCurrentBook.BookId);
            
            lblExplationLabel.Visibility = Visibility.Visible;
            lblExplation.Text = m_oCurrentWord.Explanation;

            StringBuilder sbExamples = new StringBuilder();
            foreach (Example item in m_oCurrentWord.Examples)
            {
                sbExamples.AppendLine("- " + item.Value);
            }
            lblExamples.Text = sbExamples.ToString();
            lblExamplesLabel.Visibility = Visibility.Visible;  
        }
        #endregion

        #region " Add Tab "
        private void InitAddTabControls()
        {
            for (int nLoop = 1; nLoop <= MAX_EXAMPLES_COUNT; nLoop++)
            {
                RowDefinition oRowDefinition = new RowDefinition();
                oRowDefinition.Height = GridLength.Auto;
                gridExamples.RowDefinitions.Add(oRowDefinition);

                TextBlock lblExample = new TextBlock();                
                lblExample.Text = "Example " + nLoop.ToString() + ":";
                Grid.SetRow(lblExample, nLoop - 1);
                Grid.SetColumn(lblExample, 0);
                gridExamples.Children.Add(lblExample);

                TextBox txtExample = new TextBox();
                txtExample.Name = DYNAMIC_EXAMPLE_TEXTBOX_NAME + nLoop.ToString();
                txtExample.Margin = new Thickness(5, 5, 0, 0);
                Grid.SetRow(txtExample, nLoop - 1);
                Grid.SetColumn(txtExample, 1);
                gridExamples.Children.Add(txtExample);
                gridExamples.RegisterName(txtExample.Name, txtExample);

                CheckBox chkPrintExample = new CheckBox();
                chkPrintExample.Name = DYNAMIC_PRINT_EXAMPLE_CHECKBOX_NAME + nLoop.ToString();
                chkPrintExample.Margin = new Thickness(5, 10, 0, 0);
                Grid.SetRow(chkPrintExample, nLoop - 1);
                Grid.SetColumn(chkPrintExample, 2);
                gridExamples.Children.Add(chkPrintExample);
                gridExamples.RegisterName(chkPrintExample.Name, chkPrintExample);
            }                
        }

        private void LoadAddTabControls()
        {
            txtWord.Text = m_oCurrentWord.Value;
            txtPronunciation.Text = m_oCurrentWord.Pronunciation;
            txtTranslation.Text = m_oCurrentWord.Translation;
            txtPageLocation.Text = m_oCurrentWord.GetPageLocation(m_oCurrentBook.BookId);
            txtExplanation.Text = m_oCurrentWord.Explanation;

            for (int nLoop = 1; nLoop <= MAX_EXAMPLES_COUNT; nLoop++)
            {
                TextBox txtCurrentExample = (TextBox)tiAdd.FindName(DYNAMIC_EXAMPLE_TEXTBOX_NAME + nLoop.ToString());
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
            m_oCurrentWord.Explanation = txtExplanation.Text;

            m_oCurrentWord.AddLocation(
                m_oCurrentBook.BookId
                , txtPageLocation.Text
                );

            for (int nLoop = 1; nLoop <= MAX_EXAMPLES_COUNT; nLoop++)
            {
                TextBox txtCurrentExample = (TextBox)tiAdd.FindName(DYNAMIC_EXAMPLE_TEXTBOX_NAME + nLoop.ToString());

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
            double nFontSize;
            if (!double.TryParse(txtFontSize.Text, out nFontSize))
            {
                nFontSize = 12;
            }

            m_oSetting.FontSize = nFontSize;
            m_oSetting.Save();

            this.FontSize = nFontSize;
        }
        #endregion        
    }
}
