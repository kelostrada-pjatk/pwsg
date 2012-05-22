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
using System.ComponentModel;
using System.Windows.Media.Animation;

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
        ChompGameLogic.ChompGameLogic chocolate;
        int i, j;

        public Chocolates()
            : base()
        {

        }

        public void GenerateChocolates(int i, int j)
        {
            chocolate = new ChompGameLogic.ChompGameLogic(i,j);
            this.i = i;
            this.j = j;

            Clear();
            Rectangle nowy = new Rectangle();
            nowy.Fill = new SolidColorBrush(Colors.Red);
            nowy.StrokeThickness = 4;
            nowy.Stroke = Brushes.Chocolate;
            nowy.DataContext = chocolate.Chocolate[0];
            nowy.MouseLeftButtonDown += new MouseButtonEventHandler(nowy_MouseLeftButtonDown);
            nowy.MouseEnter += new MouseEventHandler(nowy_MouseEnter);
            nowy.MouseLeave += new MouseEventHandler(nowy_MouseLeave);
            Add(nowy);

            for (int k = 1; k < i * j ; k++)
            {
                nowy = new Rectangle();
                nowy.Fill = new SolidColorBrush(Colors.SaddleBrown);
                nowy.StrokeThickness = 4;
                nowy.Stroke = Brushes.Chocolate;
                nowy.DataContext = chocolate.Chocolate[k];
                nowy.MouseLeftButtonDown += new MouseButtonEventHandler(nowy_MouseLeftButtonDown);
                nowy.MouseEnter += new MouseEventHandler(nowy_MouseEnter);
                nowy.MouseLeave += new MouseEventHandler(nowy_MouseLeave);
                
                Add(nowy);
            }
        }

        void nowy_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).IsEaten)
            {
                ColorAnimation animate = new ColorAnimation();
                if (((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).IsPoisoned)
                    animate.To = Colors.Red;
                else
                    animate.To = Colors.SaddleBrown;

                animate.Duration = TimeSpan.FromSeconds(1);

                animate.AutoReverse = false;

                ((Rectangle)sender).Fill.BeginAnimation(SolidColorBrush.ColorProperty, animate);


            }
        }

        void nowy_MouseEnter(object sender, MouseEventArgs e)
        {
            
            if (!chocolate.IsLogicRunning && !((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).IsEaten)
            {
                /*
                ColorAnimation animate = new ColorAnimation();
                animate.To = Colors.LightGreen;
                animate.Duration = TimeSpan.FromSeconds(1);

                animate.AutoReverse = false;

                ((Rectangle)sender).Fill.BeginAnimation(SolidColorBrush.ColorProperty, animate);
                 */

                ColorAnimationUsingKeyFrames animation = new ColorAnimationUsingKeyFrames();

                animation.Duration = TimeSpan.FromSeconds(3.5);
                
                animation.AutoReverse = false;


                
                LinearColorKeyFrame key1 = new LinearColorKeyFrame(Colors.LightGreen, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)));
                //LinearColorKeyFrame key2 = new LinearColorKeyFrame(Colors.Gray, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)));
                animation.KeyFrames.Add(key1);
                animation.KeyFrames.Add(
                    new LinearColorKeyFrame(Colors.Red , KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2.5)))
                );

                ((Rectangle)sender).Fill.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            }
            
        }

        void nowy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (!chocolate.IsLogicRunning && !((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).IsEaten)
            {
                ((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).Eat();
                //MessageBox.Show("Zajadam sie - " + ((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).X + " " + ((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).Y);
            }

            
        }


    }





}
