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
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeltaHATE
{
    /// <summary>
    /// Interaction logic for CustomException.xaml
    /// </summary>
    public partial class CustomException : Window
    {
        public string FileP = "Resources//exceptions.txt";

        public CustomException()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private int ReturnCheckMode(int indx)
        {
            string[] str = File.ReadAllLines(FileP, Encoding.UTF8);
            string[] sep = str[indx].Split(';');
            return Convert.ToInt32(sep[1]);
        }

        private int ReturnDetectMode(int indx)
        {
            string[] str = File.ReadAllLines(FileP, Encoding.UTF8);
            string[] sep = str[indx].Split(';');
            return Convert.ToInt32(sep[2]);
        }

        private void UpdateRadio()
        {
            switch (ReturnCheckMode(list.SelectedIndex))
            {
                case 0:
                    rb_startswith.IsChecked = true;
                    rb_contains.IsChecked = false;
                    rb_is.IsChecked = false;
                    break;

                case 1:
                    rb_startswith.IsChecked = false;
                    rb_contains.IsChecked = true;
                    rb_is.IsChecked = false;
                    break;

                case 2:
                    rb_startswith.IsChecked = false;
                    rb_contains.IsChecked = false;
                    rb_is.IsChecked = true;
                    break;
            }

            switch (ReturnDetectMode(list.SelectedIndex))
            {
                case 0:
                    rb_unst.IsChecked = true;
                    rb_stable.IsChecked = false;
                    rb_modeboth.IsChecked = false;
                    break;

                case 1:
                    rb_unst.IsChecked = false;
                    rb_stable.IsChecked = true;
                    rb_modeboth.IsChecked = false;
                    break;

                case 2:
                    rb_unst.IsChecked = false;
                    rb_stable.IsChecked = false;
                    rb_modeboth.IsChecked = true;
                    break;
            }
        }

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string[] str = File.ReadAllLines(FileP, Encoding.UTF8);
            string[] sep = str[list.SelectedIndex].Split(';');
            selected.Text = sep[0];

            UpdateRadio();
        }

        private void Windo_Loaded(object sender, RoutedEventArgs e)
        {
            string[] str = File.ReadAllLines(FileP, Encoding.UTF8);

            for(int i = 0; i < str.Length; i++)
            {
                string[] sep = str[i].Split(';');
                list.Items.Add(sep[0]);
            }
        }

        private void Addbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (addbox.Text != "")
            {
                bt_additem.IsEnabled = true;
            }
            else
            {
                bt_additem.IsEnabled = false;
            }
        }

        private void Bt_removesel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
