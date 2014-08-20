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

namespace WindowWalker
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            foreach (var shortcut in ShortcutManager.Instance.Shortcuts)
            {
                shortcutsPanel.Children.Add(BuildShortcutEntryUI(shortcut.Key, shortcut.Value));
            }

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();
        }

        void DeleteShortcutClicked(object sender, RoutedEventArgs e)
        {
            UIElement source = (UIElement)e.Source;
            StackPanel entry = source.TryFindParent<StackPanel>();

            var before = (TextBlock)entry.Children[0];
            ShortcutManager.Instance.RemoveShortcut(before.Text);

            shortcutsPanel.Children.Remove(entry);
        }

        void DoneClicked(object sender, RoutedEventArgs e)
        {
            string before = shortcutBefore.Text;
            string after = shortcutAfter.Text;

            if (!String.IsNullOrWhiteSpace(before) && !String.IsNullOrWhiteSpace(after) &&
                ShortcutManager.Instance.AddShortcut(before, after))
            {
                shortcutsPanel.Children.Add(BuildShortcutEntryUI(before, after));

                shortcutBefore.Text = String.Empty;
                shortcutAfter.Text = String.Empty;
            }
        }

        private UIElement BuildShortcutEntryUI(string before, string after)
        {
            var container = new StackPanel();
            container.Orientation = Orientation.Horizontal;

            var beforeTextBlock = new TextBlock();
            beforeTextBlock.Text = before;
            beforeTextBlock.Width = 220;
            beforeTextBlock.Margin = new Thickness(100, 0, 20, 0);
            container.Children.Add(beforeTextBlock);

            var afterTextBlock = new TextBlock();
            afterTextBlock.Text = after;
            afterTextBlock.Width = 220;
            afterTextBlock.Margin = new Thickness(0, 0, 15, 0);
            container.Children.Add(afterTextBlock);

            var rectangle = new Rectangle();
            rectangle.Style = this.FindResource("DeleteRectangle") as Style;
            rectangle.Width = 15;
            rectangle.Height = 15;

            var deleteButton = new Button();
            deleteButton.Width = 25;
            deleteButton.Height = 25;
            deleteButton.Click += new RoutedEventHandler(DeleteShortcutClicked);
            deleteButton.Content = rectangle;
            container.Children.Add(deleteButton);

            return container;
        }

        private void WindowFinishedLoading(object sender, RoutedEventArgs e)
        {
        }

        private void WindowLostFocusEventHandler(object sender, EventArgs e)
        {
        }
    }
}
