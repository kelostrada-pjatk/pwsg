using System;
using System.Collections.Generic;
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
using System.IO;
using System.Threading;

namespace PathFinderWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int radius = 12;

        public MainWindow()
        {
            InitializeComponent();
            //DataContext = new AsyncPathFinder(ref image);
        }

        private void loadImage_Click(object sender, RoutedEventArgs e)
        {

            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Image Files"; // Default file name
            dlg.Filter = "Image Files|*.jpg;*.gif;*.bmp;*.png;*.jpeg|All Files|*.*"; // Filter files by extension

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                BitmapImage source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(dlg.FileName);
                source.EndInit();

                imageCanvas.Width = source.Width;
                imageCanvas.Height = source.Height;

                image.Source = source;

                imageCanvas.Children.Remove(start);
                imageCanvas.Children.Remove(end);

                start = new Ellipse();
                start.Width = radius;
                start.Height = radius;

                Canvas.SetBottom(start, 0);

                start.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 200));

                imageCanvas.Children.Add(start);

                end = new Ellipse();
                end.Width = radius;
                end.Height = radius;

                Canvas.SetRight(end, 0);

                end.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 0, 0));

                imageCanvas.Children.Add(end);

            }

        }

        

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(start, e.GetPosition(image).X - radius / 2);
            Canvas.SetTop(start, e.GetPosition(image).Y - radius / 2);
            
            double x = Canvas.GetLeft(start);
            x += radius / 2;
            double y = Canvas.GetTop(start);
            y += radius / 2;
        }

        private void image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(end, e.GetPosition(image).X - radius / 2);
            Canvas.SetTop(end, e.GetPosition(image).Y - radius / 2);
        }

        

        private void findPath_Click(object sender, RoutedEventArgs e)
        {
            PriorityBinding priorityBinding = new PriorityBinding { FallbackValue = "0" };
            Binding superPrecise = new Binding("SuperPrecise") { IsAsync = true };
            Binding precise = new Binding("Precise") { IsAsync = true };
            Binding regular = new Binding("Regular") { IsAsync = true };
            Binding quick = new Binding("Quick") { IsAsync = true };
            Binding superQuick = new Binding("SuperQuick");
            priorityBinding.Bindings.Add(superPrecise);
            priorityBinding.Bindings.Add(precise);
            priorityBinding.Bindings.Add(regular);
            priorityBinding.Bindings.Add(quick);
            priorityBinding.Bindings.Add(superQuick);
            textBox.SetBinding(TextBox.TextProperty, priorityBinding);


            /*
            if (image.Source == null) return;
            System.Drawing.Bitmap b = new System.Drawing.Bitmap(BitmapImage2Bitmap(image.Source as BitmapImage));
            PathFinder.PathFinder pF = new PathFinder.PathFinder(b);
            IList<PathFinder.PathSegment> pathList = pF.FindPath(new System.Drawing.Point(0,0), new System.Drawing.Point(200, 200), PathFinder.PathFindingPrecision.SuperQuick);
            double length = 0;
            foreach (var p in pathList)
            {
                length += p.Length;
            }
            textBox.Text = length.ToString();
            */
        }
    }

    public class AsyncPathFinder
    {
        Image image;
        PathFinder.PathFinder pathFinder;

        public AsyncPathFinder(ref Image image)
        {
            this.image = image;
            pathFinder = new PathFinder.PathFinder(new System.Drawing.Bitmap(BitmapImage2Bitmap(image.Source as BitmapImage)));
        }

        public string SuperPrecise
        {
            get
            {
                Thread.Sleep(4000);
                return "SuperPrecise";
            }
            set
            {

            }
        }

        public string Precise
        {
            get
            {
                Thread.Sleep(3000);
                return "Precise";
            }
            set
            {

            }
        }

        public string Regular
        {
            get
            {
                Thread.Sleep(2000);
                return "Regular";
            }
            set
            {

            }
        }

        public string Quick
        {
            get
            {
                Thread.Sleep(1000);
                return "Quick";
            }
            set
            {

            }
        }

        public string SuperQuick
        {
            get
            {
                return "SuperQuick";
            }
            set
            {

            }
        }

        private System.Drawing.Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new System.Drawing.Bitmap(bitmap);
            }
        }
    }
}
