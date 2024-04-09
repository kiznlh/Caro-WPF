using Microsoft.Win32;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        public Rectangle[,] boardCells;
        int currentRow;
        int currentColumn;
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
                if (_circleUnit != value)
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
            currentColumn = 0;
            currentRow = 0;
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

            KeyDown += window_KeyDown;
            //play music
            climaxTheme.Stop();
            keycardTheme.Stop();
            battleTheme.Play();

        }

        private void setGridLine(int row_count, int col_count)
        {
            clearGrid();
            //climaxTheme.Stop();
            //keycardTheme.Stop();
            //battleTheme.Stop();
            //battleTheme.Play();
            boardCells = new Rectangle[row_count, col_count];
            for (int row = 0; row < row_count; row++)
            {
                for (int col = 0; col < col_count; col++)
                {
                    boardCells[row, col] = null;
                }
            }
            pointerRow = 0;
            pointerColumn = 0;
            pointerElement = null;


            currentRow = 0;
            currentColumn = 0;
            board = new CellState[row_count, col_count];
            for (int i = 0; i < row_count; i++)
            {
                for (int j = 0; j < col_count; j++)
                {
                    board[i, j] = CellState.Empty;
                }
            }


            for (int i = 0; i < col_count; i++)
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
            for (int i = 0; i < row_count; i++)
            {
                for (int j = 0; j < col_count; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Stroke = new SolidColorBrush(Colors.Black);
                    rectangle.Fill = new SolidColorBrush(Colors.Transparent);
                    rectangle.StrokeThickness = 0.5;
                    rectangle.MouseEnter += cell_MouseEnter;
                    rectangle.MouseLeave += cell_MouseLeave;

                    boardCells[i, j] = rectangle;

                    grid.Children.Add(rectangle);
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);
                  

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
                firstKeyPress = true;
                resetColorKeyboardCell();

                int row = pointerRow;
                int column = pointerColumn;

                if (pointerElement != null && grid.Children.Contains(pointerElement))
                {
                    grid.Children.Remove(pointerElement);
                    pointerElement = null;
                }
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



        // win condition
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
            battleTheme.Stop();
            keycardTheme.Stop();
            climaxTheme.Stop();
            victoryTheme.Play();
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
            victoryTheme.Stop();

        }
        //private void winnerName(CellState value, string line)
        //{
        //    string winner = "";
        //    if (value == CellState.O)
        //    {
        //        winner = "O";
        //        MessageBox.Show(winner + " win! Win condition: " + line);
        //        restart();
        //    }
        //    else if (value == CellState.X)
        //    {
        //        winner = "X";
        //        MessageBox.Show(winner + " win! Win condition: " + line);
        //        restart();
        //    }


        //}

        // button logic function
        private void restart()
        {
            resetColorKeyboardCell();
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
            climaxTheme.Stop();
            keycardTheme.Stop();
            battleTheme.Stop();
            battleTheme.Play();

            currentColumn = 0;
            currentRow = 0;
            Player = CellState.O;
            moveCount = 0;
            //reset keyboard
   
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Create SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            // Show save file dialog
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the selected file name
                string fileName = saveFileDialog.FileName;

                // Write the data to the selected file
                WriteToFile(fileName);

                MessageBox.Show($"{fileName} has been saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void WriteToFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // Write the dimensions of the board
                
                writer.WriteLine($"{rowsCount} {columnsCount}");

                // Write the board state
                for (int i = 0; i < rowsCount; i++)
                {
                    for (int j = 0; j < columnsCount; j++)
                    {
                        writer.Write((int)board[i, j] + " ");
                    }
                    writer.WriteLine();
                }

                // Write the current player
                writer.WriteLine((int)Player);

                // Write the move count
                writer.WriteLine(moveCount);

               
            }
        }
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
      


            // Create OpenFileDialog

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            // Show open file dialog
            if (openFileDialog.ShowDialog() == true)
            {
                climaxTheme.Stop();
                keycardTheme.Stop();
                battleTheme.Stop();
                // Get the selected file name
                string fileName = openFileDialog.FileName;

                // Read data from the selected file
                ReadFromFile(fileName);
            }
        }
        private void ReadFromFile(string fileName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    // Read the dimensions of the board
                    string[] dimensions = reader.ReadLine().Split(' ');
                    rowsCount = int.Parse(dimensions[0]);
                    columnsCount = int.Parse(dimensions[1]);
                    setGridLine(rowsCount, columnsCount);

                    // Read the board state
                    for (int i = 0; i < rowsCount; i++)
                    {
                        string[] rowValues = reader.ReadLine().Split(' ');
                        for (int j = 0; j < columnsCount; j++)
                        {
                            CellState cellState = (CellState)int.Parse(rowValues[j]); 
                            board[i, j] = cellState;

                            if (cellState == CellState.O)
                            {
                                Ellipse blueCircle = setCircle();
                                Grid.SetRow(blueCircle, i);
                                Grid.SetColumn(blueCircle, j);
                                blueCircle.HorizontalAlignment = HorizontalAlignment.Center;
                                blueCircle.VerticalAlignment = VerticalAlignment.Center;
                                pointerElement = blueCircle;
                                grid.Children.Add(blueCircle);
                            }
                            else if (cellState == CellState.X)
                            {
                                Canvas redCross = setCross();
                                Grid.SetRow(redCross, i);
                                Grid.SetColumn(redCross, j);
                                redCross.HorizontalAlignment = HorizontalAlignment.Center;
                                redCross.VerticalAlignment = VerticalAlignment.Center;
                                pointerElement = redCross;
                                grid.Children.Add(redCross);
                            }
                        }
                    }

                    // Read the current player
                    Player = (CellState)int.Parse(reader.ReadLine());

                    // Read the move count
                    moveCount = int.Parse(reader.ReadLine());

                    

                    MessageBox.Show("File loaded successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    checkToChangeMusic();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            setGridLine(rowsCount, columnsCount);
            UpdateUnits();

            Player = CellState.O;
            moveCount = 0;

            climaxTheme.Stop();
            keycardTheme.Stop();
            battleTheme.Stop();
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
;
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
                if (moveCount >= totalPossibleMove / 2 && moveCount < totalPossibleMove * 3 / 4)
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
                if (moveCount >= 30 && moveCount < 60)
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


        bool firstKeyPress = true;
        //Play with keyboard

        // Reset focus cell
        private void resetColorKeyboardCell()
        {
            boardCells[currentRow, currentColumn].Stroke = new SolidColorBrush(Colors.Black);
            boardCells[currentRow, currentColumn].StrokeThickness = 0.5;

            if (pointerElement != null && grid.Children.Contains(pointerElement))
            {
                grid.Children.Remove(pointerElement);
                pointerElement = null;
            }
        }

        // Focus a cell
        private void setColorKeyboardCell() 
        {
            boardCells[currentRow, currentColumn].Stroke = new SolidColorBrush(Colors.Red);
            boardCells[currentRow, currentColumn].StrokeThickness = 2;

            pointerRow = currentRow;
            pointerColumn = currentColumn;
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

        // Keyboard up,left,down,right and enter event
        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (firstKeyPress)
            {
                if (pointerElement != null && grid.Children.Contains(pointerElement))
                {
                    grid.Children.Remove(pointerElement);
                    pointerElement = null;
                }
                setColorKeyboardCell();
                firstKeyPress = false;
                Cursor = Cursors.None;
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Up:
                        resetColorKeyboardCell();
                        if (currentRow > 0)
                            currentRow--;
                        setColorKeyboardCell();
                        Cursor = Cursors.None;
                        break;
                    case Key.Down:
                        resetColorKeyboardCell();
                        if (currentRow < rowsCount - 1)
                            currentRow++;
                        setColorKeyboardCell();
                        Cursor = Cursors.None;
                        break;
                    case Key.Left:
                        resetColorKeyboardCell();
                        if (currentColumn > 0)
                            currentColumn--;
                        setColorKeyboardCell();
                        Cursor = Cursors.None;
                        break;
                    case Key.Right:
                        resetColorKeyboardCell();
                        if (currentColumn < columnsCount - 1)
                            currentColumn++;
                        setColorKeyboardCell();
                        Cursor = Cursors.None;
                        break;
                    case Key.Enter:
                        Cursor = Cursors.None;
                        int row = currentRow;
                        int column = currentColumn;
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
              
                        break;
                }

            }
        }
        // Enable cursor when move
        private void grid_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
    }


}