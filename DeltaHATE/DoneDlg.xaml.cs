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
using System.Windows.Shapes;

namespace DeltaHATE
{
    /// <summary>
    /// Interaction logic for DoneDlg.xaml
    /// </summary>
    public partial class DoneDlg : Window
    {
        public DoneDlg()
        {
            InitializeComponent();
        }

        private void extremely_rad_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
