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

            m_arrBookList.Add(new ValueItem("test 1", 1));
            m_arrBookList.Add(new ValueItem("test 2", 2));
            m_arrBookList.Add(new ValueItem("test 3", 3));

            LoadSettingsTab();
        }

        private void lstBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtSearch.Text = ((ValueItem)lstBook.SelectedValue).ValueMember.ToString();
        }

        private string GetBookRootPath()
        {
            return Directory.GetCurrentDirectory() + Consts.DIRECTORY_SEPARATOR + Consts.BOOK_ROOT_PATH;
        }

        private void LoadBookList()
        {
            
        }

        #region " Settings Tab "
        private void cmdSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            m_oSettings.BookPath = txtBookPath.Text;

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
