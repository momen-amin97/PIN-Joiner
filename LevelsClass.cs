using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIN_Joiner
{
    public class LevelsClass :INotifyPropertyChanged
    {
        public Document doc;
        public Level level;
        public List<LevelsClass> levelsNames = new List<LevelsClass>();

        private bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked ; }
            set
            {
                if (value == _IsChecked)
                    return;

                _IsChecked = value;

                this.OnPropertyChanged("IsChecked");

            }
        }


        




        public string Name { get; set; }

        public LevelsClass(Document document, string linkName)
        {
            this.doc = document; 
            this.Name = linkName;
       
            

        }

        public LevelsClass()
        {
        }


        public List<LevelsClass> GetLevelsNames(Document doc , List<Element> levels)
        {
            foreach (Level level in levels)
            {
                levelsNames.Add(new LevelsClass(doc,level.Name));
            }
            return levelsNames;
        }
        


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
