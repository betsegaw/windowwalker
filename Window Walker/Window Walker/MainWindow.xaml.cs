using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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

            WindowSearchController.Instance.OnSearchResultUpdate += this.SearchResultUpdateHandler;
            WindowSearchController.Instance.SearchTextUpdated();
        }

        private void TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            WindowSearchController.Instance.SearchText = this.searchTextBox.Text;
        }

        public void SearchResultUpdateHandler(object sender, WindowWalker.Components.Window.WindowListUpdateEventArgs e)
        {
            resultsListBox.Items.Clear();

            foreach(var window in WindowSearchController.Instance.SearchMatches)
            {
                resultsListBox.Items.Add(window.Title);
            }
        }
    }
}
