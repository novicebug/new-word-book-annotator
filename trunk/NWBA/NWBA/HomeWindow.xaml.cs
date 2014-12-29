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
        private ObservableCollection<ValueItem> m_arrBookList = new ObservableCollection<ValueItem>();
        private Settings m_oSettings;

        public ObservableCollection<ValueItem> BookList
        {
            get
            {
                return m_arrBookList;
            }
            set
            {
                m_arrBookList = value;
            }
        }

        public HomeWindow()
        {
            InitializeComponent();

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

        private void lstBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtSearch.Text = ((ValueItem)lstBook.SelectedValue).ValueMember.ToString();

            if (((int)((ValueItem)lstBook.SelectedValue).ValueMember) == 0) // All
            {
                tiAdd.IsEnabled = false;                
            }
            else
            {
                tiAdd.IsEnabled = true;                
            }
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

            m_arrBookList.Add(new ValueItem("---ALL---", 0));

            foreach (string sBookFilePath in books)
            {
                Book oBook = new Book(sBookFilePath);

                m_arrBookList.Add(new ValueItem(oBook.Title, oBook.Id));
            }
        }

        private void LoadHomeTab()
        {
            LoadBookList();
        }
        #endregion

        #region " Settings Tab "
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

        private void LoadSettingsTab()
        {
            txtBookPath.Text = m_oSettings.BookPath;
        }
        #endregion
    }
}
