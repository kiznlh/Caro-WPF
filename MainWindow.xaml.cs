using System;
using System.ComponentModel;

using System.Globalization;
using System.Media;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
    public enum CellState
    {
        Empty,
        X,
        O
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
        CellState Player = CellState.O;
        int moveCount = 0;
        CellState[,] board;

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
        public int rowsCount { get; set; } = 12;
        public int columnsCount { get; set; } = 12;

        // Music property
        

        // Main Window()
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            Loaded += MainWindow_Loaded;

            grid.Focus();

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
            setGridLine(rowsCount, columnsCount);
            UpdateUnits();
            board = new CellState[rowsCount, columnsCount];
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    board[i,j] = CellState.Empty;
                }
            }
          
            //play music
            climaxTheme.Stop();
            keycardTheme.Stop();
            battleTheme.Play();

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
                    rectangle.MouseEnter += cell_MouseEnter;
                    rectangle.MouseLeave += cell_MouseLeave;
                    
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
            //blueCircle.Opacity = 0.1;

            
            return blueCircle;
        }
        Ellipse setHoverCircle()
        {
            Ellipse blueCircle = new Ellipse();
            blueCircle.Stretch = Stretch.Uniform;

            blueCircle.SetBinding(WidthProperty, new Binding("CircleUnit") { Source = this, Converter = new MultiplyConverter(), ConverterParameter = 0.8 });
            blueCircle.SetBinding(HeightProperty, new Binding("Width") { Source = blueCircle });
            blueCircle.Stroke = new SolidColorBrush(Colors.Blue);
            blueCircle.StrokeThickness = 3;
            blueCircle.Opacity = 0.3;


            ApplyBlinkingAnimation(blueCircle);
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
            Line1.StrokeThickness = 2;
            var Line2 = new Line();
            Line2.SetBinding(Line.X1Property, new Binding("Width") { Source = redCross, });
            Line2.Y1 = 0;
            Line2.X2 = 0;
            Line2.SetBinding(Line.Y2Property, new Binding("Height") { Source = redCross, });
            Line2.Stroke = new SolidColorBrush(Colors.Red);
            Line2.StrokeThickness = 2;


            redCross.Children.Add(Line1);
            redCross.Children.Add(Line2);

            return redCross;
        }
        Canvas setHoverCross()
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
            Line1.StrokeThickness = 2;
            var Line2 = new Line();
            Line2.SetBinding(Line.X1Property, new Binding("Width") { Source = redCross, });
            Line2.Y1 = 0;
            Line2.X2 = 0;
            Line2.SetBinding(Line.Y2Property, new Binding("Height") { Source = redCross, });
            Line2.Stroke = new SolidColorBrush(Colors.Red);
            Line2.StrokeThickness = 2;

            redCross.Children.Add(Line1);
            redCross.Children.Add(Line2);

            ApplyBlinkingAnimation(redCross);
            return redCross;
        }
        private void ApplyBlinkingAnimation(UIElement element)
        {
            // Create a storyboard
            Storyboard storyboard = new Storyboard();
            storyboard.RepeatBehavior = RepeatBehavior.Forever;

            // Create the opacity animations
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0.0;
            opacityAnimation.To = 0.5;
            opacityAnimation.Duration = TimeSpan.FromSeconds(1);
            opacityAnimation.AutoReverse = true;

            // Set the target property
            Storyboard.SetTarget(opacityAnimation, element);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));

            // Add the opacity animation to the storyboard
            storyboard.Children.Add(opacityAnimation);

            // Begin the storyboard
            storyboard.Begin();
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
                if (board[row, column] == CellState.Empty)
                {
                    board[row, column] = Player;
                    if (Player == CellState.O)
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
             
                    if (CheckWinner(row, column, Player))
                    {
                        winnerName2(Player);
                        
                        return;
                    }
                    moveCount++;
                    checkToChangeMusic();
                    Player = Player == CellState.O ? CellState.X : CellState.O;
                }

            }
        }


        // Hover property
        public int pointerRow { get; set; } = 0;
        public int pointerColumn { get; set; } = 0;

        public UIElement? pointerElement { get; set; }
        // Hover animation
        private void cell_MouseEnter(object sender, MouseEventArgs e)
        {
           
            Rectangle rectangle = sender as Rectangle;

            pointerRow = Grid.GetRow(rectangle);
            pointerColumn = Grid.GetColumn(rectangle);
            if (board[pointerRow, pointerColumn] == CellState.Empty)
            {
                if (Player == CellState.O)
                {
                    Ellipse blueCircle = setHoverCircle();
                    Grid.SetRow(blueCircle, pointerRow);
                    Grid.SetColumn(blueCircle, pointerColumn);
                    blueCircle.HorizontalAlignment = HorizontalAlignment.Center;
                    blueCircle.VerticalAlignment = VerticalAlignment.Center;
                    pointerElement = blueCircle;
                    grid.Children.Add(blueCircle);
                }
                else
                {
                    Canvas redCross = setHoverCross();
                    Grid.SetRow(redCross, pointerRow);
                    Grid.SetColumn(redCross, pointerColumn);
                    redCross.HorizontalAlignment = HorizontalAlignment.Center;
                    redCross.VerticalAlignment = VerticalAlignment.Center;
                    pointerElement = redCross;
                    grid.Children.Add(redCross);
                }
   
            }
        }

        private void cell_MouseLeave(object sender, MouseEventArgs e)
        {
            if (pointerElement != null && grid.Children.Contains(pointerElement))
            {
                grid.Children.Remove(pointerElement);
                pointerElement = null;
            }
        }



        //
        public bool CheckWinner(int row, int col, CellState player)
        {
            int[][] directions =
            [
                new int[] { 1, 0 }, // Horizontal
                new int[] { 0, 1 }, // Vertical
                new int[] { 1, 1 }, // Diagonal /
                new int[] { 1, -1 } // Diagonal \
            ];

            foreach (var dir in directions)
            {
                int count = 1;
                int r = row + dir[0];
                int c = col + dir[1];

                while (r >= 0 && r < rowsCount && c >= 0 && c < columnsCount && board[r, c] == player)
                {
                    count++;
                    r += dir[0];
                    c += dir[1];
                }

                r = row - dir[0];
                c = col - dir[1];
                while (r >= 0 && r < rowsCount && c >= 0 && c < columnsCount && board[r, c] == player)
                {
                    count++;
                    r -= dir[0];
                    c -= dir[1];
                }

                if (count >= 5)
                {
                    return true;
                }
            }
            return false;
        }

        private void winnerName2(CellState value)
        {
            string winner = "";
            if (value == CellState.O)
            {
                winner = "O";
                MessageBox.Show(winner + " win");
                restart();
            }
            else if (value == CellState.X)
            {
                winner = "X";
                MessageBox.Show(winner + " win!");
                restart();
            }


        }
        private void winnerName(CellState value, string line)
        {
            string winner = "";
            if (value == CellState.O)
            {
                winner = "O";
                MessageBox.Show(winner + " win! Win condition: " + line);
                restart();
            }
            else if (value == CellState.X)
            {
                winner = "X";
                MessageBox.Show(winner + " win! Win condition: " + line);
                restart();
            }


        }

        // button logic function
        private void restart()
        {

            // Create a list to store items to remove
            List<UIElement> elementsToRemove = new List<UIElement>();

            // Collect items to remove
            foreach (var child in grid.Children)
            {
                if (child is Ellipse || child is Canvas)
                {
                    elementsToRemove.Add((UIElement)child);
                }
            }

            // Remove collected items
            foreach (var element in elementsToRemove)
            {
                grid.Children.Remove(element);
            }

            // Reset arr to initial values
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    board[i, j] = CellState.Empty;
                }
            }
            Player = CellState.O;
            moveCount = 0;

            climaxTheme.Stop();
            keycardTheme.Stop();
            battleTheme.Play();

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
            sizeChangeWindow.NumCols = columnsCount;
            sizeChangeWindow.NumRows = rowsCount;
            sizeChangeWindow.SizeChanged += SizeChangeWindow_SizeChanged;
            sizeChangeWindow.ShowDialog();
        }

        private void SizeChangeWindow_SizeChanged(object sender, GetSizeChangeEvent e)
        {
            int numRows = e.NumRows;
            int numCols = e.NumColumns;

            rowsCount = numRows;
            columnsCount = numCols;
            setGridLine(numCols, numRows);

            setGridLine(rowsCount, columnsCount);
            UpdateUnits();
            board = new CellState[rowsCount, columnsCount];
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < columnsCount; j++)
                {
                    board[i, j] = CellState.Empty;
                }
            }
            Player = CellState.O;
            moveCount = 0;

            climaxTheme.Stop();
            keycardTheme.Stop();
            battleTheme.Play();
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
            Player = CellState.O;
        }

        //Music handler
        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = sender as MediaElement;
            mediaElement.Position = TimeSpan.Zero;
            mediaElement.Play();
        }
        
        void checkToChangeMusic()
        {
            int totalPossibleMove = rowsCount * columnsCount;
            if (totalPossibleMove <= 60)
            {
                if (moveCount == totalPossibleMove / 2)
                {
                    battleTheme.Stop();
                    keycardTheme.Play();
                    climaxTheme.Stop();
                }
                else if (moveCount >= totalPossibleMove * 3 / 4)
                {
                    battleTheme.Stop();
                    keycardTheme.Stop();
                    climaxTheme.Play();
                }
            }
            else
            {
                if (moveCount == 30)
                {
                    battleTheme.Stop();
                    keycardTheme.Play();
                    climaxTheme.Stop();
                }
                else if (moveCount >= 60)
                {
                    battleTheme.Stop();
                    keycardTheme.Stop();           
                    climaxTheme.Play();
                }
            }
        }

        //Play with keyboard
        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            {
                // Get the currently focused element
                UIElement focusedElement = Keyboard.FocusedElement as UIElement;
                if (focusedElement != null)
                    MessageBox.Show(focusedElement.GetType().Name);
                // If the focused element is a Rectangle within the grid
                if (focusedElement is Rectangle)
                {
                  
                    // Get the row and column of the focused rectangle
                    int currentRow = Grid.GetRow(focusedElement);
                    int currentColumn = Grid.GetColumn(focusedElement);

                    // Calculate the new row and column based on the arrow key pressed
                    int newRow = currentRow;
                    int newColumn = currentColumn;
                    if (e.Key == Key.Up && currentRow > 0)
                    {
                        newRow = currentRow - 1;
                    }
                    else if (e.Key == Key.Down && currentRow < grid.RowDefinitions.Count - 1)
                    {
                        newRow = currentRow + 1;
                    }
                    else if (e.Key == Key.Left && currentColumn > 0)
                    {
                        newColumn = currentColumn - 1;
                    }
                    else if (e.Key == Key.Right && currentColumn < grid.ColumnDefinitions.Count - 1)
                    {
                        newColumn = currentColumn + 1;
                    }

                    // Find the rectangle at the new row and column
                    UIElement newElement = null;
                    foreach (UIElement child in grid.Children)
                    {
                        if (Grid.GetRow(child) == newRow && Grid.GetColumn(child) == newColumn)
                        {
                            newElement = child;
                            break;
                        }
                    }

                    // Focus the new rectangle
                    if (newElement != null)
                    {
                        newElement.Focus();
                    }
                }

                // Mark the event as handled to prevent other controls from processing the arrow key
                e.Handled = true;
            }
        }
    }


}