using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NWBA.Base
{
    public class ValueItem: INotifyPropertyChanged
    {
        private string m_sDisplayMember;
        private object m_oValueMember;

        public string DisplayMember
        {
            get
            {
                return m_sDisplayMember;
            }
            set
            {
                if (m_sDisplayMember != value)
                {
                    m_sDisplayMember = value;
                    NotifyPropertyChanged("DisplayMember");
                }
            }
        }

        public object ValueMember
        {
            get
            {
                return m_oValueMember;
            }
            set
            {
                if (m_oValueMember != value)
                {
                    m_oValueMember = value;
                    NotifyPropertyChanged("ValueMember");
                }
            }
        }

        public ValueItem(string sDisplayMember, object oValueMember)
        {
            this.DisplayMember = sDisplayMember;
            this.ValueMember = oValueMember;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String sPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(sPropertyName));
            }
        }
        #endregion
    }
}
