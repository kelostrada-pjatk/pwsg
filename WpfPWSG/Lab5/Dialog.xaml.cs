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
using System.Windows.Shapes;

namespace Lab5
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        public int x, y;

        public Dialog()
        {
            InitializeComponent();
            button1.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.sizeX.Text, out x) && int.TryParse(this.sizeY.Text, out y))
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Nieprawidłowa wielkość kształtu");
            }
        }
    }
}
