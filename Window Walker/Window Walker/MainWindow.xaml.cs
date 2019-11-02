using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
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

        private void UpdateDisplayedVersionNumber()
        {
            // Since displaying version number is not critical to functionality, we don't need to do anything if it fails.
            try
            {
                Version ApplicationVersion = new Version("0.0.0.0");
                if (ApplicationDeployment.IsNetworkDeployed)
                    ApplicationVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                
                if (ApplicationDeployment.CurrentDeployment.UpdateLocation.AbsolutePath.Contains("develop"))
                {
                    this.versionDisplay.Text = "(develop) " + ApplicationVersion.ToString();
                }
                else
                {
                    this.versionDisplay.Text = ApplicationDeployment.CurrentDeployment.UpdateLocation.Host + " " + ApplicationVersion.ToString();
                }
            }
            catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new ViewModels.WindowWalkerViewModel(this);
            this.searchBox.Focus();
            
            UpdateDisplayedVersionNumber();
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

        private void Window_Deactivated(object sender, EventArgs e)
        {
            var viewModel = ((ViewModels.WindowWalkerViewModel)this.DataContext);
            if (viewModel.WindowHideCommand.CanExecute(null))
            {
                viewModel.WindowHideCommand.Execute(null);
            }
        }
    }

    /// <summary>
    /// Converts a string containing valid XAML into WPF objects.
    /// </summary>
    [ValueConversion(typeof(WindowSearchResult), typeof(object))]
    public sealed class WindowSearchResultToXamlConverter : IValueConverter
    {
        /// <summary>
        /// Converts a string containing valid XAML into WPF objects.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>A WPF object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            WindowSearchResult input = value as WindowSearchResult;
            if (input != null)
            {
                string withTags;

                if (input.BestScoreSource == WindowSearchResult.TextType.ProcessName)
                {
                    withTags = input.ResultWindow.Title;
                    withTags += $" ({InsertHighlightTags(input.ResultWindow.ProcessName, input.SearchMatchesInProcessName)})";
                }
                else
                {
                    withTags = InsertHighlightTags(input.ResultWindow.Title, input.SearchMatchesInTitle);
                    withTags += $" ({input.ResultWindow.ProcessName})";
                }

                withTags = SecurityElement.Escape(withTags);

                withTags = withTags.Replace("[[", "<Run Background=\"{DynamicResource SecondaryAccentBrush}\" FontSize=\"18\" FontWeight=\"Bold\" Foreground=\"{DynamicResource SecondaryAccentForegroundBrush}\">").
                                    Replace("]]", "</Run>");
                
                string wrappedInput = string.Format("<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" TextWrapping=\"Wrap\">{0}</TextBlock>", withTags);

                using (StringReader stringReader = new StringReader(wrappedInput))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    {
                        return XamlReader.Load(xmlReader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Converts WPF framework objects into a XAML string.
        /// </summary>
        /// <param name="value">The WPF Famework object to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>A string containg XAML.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This converter cannot be used in two-way binding.");
        }

        private string InsertHighlightTags(string content, List<int> indexes)
        {
            int offset = 0;
            var result = content.Replace("[[", "**").Replace("]]", "**");
        
            string startTag = "[[";
            string stopTag = "]]";

            foreach (var index in indexes)
            {
                result = result.Insert(index + offset, startTag);
                result = result.Insert(index + offset + startTag.Length + 1, stopTag);

                offset += startTag.Length + stopTag.Length;
            }

            return result;
        }
    }
}