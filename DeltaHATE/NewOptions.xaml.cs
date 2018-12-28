using System.Windows;

namespace DeltaHATE
{
    /// <summary>
    /// Interaction logic for NewOptions.xaml
    /// </summary>
    public partial class NewOptions : Window
    {
        public NewOptions()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chk_kris.IsChecked = Values.DoKris;
            chalk_susie.IsChecked = Values.SuseiChalkScene;
            xcxcxcxc.IsChecked = Values.SantaLancer;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Values.DoKris = (bool)chk_kris.IsChecked;

            Values.SuseiChalkScene = (bool)chalk_susie.IsChecked;

            Values.SantaLancer = (bool)xcxcxcxc.IsChecked;

            //MainWindow ghgh = new MainWindow();

            //ghgh.button_corrupt.IsEnabled = true;

            this.Close();
        }
    }
}
