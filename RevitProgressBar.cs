using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIN_Joiner
{
    public class RevitProgressBar : INotifyPropertyChanged
    {

        public double Progress
        {
            get { return _Progress; }
            set
            {
                _Progress = value;
                OnPropertyChanged("Progress");
            }
        }
        private double _Progress { get; set; }




        public string CurrentElementNumber
        {
            get { return _CurrentElementNumber; }
            set
            {
                _CurrentElementNumber = value;
                OnPropertyChanged("CurrentElementNumber");
                OnPropertyChanged("_CurrentElementNumber");
            }
        }
        private string _CurrentElementNumber { get; set; }






        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
