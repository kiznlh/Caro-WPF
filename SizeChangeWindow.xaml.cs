using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Caro_WPF
{
    /// <summary>
    /// Interaction logic for SizeChangeWindow.xaml
    /// </summary>
    /// 

    public class SizeRangeRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int result = 0;
            try
            {
                if (((string)value).Length > 0)
                {
                    result = Int32.Parse((String)value);
                }
            }
            catch(Exception e) 
            {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }
            if (result < 5)
            {
                return new ValidationResult(false,
                    "Please enter value >= 5");

            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
    public partial class SizeChangeWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private int _numRows = 5;
        private int _numCols = 5;
        public int NumRows
        {
            get { return _numRows; }
            set
            {
                _numRows = value;
                OnPropertyChanged();
            }
        }
        public int NumCols
        {
            get { return _numCols; }
            set
            {
                _numCols = value;
                OnPropertyChanged();
            }
        }
        public SizeChangeWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        public event EventHandler<GetSizeChangeEvent> SizeChanged;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SizeChanged?.Invoke(this, new GetSizeChangeEvent(NumRows, NumCols));
       
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }

    public class GetSizeChangeEvent : EventArgs
    {
        public int NumRows { get; }
        public int NumColumns { get; }

        public GetSizeChangeEvent(int numRows, int numColumns)
        {
            NumRows = numRows;
            NumColumns = numColumns;
        }
    }

}
