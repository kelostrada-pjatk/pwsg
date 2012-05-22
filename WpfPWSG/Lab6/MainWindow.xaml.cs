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

    class ChocolatePropertiesConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Rectangle rect = parameter as Rectangle;
            ChompGameLogic.ChocolateBrick chocolate = rect.DataContext as ChompGameLogic.ChocolateBrick;

            if (chocolate.IsSelectedBrick && (bool)rect.Tag)
            {
                ColorAnimationUsingKeyFrames ani = new ColorAnimationUsingKeyFrames();
                ani.Duration = TimeSpan.FromSeconds(1);
                ani.KeyFrames.Add(
                    new LinearColorKeyFrame(Colors.Green, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5)))
                    );
                ani.KeyFrames.Add(
                    new LinearColorKeyFrame(Colors.Gray, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)))
                    );

                rect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ani);

                DoubleAnimation resize = new DoubleAnimation();
                resize.Duration = TimeSpan.FromSeconds(1);
                resize.To = 0;

                rect.BeginAnimation(Rectangle.StrokeThicknessProperty, resize);

                rect.Tag = false;
                
            }
            else if (chocolate.IsEaten)
            {
                ColorAnimation ani = new ColorAnimation();
                ani.Duration = TimeSpan.FromSeconds(1);
                ani.To = Colors.LightGray;

                rect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ani);

                DoubleAnimation resize = new DoubleAnimation();
                resize.Duration = TimeSpan.FromSeconds(1);
                resize.To = 0;

                rect.BeginAnimation(Rectangle.StrokeThicknessProperty, resize);
            }
            else
            {
                if ((rect.DataContext as ChompGameLogic.ChocolateBrick).X == 0 && (rect.DataContext as ChompGameLogic.ChocolateBrick).Y == 0)
                    rect.Fill = new SolidColorBrush(Colors.Red);
                else
                    rect.Fill = new SolidColorBrush(Colors.SaddleBrown);
                rect.Stroke = Brushes.Chocolate;

                DoubleAnimation resize = new DoubleAnimation();
                resize.Duration = TimeSpan.FromSeconds(0);
                resize.To = 4;

                rect.BeginAnimation(Rectangle.StrokeThicknessProperty, resize);

                rect.Tag = true;
            }


            return null;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
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
            chocolate.GameEnded += new EventHandler<ChompGameLogic.GameResultEventArgs>(chocolate_GameEnded);
            this.i = i;
            this.j = j;

            Clear();
            Rectangle nowy = new Rectangle();
            nowy.Fill = new SolidColorBrush(Colors.Red);
            nowy.StrokeThickness = 4;
            nowy.Stroke = Brushes.Chocolate;
            nowy.DataContext = chocolate.Chocolate[0];
            nowy.MouseLeftButtonDown += new MouseButtonEventHandler(nowy_MouseLeftButtonDown);
            nowy.Tag = true;
            Add(nowy);

            MultiBinding multiBinding = new MultiBinding();
            
            Binding myBinding = new Binding("IsEaten");
            myBinding.Source = chocolate.Chocolate[0];
            multiBinding.Bindings.Add(myBinding);
            myBinding = new Binding("IsSelected");
            myBinding.Source = chocolate.Chocolate[0];
            multiBinding.Bindings.Add(myBinding);
            multiBinding.ConverterParameter = nowy;
            multiBinding.Converter = new ChocolatePropertiesConverter();
            this[0].SetBinding(SolidColorBrush.TransformProperty, multiBinding);

            

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
                nowy.Tag = true;
                Add(nowy);

                multiBinding = new MultiBinding();

                myBinding = new Binding("IsEaten");
                myBinding.Source = chocolate.Chocolate[k];
                multiBinding.Bindings.Add(myBinding);
                myBinding = new Binding("IsSelected");
                myBinding.Source = chocolate.Chocolate[k];
                multiBinding.Bindings.Add(myBinding);
                multiBinding.ConverterParameter = nowy;
                multiBinding.Converter = new ChocolatePropertiesConverter();
                this[k].SetBinding(SolidColorBrush.TransformProperty, multiBinding);
                
            }
        }

        void chocolate_GameEnded(object sender, ChompGameLogic.GameResultEventArgs e)
        {
            if (e.DidPlayerWin)
                MessageBox.Show("Player wins");
            else
                MessageBox.Show("PC wins");

            chocolate.ResetGame();
        }

        void nowy_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!chocolate.IsLogicRunning && !((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).IsEaten)
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
                ColorAnimation animate = new ColorAnimation();
                animate.To = Colors.LightGreen;
                animate.Duration = TimeSpan.FromSeconds(1);

                animate.AutoReverse = false;

                ((Rectangle)sender).Fill.BeginAnimation(SolidColorBrush.ColorProperty, animate);
            }
        }

        void nowy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!chocolate.IsLogicRunning && !((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).IsEaten)
            {
                ((ChompGameLogic.ChocolateBrick)((Rectangle)sender).DataContext).Eat();
                Rectangle rect = (Rectangle)sender;
                
                ColorAnimation ani = new ColorAnimation();
                ani.Duration = TimeSpan.FromSeconds(1);
                ani.To = Colors.Green;
                
                rect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ani);

                DoubleAnimation resize = new DoubleAnimation();
                resize.Duration = TimeSpan.FromSeconds(1);
                resize.To = 0;

                rect.BeginAnimation(Rectangle.StrokeThicknessProperty, resize);
            }
        }


    }





}
