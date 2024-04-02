using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Caro_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double inputValue = (double)value;
            double factor = (double)(parameter);
            return inputValue * factor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        // property changed
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        // game property
        bool playerTurn = true;
        int count = 0;
        int[,] arr;

        // UI property
        private double _heightUnit;
        private double _widthUnit;
        
        public double HeightUnit
        {
            get { return _heightUnit; }
            set
            {
                if (_heightUnit != value)
                {
                    _heightUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        public double WidthUnit
        {
            get { return _widthUnit; }
            set
            {
                if (_widthUnit != value)
                {
                    _widthUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _circleUnit;
        public double CircleUnit
        {
            get { return _circleUnit; }
            set
            {
                if ( _circleUnit != value)
                {
                    _circleUnit = value;
                    OnPropertyChanged();
                }
            }
        }


        // Main Window()
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            Loaded += MainWindow_Loaded;

        }

        // UI function
        private void UpdateUnits()
        {
            HeightUnit = grid.ActualHeight / grid.RowDefinitions.Count;
            WidthUnit = grid.ActualWidth / grid.ColumnDefinitions.Count;
            CircleUnit = Math.Min(WidthUnit, HeightUnit);
        }
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateUnits();
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            setGridLine(12, 12);
            UpdateUnits();
            arr = new int[12, 12];
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    arr[i,j] = -1;
                }
            }
        }

        private void setGridLine(int col_count, int row_count)
        {
            clearGrid();
            for (int i =0; i < col_count; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions.Add(column);
            }
            for (int i = 0; i < row_count; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                grid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < col_count; i++)
            {
                for (int j = 0; j < row_count; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Stroke = new SolidColorBrush(Colors.Black);
                    rectangle.Fill = new SolidColorBrush(Colors.Transparent);
                    rectangle.StrokeThickness = 0.2;
                    grid.Children.Add(rectangle);
                    Grid.SetColumn(rectangle,i);
                    Grid.SetRow(rectangle,j);
                }
            }

        }
        Ellipse setCircle()
        {
            Ellipse blueCircle = new Ellipse();
            blueCircle.Stretch = Stretch.Uniform;
           
            blueCircle.SetBinding(WidthProperty, new Binding("CircleUnit") { Source = this, Converter = new MultiplyConverter(), ConverterParameter = 0.8 });
            blueCircle.SetBinding(HeightProperty, new Binding("Width") { Source = blueCircle });
            blueCircle.Stroke = new SolidColorBrush(Colors.Blue);
            blueCircle.StrokeThickness = 3;

            return blueCircle;
        }
        Canvas setCross()
        {
            Canvas redCross = new Canvas();
            redCross.SetBinding(WidthProperty, new Binding("WidthUnit") { Source = this, Converter = new MultiplyConverter(), ConverterParameter = 0.8 });
            redCross.SetBinding(HeightProperty, new Binding("HeightUnit") { Source = this, Converter = new MultiplyConverter(), ConverterParameter = 0.8 });

            var Line1 = new Line();
            Line1.X1 = 0;
            Line1.Y1 = 0;
            Line1.SetBinding(Line.X2Property, new Binding("Width") { Source = redCross, });
            Line1.SetBinding(Line.Y2Property, new Binding("Height") { Source = redCross, });
            Line1.Stroke = new SolidColorBrush(Colors.Red);

            var Line2 = new Line();
            Line2.SetBinding(Line.X1Property, new Binding("Width") { Source = redCross, });
            Line2.Y1 = 0;
            Line2.X2 = 0;
            Line2.SetBinding(Line.Y2Property, new Binding("Height") { Source = redCross, });
            Line2.Stroke = new SolidColorBrush(Colors.Red);

            redCross.Children.Add(Line1);
            redCross.Children.Add(Line2);

            return redCross;
        }


        // Game logic function
        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                int row = -1;
                int column = -1;
                var coordinate = e.GetPosition(grid);
                row = (int)(coordinate.Y / HeightUnit);
                column = (int)(coordinate.X / WidthUnit);
                //MessageBox.Show(row + " " + column);
                if (arr[row, column] == -1 && arr[row, column] != 0 && arr[row, column] != 1)
                {
                    arr[row, column] = playerTurn == true ? 1 : 0;
                    count++;
                    if (playerTurn)
                    {
                        Ellipse blueCircle = setCircle();
                        Grid.SetRow(blueCircle, row);
                        Grid.SetColumn(blueCircle, column);
                        blueCircle.HorizontalAlignment = HorizontalAlignment.Center;
                        blueCircle.VerticalAlignment = VerticalAlignment.Center;
                        grid.Children.Add(blueCircle);
                    }
                    else
                    {
                        Canvas redCross = setCross();
                        Grid.SetRow(redCross, row);
                        Grid.SetColumn(redCross, column);
                        redCross.HorizontalAlignment = HorizontalAlignment.Center;
                        redCross.VerticalAlignment = VerticalAlignment.Center;
                        grid.Children.Add(redCross);
                    }
                    playerTurn = !playerTurn;
                    checkWinner();

                }

            }
        }
        private void checkWinner()
        {
       
        }
        private void winnerName(int value, string line)
        {
            string winner = "";
            if (value == 1)
            {
                winner = "O";
                MessageBox.Show(winner + " win! Win condition: " + line);
                restart();
            }
            else if (value == 0)
            {
                winner = "X";
                MessageBox.Show(winner + " win! Win condition: " + line);
                restart();
            }


        }

        // button logic function
        private void restart()
        {

            foreach (var element in grid.Children)
            {
                if (element is Ellipse ellipse || element is Canvas canvas)
                {
                    grid.Children.Remove((UIElement)element);
                }
            }

            // Reset arr to initial values
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    arr[i, j] = -1;
                }
            }
            count = 0;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void changeSizeButton_Click(object sender, RoutedEventArgs e)
        {
            SizeChangeWindow sizeChangeWindow = new SizeChangeWindow();
            sizeChangeWindow.SizeChanged += SizeChangeWindow_SizeChanged;
            sizeChangeWindow.ShowDialog();
        }

        private void SizeChangeWindow_SizeChanged(object sender, GetSizeChangeEvent e)
        {
            int numRows = e.NumRows;
            int numCols = e.NumColumns;

            setGridLine(numCols, numRows);

            arr = new int[numRows, numCols];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    arr[i, j] = -1;
                }
            }
        }
        private void restartButton_Click(object sender, RoutedEventArgs e)
        {
            restart();
        }

        
        private void clearGrid()
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }

       
    }


}