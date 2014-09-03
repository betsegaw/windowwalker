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

        private SettingsWindow settingsWindow;

        private IntPtr handleToMainWindow;

        private bool waitState = false;

        private object hotkeyLock = new object();

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

            OpenWindows.Instance.UpdateOpenWindowsList();
        }

        void SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            this.SwitchToSettingsPage();
        }

        private void SwitchToSettingsPage()
        {
            if (this.settingsWindow == null)
            {
                this.settingsWindow = new SettingsWindow();
            }

            this.settingsWindow.MainWindow = this;
            this.settingsWindow.Show();

            this.Hide();
        }

        private void TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            WindowSearchController.Instance.SearchText = this.searchTextBox.Text;
        }

        public void SearchResultUpdateHandler(object sender, WindowWalker.Components.Window.WindowListUpdateEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
            
                resultsListBox.Items.Clear();

                var windowsResult = WindowSearchController.Instance.SearchMatches.Where(x => x.ResultWindow.Hwnd != this.handleToMainWindow);
                Dictionary<TextBlock, WindowSearchResult> highlightStack = new Dictionary<TextBlock,WindowSearchResult>();

                windowsResult = windowsResult.OrderByDescending(x => x.Score);

                foreach (WindowSearchResult windowResult in windowsResult)
                {
                    /// Each window is shown in a horizontal stack panel
                    /// that contains an image object on the left and 
                    /// a textblock with the window title on the right

                    var tempStackPanel = new StackPanel();
                    tempStackPanel.Orientation = Orientation.Horizontal;
                    var image = new Image();
                    image.Source = windowResult.ResultWindow.WindowIcon;
                    image.Margin = new Thickness(0,0,5,0);
                    tempStackPanel.Children.Add(image);
                    var tempTextBlock = new TextBlockWindow();

                    tempTextBlock.Window = windowResult.ResultWindow;
                    tempStackPanel.Children.Add(tempTextBlock);
                    image.Height = 15;
                    this.resultsListBox.Items.Add(tempStackPanel);
                    highlightStack[tempTextBlock] = windowResult;
                }

                if (resultsListBox.Items.Count != 0)
                {
                    resultsListBox.SelectedIndex = 0;
                }

                this.UpdateTextWithHighlight(highlightStack);
                
                System.Diagnostics.Debug.Print("Search result updated in Main Window. There are now " + this.resultsListBox.Items.Count + " windows that match the search term");
            });
        }

        private void UpdateTextWithHighlight(Dictionary<TextBlock, WindowSearchResult> highlightstack)
        {
            foreach (var key in highlightstack.Keys)
            {
                var textBlock = key;
                var windowResult = highlightstack[key];
                var windowDisplayText = windowResult.ResultWindow.Title;

                List<int> listOfBoldIndexesForDisplayText;
                IOrderedEnumerable<int> listOfIndexesForProcessName;

                if (windowResult.BestScoreSource == WindowSearchResult.TextType.ProcessName)
                {
                    listOfBoldIndexesForDisplayText = new List<int>();
                    listOfIndexesForProcessName = windowResult.SearchMatchesInProcessName.OrderBy(x => x);
                }
                else
                {
                    listOfBoldIndexesForDisplayText = windowResult.SearchMatchesInTitle.OrderBy(x => x).ToList();
                    listOfIndexesForProcessName = new List<int>().OrderBy(x => x);
                }

                foreach(var i in listOfIndexesForProcessName)
                {
                    listOfBoldIndexesForDisplayText.Add(i + windowDisplayText.Length + 2);
                }

                if (!windowResult.ResultWindow.ProcessName.ToUpper().Equals(string.Empty))
                {
                    windowDisplayText += " (" + windowResult.ResultWindow.ProcessName.ToUpper() + ")";
                }

                int start = 0;
                textBlock.Inlines.Clear();

                if (listOfBoldIndexesForDisplayText.Count() != 0)
                {
                    foreach (int boldIndex in listOfBoldIndexesForDisplayText)
                    {
                        var r = new Run(windowDisplayText.Substring(start, boldIndex - start));
                        r.FontSize = 13;
                        r.Foreground = new SolidColorBrush(Color.FromRgb(110,110,110));
                        textBlock.Inlines.Add(r);

                        System.Diagnostics.Debug.Print(windowDisplayText + " " + windowDisplayText.Length + " " + boldIndex);
                        r = new Run(windowDisplayText.Substring(boldIndex, 1));
                        r.Text = windowDisplayText.Substring(boldIndex, 1);
                        var b = new Bold(r);
                        b.FontSize = 15;
                        textBlock.Inlines.Add(b);
                        start = boldIndex + 1;
                    }

                    if (start < windowDisplayText.Length)
                    {
                        var r = new Run(windowDisplayText.Substring(start, windowDisplayText.Length - start));
                        r.FontSize = 13;
                        r.Foreground = new SolidColorBrush(Color.FromRgb(110,110,110));
                        textBlock.Inlines.Add(r);
                    }
                }
                else
                {
                    var r = new Run(windowDisplayText);
                    r.FontSize = 13;
                    textBlock.Inlines.Add(r);
                }
            }
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
                else if (this.searchTextBox.Text == ":s")
                {
                    this.searchTextBox.Text = string.Empty;
                    this.SwitchToSettingsPage();
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
#if DEBUG
            this.Topmost = false;
#endif
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
            lock(this.hotkeyLock)
            {
                if (this.waitState)
                {
                    this.SwitchToSearchWindow(true);
                }
            }
        }

        public void SwitchToSearchWindow(bool clearSearchBox)
        {
            this.waitState = false;
            this.searchTextBox.Text = clearSearchBox ? string.Empty : searchTextBox.Text;
            OpenWindows.Instance.UpdateOpenWindowsList();
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
            this.waitState = true;
        }

        private void WindowLostFocusEventHandler(object sender, EventArgs e)
        {
#if !DEBUG
            this.EnterWaitState();
#endif
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

#if !DEBUG
            if (windowIsLoaded && this.resultsListBox.SelectedItem != null)
            {
                Components.LivePreview.ActivateLivePreview(
                    ((TextBlockWindow)(((StackPanel)this.resultsListBox.SelectedItem).Children[1])).Window.Hwnd,
                    this.handleToMainWindow);
            }
#endif
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