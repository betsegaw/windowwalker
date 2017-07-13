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
        }

        private void SearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
            }
            else if (e.Key == Key.Down || e.Key == Key.Up)
            {
                if (results.Items.Count <= 1)
                {
                    return;
                }

                if ((e.Key == Key.Down && results.SelectedIndex + 1 == results.Items.Count) ||
                    (e.Key == Key.Up && results.SelectedIndex == 0))
                {
                    return;
                }
                else
                {
                    results.SelectedIndex += e.Key == Key.Down ? 1 : -1;
                }
            }
            else if (e.Key == Key.Enter)
            {
                ((ViewModels.WindowWalkerViewModel)this.DataContext).UserSelectionFinalized();
            }
        }
    }
}