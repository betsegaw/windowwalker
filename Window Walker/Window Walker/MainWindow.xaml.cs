using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowWalker.Components;

namespace WindowWalker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new ViewModels.WindowWalkerViewModel(this);
            this.searchBox.Focus();
        }

        private void SearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            var viewModel = ((ViewModels.WindowWalkerViewModel)this.DataContext);

            if (e.Key == Key.Escape)
            {
                if(viewModel.WindowHideCommand.CanExecute(null))
                {
                    viewModel.WindowHideCommand.Execute(null);
                }
            }
            else if (e.Key == Key.Down)
            {
                if (viewModel.WindowNavigateToNextResultCommand.CanExecute(null))
                {
                    viewModel.WindowNavigateToNextResultCommand.Execute(null);
                }
            }
            else if (e.Key == Key.Up)
            {
                if (viewModel.WindowNavigateToPreviousResultCommand.CanExecute(null))
                {
                    viewModel.WindowNavigateToPreviousResultCommand.Execute(null);
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (viewModel.SwitchToSelectedWindowCommand.CanExecute(null))
                {
                    viewModel.SwitchToSelectedWindowCommand.Execute(null);
                }
            }
        }

        private void results_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = ((ViewModels.WindowWalkerViewModel)this.DataContext);

            if (viewModel.SwitchToSelectedWindowCommand.CanExecute(null))
            {
                viewModel.SwitchToSelectedWindowCommand.Execute(null);
            }
        }
    }

    public class IntToDoubleLeftValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rectangle = values[0] as Rectangle;
            var grid = values[1] as Grid;
            var textBox = ((TextBox)grid.Children[0]);
            var index = (int)rectangle.DataContext;

            var left = textBox.GetRectFromCharacterIndex(index).Left;

            return System.Convert.ToDouble(left - 2);
        }

        public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class IntToDoubleRightValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rectangle = values[0] as Rectangle;
            var grid = values[1] as Grid;
            var textBox = ((TextBox)grid.Children[0]);
            var index = (int)rectangle.DataContext;

            var left = textBox.GetRectFromCharacterIndex(index).Left;

            return System.Convert.ToDouble(left - 2);
        }

        public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}