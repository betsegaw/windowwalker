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
        private HotKeyHandler hotKeyHandler;

        public MainWindow()
        {
            InitializeComponent();

            WindowSearchController.Instance.OnSearchResultUpdate += this.SearchResultUpdateHandler;
            WindowSearchController.Instance.SearchTextUpdated();

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.searchTextBox.Focus();
        }

        private void TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            WindowSearchController.Instance.SearchText = this.searchTextBox.Text;
        }

        public void SearchResultUpdateHandler(object sender, WindowWalker.Components.Window.WindowListUpdateEventArgs e)
        {
            resultsListBox.Items.Clear();

            var windows = WindowSearchController.Instance.SearchMatches.Where(x => x.Hwnd != new WindowInteropHelper(this).Handle);

            foreach (var window in windows)
            {
                resultsListBox.Items.Add(window);
            }

            if (resultsListBox.Items.Count != 0)
            {
                resultsListBox.SelectedIndex = 0;
            }

            this.UpdateWindowSize();
        }

        private void KeyPressActionHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.EnterWaitState();
            }
            else if (e.Key == Key.Down && this.resultsListBox.SelectedIndex != this.resultsListBox.Items.Count)
            {
                this.resultsListBox.SelectedIndex++;
            }
            else if (e.Key == Key.Up && this.resultsListBox.SelectedIndex > 0)
            {
                this.resultsListBox.SelectedIndex--;
            }
            else if (e.Key == Key.Enter)
            {
                if (this.searchTextBox.Text == ":quit")
                {
                    App.Current.Shutdown();
                    return;
                }
                else
                {
                    this.SwitchToSelectedWindow();
                }
            }

            this.UpdateWindowSize();
        }

        private void SetWindowLocation(object sender, RoutedEventArgs e)
        {
            double left = this.Left;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            this.Left = left;
            this.Top = 0;
            this.UpdateWindowSize();

            this.hotKeyHandler = new HotKeyHandler(this);
            this.hotKeyHandler.OnHotKeyPressed += this.HotKeyPressedHandler;
        }

        private void UpdateWindowSize()
        {
            this.Height = 
                this.resultsListBox.ActualHeight + 
                this.searchTextBox.ActualHeight +
                this.windowBorder.BorderThickness.Top * 2  +
                this.separator.ActualHeight;
        }

        public void HotKeyPressedHandler(object sender, EventArgs e)
        {
            WindowSearchController.Instance.SearchText = this.searchTextBox.Text;
            this.Show();
            InteropAndHelpers.SetForegroundWindow(new WindowInteropHelper(this).Handle);
            this.searchTextBox.Text = string.Empty;
            this.TextChangedEvent(null, null);
            this.searchTextBox.Focus();
        }

        /// <summary>
        /// Makes the window invisible and wait for the hotkey to be pressed
        /// </summary>
        public void EnterWaitState()
        {
            this.Hide();
        }

        private void WindowLostFocusEventHandler(object sender, EventArgs e)
        {
            this.EnterWaitState();
        }

        private void WindowSelectedByMouseEvent(object sender, MouseButtonEventArgs e)
        {
            this.SwitchToSelectedWindow();   
        }

        private void SwitchToSelectedWindow()
        {
            if (resultsListBox.SelectedIndex >= 0)
            {
                IntPtr hwndOfSelectedWindow = ((Components.Window)this.resultsListBox.SelectedItem).Hwnd;
                InteropAndHelpers.ShowWindow(hwndOfSelectedWindow, InteropAndHelpers.ShowWindowCommands.Maximize);
                InteropAndHelpers.SetForegroundWindow(hwndOfSelectedWindow);
                this.EnterWaitState();
                InteropAndHelpers.FlashWindow(hwndOfSelectedWindow, true);
            }
        }
    }
}
