using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using System.Windows.Shapes;

namespace PIN_Joiner
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window
    {
        Document document;
        public RevitProgressBar revitProgressViewer { get; set; }
        public ProgressBar( Document document)
        {
            this.document = document;
            InitializeComponent();
            revitProgressViewer = new RevitProgressBar();
            this.DataContext = revitProgressViewer;

            CloseBtn.IsEnabled = true;
            SaveBtn.IsEnabled = true;





        }

        

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            document.Save();    
        }
    }
}
