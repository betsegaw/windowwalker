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
        }

        public void UpdateWindowList()
        {
            var windows = WindowSearchController.Instance.SearchMatches;

            this.OpenWindowsCombo.Items.Clear();

            foreach(var window in windows)
            {
                this.OpenWindowsCombo.Items.Add(window.Title);
            }
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateWindowList();
        }
    }
}
