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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using WindowWalker.Components;
using System.Windows.Interop;

namespace WindowWalker
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : System.Windows.Window
    {
        private IntPtr handleToMainWindow;

        public MainWindow MainWindow
        {
            get;
            set;
        }

        public SettingsWindow()
        {
            InitializeComponent();

            foreach (var shortcut in SettingsManager.SettingsInstance.Shortcuts)
            {
                shortcutsPanel.Items.Add(BuildShortcutEntryUI(shortcut.Key, shortcut.Value));
            }

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.shortcutBefore.Focus();
        }

        void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.MainWindow.SwitchToSearchWindow(false);
        }

        void DeleteShortcutClicked(object sender, RoutedEventArgs e)
        {
            UIElement source = (UIElement)e.Source;
            Grid entry = source.TryFindParent<Grid>();

            var before = (TextBlock)entry.Children[0];
            SettingsManager.Instance.RemoveShortcut(before.Text);

            shortcutsPanel.Items.Remove(entry);
        }

        void DoneClicked(object sender, RoutedEventArgs e)
        {
            this.AddNewShortcut();
        }

        private void AddNewShortcut()
        {
            string before = shortcutBefore.Text;
            string after = shortcutAfter.Text;

            if (!String.IsNullOrWhiteSpace(before) && !String.IsNullOrWhiteSpace(after) &&
                SettingsManager.Instance.AddShortcut(before, after))
            {
                shortcutsPanel.Items.Add(BuildShortcutEntryUI(before, after));

                shortcutBefore.Text = String.Empty;
                shortcutAfter.Text = String.Empty;
            }
        }

        private UIElement BuildShortcutEntryUI(string before, string after)
        {
            var container = new Grid();
            ColumnDefinition colFrom = new ColumnDefinition() { Width = new GridLength(322.5) };
            ColumnDefinition colSpacing = new ColumnDefinition() { Width = new GridLength(10) };
            ColumnDefinition colTo = new ColumnDefinition() { Width = new GridLength(322.5) };
            ColumnDefinition colSpacing2 = new ColumnDefinition() { Width = new GridLength(10) };
            ColumnDefinition colDeleteButtonForShortcut = new ColumnDefinition() { Width = new GridLength(25) };

            container.ColumnDefinitions.Add(colFrom);
            container.ColumnDefinitions.Add(colSpacing);
            container.ColumnDefinitions.Add(colTo);
            container.ColumnDefinitions.Add(colSpacing2);
            container.ColumnDefinitions.Add(colDeleteButtonForShortcut);

            var beforeTextBlock = new TextBlock();
            beforeTextBlock.Text = before;
            beforeTextBlock.FontSize = 13;
            beforeTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            container.Children.Add(beforeTextBlock);
            beforeTextBlock.SetValue(Grid.ColumnProperty, 0);

            var afterTextBlock = new TextBlock();
            afterTextBlock.Text = after;
            afterTextBlock.FontSize = 13;
            afterTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            container.Children.Add(afterTextBlock);
            afterTextBlock.SetValue(Grid.ColumnProperty, 2);

            var deleteButton = new Button();
            deleteButton.Style = this.FindResource("ButtonStyle") as Style;
            deleteButton.Width = 25;
            deleteButton.Height = 25;
            deleteButton.Click += new RoutedEventHandler(DeleteShortcutClicked);
            deleteButton.Background = this.FindResource("DeleteBrush") as Brush;
            deleteButton.SetValue(Grid.ColumnProperty, 4);

            container.Children.Add(deleteButton);

            container.Margin = new Thickness(0, 0, 0, 4);

            return container;
        }

        private void WindowFinishedLoading(object sender, RoutedEventArgs e)
        {
            double left = this.Left;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            this.Left = left;
            this.Top = 0;

            this.handleToMainWindow = new WindowInteropHelper(this).Handle;

#if DEBUG
            this.Topmost = false;
#endif
        }

        private void WindowLostFocusEventHandler(object sender, EventArgs e)
        {
#if !DEBUG
            this.Hide();
            this.MainWindow.EnterWaitState();
#endif
        }

        private void UpdateWindowSize()
        {
            this.Height =
                this.shortcutsPanel.ActualHeight +
                this.TitleLabels.ActualHeight +
                this.windowBorder.BorderThickness.Top * 2 +
                this.separator.ActualHeight;

            var screen = System.Windows.Forms.Screen.FromHandle(this.handleToMainWindow);

            if (this.Height > screen.Bounds.Height)
            {
                this.Height = screen.Bounds.Height;
                this.shortcutsPanel.MaxHeight = screen.Bounds.Height - 30;
            }

            System.Diagnostics.Debug.Print("Window Size getting Updated");
        }

        private void ListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateWindowSize();
        }

        private void KeyPressedOnShortcutAfterBox(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.AddNewShortcut();
                this.shortcutBefore.Focus();
            }
        }
    }
}
