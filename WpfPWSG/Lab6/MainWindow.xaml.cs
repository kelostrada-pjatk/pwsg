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

namespace Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Chocolates ch;

        public MainWindow()
        {
            InitializeComponent();

            ch = (Chocolates)FindResource("chocolates");
            ch.GenerateChocolates((int)slider1.Value, (int)slider2.Value);
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ch!=null)
                ch.GenerateChocolates((int)slider1.Value, (int)slider2.Value);
        }


    }

    public class Chocolates : ObservableCollection<Rectangle>
    {
        public Chocolates()
            : base()
        {

        }

        public void GenerateChocolates(int i, int j)
        {
            Clear();
            Rectangle nowy = new Rectangle();
            nowy.Fill = Brushes.Red;
            nowy.StrokeThickness = 4;
            nowy.Stroke = Brushes.Chocolate;
            Add(nowy);

            for (int k = 0; k < i * j - 1; k++)
            {
                nowy = new Rectangle();
                nowy.Fill = Brushes.SaddleBrown;
                nowy.StrokeThickness = 4;
                nowy.Stroke = Brushes.Chocolate;
                Add(nowy);
            }
        }
    }





}
