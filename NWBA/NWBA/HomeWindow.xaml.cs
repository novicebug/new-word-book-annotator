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
using NWBA.Base;

namespace NWBA
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        private Settings m_oSettings;
        private Book m_oCurrentBook;

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
            string sBookRootPath = GetBookRootPath();
            string[] books = Directory.GetFiles(sBookRootPath);

            this.BookList.Add(new ValueItem("---ALL---", 0));

            foreach (string sBookFilePath in books)
            {
                Book oBook = new Book(sBookFilePath);

                this.BookList.Add(new ValueItem(oBook.Title, oBook.Id));
            }

            lstBook.SelectedIndex = 0;
        }

        private void LoadHomeTab()
        {
            LoadBookList();
        }

        private void lstBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nBookId = (int)((ValueItem)lstBook.SelectedValue).ValueMember;

            txtSearch.Text = nBookId.ToString();

            if (nBookId == 0) // All
            {
                tiAdd.IsEnabled = false;
            }
            else
            {
                tiAdd.IsEnabled = true;
                m_oCurrentBook = new Book(GetBookRootPath(), nBookId);
            }
        }

        private void cmdSearch_Click(object sender, RoutedEventArgs e)
        {
            List<Word> arrWords = m_oCurrentBook.FindMatchingWords(txtSearch.Text);

            this.MatchingWords.Clear();
            foreach (Word item in arrWords)
            {
                this.MatchingWords.Add(item);
            }
        }

        private void lstMatchingWords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object a = lstMatchingWords.SelectedValue;
        }
        #endregion

        #region " Add Tab "
        private void cmdSaveWord_Click(object sender, RoutedEventArgs e)
        {

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

            m_oSettings.SaveSettings();

            string sBookRootPath = GetBookRootPath();
            if (!Directory.Exists(sBookRootPath))
            {
                Directory.CreateDirectory(sBookRootPath);
            }
        }
        #endregion
    }
}
