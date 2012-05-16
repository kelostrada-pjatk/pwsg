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
using System.Windows.Media.Effects;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Markup;
using System.Globalization;

namespace Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += canvasMouseMove;
            this.canvas.MouseLeftButtonDown += onClick;
            this.MouseLeftButtonUp += ReleaseShape;
            foreach (var c in panel.Children)
            {
                if (!(c is Control)) continue;
                (c as Control).IsEnabled = false;
            }
            PropertyInfo[] kolory = typeof(Colors).GetProperties();
            foreach (var k in kolory)
                if (k.Name != "Transparent")
                {
                    kolorWypelnienia.Items.Add(k.Name);
                    kolorRamki.Items.Add(k.Name);
                }
            
        }

        private Shape highlighted;
        public Shape Highlighted
        {
            get { return highlighted; }
            set
            {
                highlighted = value;
                panel.DataContext = highlighted;
                if (highlighted == null)
                {

                    foreach (var c in panel.Children)
                    {
                        if (!(c is Control)) continue;
                        (c as Control).IsEnabled = false;
                        if (c is RadioButton)
                            (c as RadioButton).IsChecked = false;
                    }
                    usun.IsEnabled = false;
                    if (clipper == null)
                        obcinanie.IsEnabled = false;
                }
                else
                {
                    usun.IsEnabled = true;
                    obcinanie.IsEnabled = true;
                }
            }

        }
        private Shape dragged;
        private Shape clipper;
        private Point pointClicked = new Point();
        private int zIndex = 0;

        private List<Shape> highlights = new List<Shape>();
        private List<Shape> drags = new List<Shape>();


        /// <summary>
        /// Dodaj MenuItem
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Dialog aaa = new Dialog();
            aaa.Owner = this;
            
            aaa.ShowDialog();
            if (aaa.DialogResult==true)
            {
                Random Randomize = new Random();

                PropertyInfo[] kolory = typeof(Colors).GetProperties();
                PropertyInfo kolor = kolory[Randomize.Next(kolory.Length)];
                while (kolor.Name == "Transparent")
                    kolor = kolory[Randomize.Next(kolory.Length)];
                
                if (aaa.ellipse.IsChecked==true)
                {
                    Ellipse nowa = new Ellipse();
                    
                    nowa.Width = double.Parse(aaa.sizeX.Text);
                    nowa.Height = double.Parse(aaa.sizeY.Text);
                    nowa.Fill = new SolidColorBrush((Color)kolor.GetValue(null,null));
                    kolor = kolory[Randomize.Next(kolory.Length)];
                    while (kolor.Name == "Transparent")
                        kolor = kolory[Randomize.Next(kolory.Length)];
                    nowa.Stroke = new SolidColorBrush((Color)kolor.GetValue(null, null));
                    nowa.StrokeThickness = 0;
                    nowa.RenderTransform = new RotateTransform(0, nowa.Width / 2, nowa.Height / 2);

                    nowa.Tag = new ImageBrush(WhiteBmp());

                    nowa.MouseLeftButtonDown += shapeMouseLeftButtonDown;
                    nowa.MouseRightButtonDown += shapeMouseRightButtonDown;

                    if (int.Parse(aaa.sizeX.Text) < (int)canvas.ActualWidth && int.Parse(aaa.sizeY.Text) < (int)canvas.ActualHeight)
                    {
                        Canvas.SetLeft(nowa, Randomize.Next((int)canvas.ActualWidth - int.Parse(aaa.sizeX.Text)));
                        Canvas.SetTop(nowa, Randomize.Next((int)canvas.ActualHeight - int.Parse(aaa.sizeY.Text)));
                    }
                    canvas.Children.Add(nowa);
                }
                else if (aaa.rectangle.IsChecked == true)
                {
                    Rectangle nowa = new Rectangle();
                    nowa.Width = double.Parse(aaa.sizeX.Text);
                    nowa.Height = double.Parse(aaa.sizeY.Text);
                    nowa.Fill = new SolidColorBrush((Color)kolor.GetValue(null, null));
                    kolor = kolory[Randomize.Next(kolory.Length)];
                    while (kolor.Name == "Transparent")
                        kolor = kolory[Randomize.Next(kolory.Length)];
                    nowa.Stroke = new SolidColorBrush((Color)kolor.GetValue(null, null));
                    nowa.StrokeThickness = 0;
                    nowa.RenderTransform = new RotateTransform(0, nowa.Width / 2, nowa.Height / 2);

                    nowa.Tag = new ImageBrush(WhiteBmp());

                    nowa.MouseLeftButtonDown += shapeMouseLeftButtonDown;
                    nowa.MouseRightButtonDown += shapeMouseRightButtonDown;
                    
                    if (int.Parse(aaa.sizeX.Text) < (int)canvas.ActualWidth && int.Parse(aaa.sizeY.Text) < (int)canvas.ActualHeight)
                    {
                        Canvas.SetLeft(nowa, Randomize.Next((int)canvas.ActualWidth - int.Parse(aaa.sizeX.Text)));
                        Canvas.SetTop(nowa, Randomize.Next((int)canvas.ActualHeight - int.Parse(aaa.sizeY.Text)));
                    }
                    canvas.Children.Add(nowa);
                }

            }
        }

        /// <summary>
        /// Usuń MenuItem
        /// </summary>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(highlighted);
            Highlighted = null;
        }

        /// <summary>
        /// Zamknij MenuItem
        /// </summary>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Obcinanie MenuItem
        /// </summary>
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuItem).IsChecked == true)
            {
                RotateTransform rt = new RotateTransform(((RotateTransform)highlighted.RenderTransform).Angle, Canvas.GetLeft(highlighted) + ((RotateTransform)highlighted.RenderTransform).CenterX, Canvas.GetTop(highlighted) + ((RotateTransform)highlighted.RenderTransform).CenterY);
                if (highlighted is Rectangle)
                {
                    RectangleGeometry nowy = new RectangleGeometry(new Rect(Canvas.GetLeft(highlighted), Canvas.GetTop(highlighted), highlighted.Width, highlighted.Height), 0, 0, rt);
                    canvas.Clip = nowy;
                }
                else if (highlighted is Ellipse)
                {
                    EllipseGeometry nowa = new EllipseGeometry(new Point(Canvas.GetLeft(highlighted) + highlighted.Width / 2, Canvas.GetTop(highlighted) + highlighted.Height / 2), highlighted.Width / 2, highlighted.Height / 2, rt);
                    canvas.Clip = nowa;
                }
                clipper = highlighted;
                canvas.Children.Remove(highlighted);
                highlights.Clear();
                Highlighted = null;
            }
            else
            {
                foreach (var h in highlights)
                    h.Effect = (DropShadowEffect)FindResource("null");
                highlights.Clear();                    
                canvas.Clip = null;
                Highlighted = clipper;
                canvas.Children.Add(clipper);
                clipper = null;
                Canvas.SetZIndex(highlighted, ++zIndex);
                highlights.Add(highlighted);
            }
        }

        /// <summary>
        /// Event Handler dla klikania po canvasie. 
        /// Powoduje niwelowanie podświetlenia i dezaktywacje Shape'a.
        /// </summary>
        private void onClick(object sender, MouseEventArgs e)
        {
            IInputElement uie = canvas.InputHitTest(new Point(e.GetPosition(canvas).X, e.GetPosition(canvas).Y));
            if (uie == canvas && highlights.Count > 0)
            {
                foreach (var h in highlights)
                    h.Effect = (DropShadowEffect)FindResource("null");
                highlights.Clear();

                Highlighted = null;

                panel.DataContext = null;
            }
        }

        /// <summary>
        /// Event Handler dla ruszania myszką po canvasie.
        /// Potrzebny do przesuwania Shape'ów.
        /// </summary>
        private void canvasMouseMove(object sender, MouseEventArgs e)
        {
            if (dragged != null)
            {
                double topbeg = Canvas.GetTop(dragged);
                double leftbeg = Canvas.GetLeft(dragged);

                Canvas.SetTop(dragged,Mouse.GetPosition(canvas).Y - pointClicked.Y);
                Canvas.SetLeft(dragged,Mouse.GetPosition(canvas).X - pointClicked.X);

                topbeg = Canvas.GetTop(dragged) - topbeg;
                leftbeg = Canvas.GetLeft(dragged) - leftbeg;

                foreach (var d in drags)
                {
                    Canvas.SetTop(d, Canvas.GetTop(d) + topbeg);
                    Canvas.SetLeft(d,  Canvas.GetLeft(d) + leftbeg);
                }
            }
        }

        /// <summary>
        /// Kliknięcie myszką na Shape'a.
        /// Dezaktywacja starego Shape'a, aktywowanie przesuwania i podświetlenia.
        /// Ponadto ustawianie pozycji zIndex.
        /// </summary>
        private void shapeMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (highlights.Count < 2)
            {
                if (highlighted != null)
                    highlighted.Effect = (DropShadowEffect)FindResource("null");
                highlights.Clear();

                Highlighted = (Shape)sender;
                dragged = (Shape)sender;
                highlights.Add(highlighted);
                pointClicked.X = e.GetPosition(dragged).X;
                pointClicked.Y = e.GetPosition(dragged).Y;
                pointClicked = ((RotateTransform)dragged.RenderTransform).Transform(pointClicked);
                Mouse.Capture(highlighted);

                highlighted.Effect = (DropShadowEffect)FindResource("shadow");
                // Na wypadek gdyby zIndex osiągnęło wartość maksymalną.
                if (zIndex == Int16.MaxValue - 1)
                {
                    zIndex = 0;
                    foreach (Shape c in canvas.Children)
                        Canvas.SetZIndex(c, 0);
                }
                Canvas.SetZIndex(highlighted, ++zIndex);

                foreach (var c in panel.Children)
                {
                    if (!(c is Control)) continue;
                    (c as Control).IsEnabled = true;
                }

                if (highlighted.Tag is ImageBrush)
                {
                    wypelnienieKolor.IsChecked = true;
                    kolorWypelnienia.IsEnabled = true;
                    wyborObrazu.IsEnabled = false;
                    SetComboBoxBinding("Fill");
                }
                else
                {
                    wypelnienieObraz.IsChecked = true;
                    kolorWypelnienia.IsEnabled = false;
                    wyborObrazu.IsEnabled = true;
                    SetComboBoxBinding("Tag");
                }
            }
            else
            {
                if (highlights.Contains(sender as Shape))
                {
                    foreach (var h in highlights)
                        if ((Shape)sender != h)
                            drags.Add(h);
                    dragged = (Shape)sender;
                    pointClicked.X = e.GetPosition(dragged).X;
                    pointClicked.Y = e.GetPosition(dragged).Y;
                    pointClicked = ((RotateTransform)dragged.RenderTransform).Transform(pointClicked);
                    Mouse.Capture(dragged);
                    if (zIndex == Int16.MaxValue - 1)
                    {
                        zIndex = 0;
                        foreach (Shape c in canvas.Children)
                            Canvas.SetZIndex(c, 0);
                    }
                    Canvas.SetZIndex(dragged, ++zIndex);
                }
                else
                {
                    foreach (var h in highlights)
                        h.Effect = (DropShadowEffect)FindResource("null");
                    highlights.Clear();
                    
                    shapeMouseLeftButtonDown(sender, e);
                }
            }
        }

        private void shapeMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (highlights.Count == 0)
            {
                if (highlighted != null)
                    highlighted.Effect = (DropShadowEffect)FindResource("null");

                Highlighted = (Shape)sender;
                highlights.Add(highlighted);

                highlighted.Effect = (DropShadowEffect)FindResource("shadow");
                // Na wypadek gdyby zIndex osiągnęło wartość maksymalną.
                if (zIndex == Int16.MaxValue - 1)
                {
                    zIndex = 0;
                    foreach (Shape c in canvas.Children)
                        Canvas.SetZIndex(c, 0);
                }
                Canvas.SetZIndex(highlighted, ++zIndex);

                foreach (var c in panel.Children)
                {
                    if (!(c is Control)) continue;
                    (c as Control).IsEnabled = true;
                }

                if (highlighted.Tag is ImageBrush)
                {
                    wypelnienieKolor.IsChecked = true;
                    kolorWypelnienia.IsEnabled = true;
                    wyborObrazu.IsEnabled = false;
                    SetComboBoxBinding("Fill");
                }
                else
                {
                    wypelnienieObraz.IsChecked = true;
                    kolorWypelnienia.IsEnabled = false;
                    wyborObrazu.IsEnabled = true;
                    SetComboBoxBinding("Tag");
                }
            }
            else
            {
                if (!highlights.Contains((Shape)sender))
                {
                    if (highlighted != null)
                    {
                        if (!highlights.Contains(highlighted))
                            highlights.Add(highlighted);
                        Highlighted = null;
                    }

                    highlights.Add(sender as Shape);
                    ((Shape)sender).Effect = (DropShadowEffect)FindResource("shadow");

                    // Na wypadek gdyby zIndex osiągnęło wartość maksymalną.
                    if (zIndex == Int16.MaxValue - 1)
                    {
                        zIndex = 0;
                        foreach (Shape c in canvas.Children)
                            Canvas.SetZIndex(c, 0);
                    }
                    Canvas.SetZIndex((Shape)sender, ++zIndex);
                }
                else
                {
                    ((Shape)sender).Effect = (DropShadowEffect)FindResource("null");
                    if (zIndex == Int16.MaxValue - 1)
                    {
                        zIndex = 0;
                        foreach (Shape c in canvas.Children)
                            Canvas.SetZIndex(c, 0);
                    }
                    Canvas.SetZIndex((Shape)sender, ++zIndex);

                    highlights.Remove((Shape)sender);

                    if (highlights.Count == 1)
                    {
                        Highlighted = highlights[0];
                        foreach (var c in panel.Children)
                        {
                            if (!(c is Control)) continue;
                            (c as Control).IsEnabled = true;
                        }

                        if (highlighted.Tag is ImageBrush)
                        {
                            wypelnienieKolor.IsChecked = true;
                            kolorWypelnienia.IsEnabled = true;
                            wyborObrazu.IsEnabled = false;
                            SetComboBoxBinding("Fill");
                        }
                        else
                        {
                            wypelnienieObraz.IsChecked = true;
                            kolorWypelnienia.IsEnabled = false;
                            wyborObrazu.IsEnabled = true;
                            SetComboBoxBinding("Tag");
                        }
                    }

                }
            }
        }

        private void wypelnienieKolor_Checked(object sender, RoutedEventArgs e)
        {
            kolorWypelnienia.IsEnabled = true;
            wyborObrazu.IsEnabled = false;
            if (highlighted.Fill is ImageBrush)
            {
                ImageBrush ib = (ImageBrush)highlighted.Fill;
                highlighted.Fill = (SolidColorBrush)highlighted.Tag;
                highlighted.Tag = ib;

                SetComboBoxBinding("Fill");
                
            }
        }

        private void wypelnienieObraz_Checked(object sender, RoutedEventArgs e)
        {
            kolorWypelnienia.IsEnabled = false;
            wyborObrazu.IsEnabled = true;
            if (highlighted.Tag is ImageBrush)
            {
                ImageBrush ib = (ImageBrush)highlighted.Tag;
                highlighted.Tag = highlighted.Fill;
                highlighted.Fill = ib;

                SetComboBoxBinding("Tag");
                
            }
        }

        /// <summary>
        /// Kiedy myszka zostaje zwolniona. Event handler dla canvasa.
        /// </summary>
        private void ReleaseShape(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            dragged = null;
            drags.Clear();
        }

        

        private void wyborObrazu_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Image Files"; // Default file name
            dlg.Filter = "Image Files|*.jpg;*.bmp;*.png;*.jpeg|All Files|*.*"; // Filter files by extension

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

                ImageBrush ib = new ImageBrush(source);
                highlighted.Fill = ib;
            }
        }

        private BitmapSource WhiteBmp()
        {
            BitmapSource bmpSource;
            double dpi = 96;
            int width = 1;
            int height = 1;
            byte[] pixelData = new byte[width * height];
            pixelData[0] = (byte)255;
            bmpSource = BitmapSource.Create(width, height, dpi, dpi, PixelFormats.Gray8, null, pixelData, width);
            return bmpSource;
        }

        private void SetComboBoxBinding(String Path)
        {
            Binding old = kolorWypelnienia.GetBindingExpression(ComboBox.TextProperty).ParentBinding;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(Path);
            binding.Mode = old.Mode;
            binding.UpdateSourceTrigger = old.UpdateSourceTrigger;
            binding.Converter = old.Converter;
            kolorWypelnienia.SetBinding(ComboBox.TextProperty, binding);
        }
        
        private void szerokosc_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (highlighted!=null)
                ((RotateTransform)highlighted.RenderTransform).CenterX = highlighted.Width / 2;
        }

        private void wysokosc_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (highlighted != null)
                ((RotateTransform)highlighted.RenderTransform).CenterY = highlighted.Height / 2;
        }        

    }


    [ValueConversion(typeof(Brush), typeof(String))]
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            Brush brush = (Brush)value;
            PropertyInfo[] kolory = typeof(Colors).GetProperties();
            foreach (var k in kolory)
                if (k.GetValue(null, null).ToString() == brush.ToString())
                    return k.Name;

            return brush.ToString();
        }

        public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            string strVal = value as string;
            PropertyInfo[] kolory = typeof(Colors).GetProperties();
            foreach (var k in kolory)
                if (k.Name == strVal)
                    return new SolidColorBrush((Color)k.GetValue(null, null));

            return new SolidColorBrush((Color)kolory[0].GetValue(null, null));
        }
    }


}
