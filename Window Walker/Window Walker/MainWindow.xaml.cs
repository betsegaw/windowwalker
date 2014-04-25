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

        private IntPtr handleToMainWindow;

        /// <summary>
        /// An flag indicating if the window has finished loading
        /// </summary>
        private bool windowIsLoaded = false;

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

            var windows = WindowSearchController.Instance.SearchMatches.Where(x => x.Hwnd != this.handleToMainWindow);

            foreach (var window in windows)
            {
                var tempStackPanel = new StackPanel();
                tempStackPanel.Orientation = Orientation.Horizontal;
                var image = new Image();
                image.Source = window.WindowIcon;
                image.Margin = new Thickness(0,0,5,0);
                tempStackPanel.Children.Add(image);
                var tempTextBlock = new TextBlockWindow();
                tempTextBlock.Text = window.Title + " (" + window.ProcessName.ToUpper() + ")" ;
                tempTextBlock.Window = window;
                tempStackPanel.Children.Add(tempTextBlock);
                image.Height = 15;
                this.resultsListBox.Items.Add(tempStackPanel);
            }

            if (resultsListBox.Items.Count != 0)
            {
                resultsListBox.SelectedIndex = 0;
            }

            System.Diagnostics.Debug.Print("Search result updated in Main Window. There are now " + this.resultsListBox.Items.Count + " windows that match the search term");
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

            System.Diagnostics.Debug.Print("Keypress handled");
        }

        private void WindowFinishedLoading(object sender, RoutedEventArgs e)
        {
            double left = this.Left;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            this.Left = left;
            this.Top = 0;
            this.UpdateWindowSize();

            this.hotKeyHandler = new HotKeyHandler(this);
            this.hotKeyHandler.OnHotKeyPressed += this.HotKeyPressedHandler;

            this.handleToMainWindow = new WindowInteropHelper(this).Handle;
            LivePreview.SetWindowExlusionFromLivePreview(this.handleToMainWindow);
            this.windowIsLoaded = true;
        }

        private void UpdateWindowSize()
        {
            this.Height =
                this.resultsListBox.ActualHeight +
                this.searchTextBox.ActualHeight +
                this.windowBorder.BorderThickness.Top * 2 +
                this.separator.ActualHeight;

            var screen = System.Windows.Forms.Screen.FromHandle(this.handleToMainWindow);

            if (this.Height > screen.Bounds.Height)
            {
                this.Height = screen.Bounds.Height;
                this.resultsListBox.MaxHeight = screen.Bounds.Height - 30;
            }

            System.Diagnostics.Debug.Print("Window Size getting Updated");
        }

        public void HotKeyPressedHandler(object sender, EventArgs e)
        {
            this.searchTextBox.Text = string.Empty;
            this.Show();
            InteropAndHelpers.SetForegroundWindow(this.handleToMainWindow);
            this.TextChangedEvent(null, null);
            this.searchTextBox.Focus();
        }

        /// <summary>
        /// Makes the window invisible and wait for the hotkey to be pressed
        /// </summary>
        public void EnterWaitState()
        {
            this.Hide();
            Components.LivePreview.DeactivateLivePreview();
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
                ((TextBlockWindow)(((StackPanel)this.resultsListBox.SelectedItem).Children[1])).Window.SwitchToWindow();
                this.EnterWaitState();
            }
        }

        private void WindowSelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            if (windowIsLoaded && this.resultsListBox.SelectedItem != null)
            {
                Components.LivePreview.ActivateLivePreview(
                    ((TextBlockWindow)(((StackPanel)this.resultsListBox.SelectedItem).Children[1])).Window.Hwnd,
                    this.handleToMainWindow);
            }
        }

        private void ListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateWindowSize();
        }
    }

    public class TextBlockWindow : TextBlock
    {
        public Components.Window Window
        {
            get;
            set;
        }
    }
}