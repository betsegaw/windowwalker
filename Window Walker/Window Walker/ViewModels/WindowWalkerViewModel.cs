using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using WindowWalker.Components;

namespace WindowWalker.ViewModels
{


    class WindowWalkerViewModel: MVVMHelpers.PropertyChangedBase
    {
        private string _searchText = string.Empty;
        private List<WindowSearchResult> _results = new List<WindowSearchResult>();
        private WindowSearchResult _selectedWindow;
        private static WindowWalkerViewModel _instance;

        public string SearchText {
            get => _searchText;

            set {
                _searchText = value;
                WindowSearchController.Instance.SearchText = value;
                NotifyPropertyChanged("SearchText");
            }
        }

        public List<WindowSearchResult> Results
        {
            get => _results;

            set
            {
                if (_results != value)
                {
                    _results = value;
                    NotifyPropertyChanged("Results");
                }
            }
        }

        public WindowSearchResult SelectedWindowResult
        {
            get => _selectedWindow;
            set
            {
                _selectedWindow = value;
                this.WindowResultSelected();
                NotifyPropertyChanged("SelectedWindowResult");
            }
        }

        private void WindowResultSelected()
        {
            Components.LivePreview.ActivateLivePreview(this.SelectedWindowResult.ResultWindow.Hwnd, this.Hwnd);
        }

        public IntPtr Hwnd { get; private set; }

        public WindowWalkerViewModel(System.Windows.Window mainWindow)
        {
            WindowSearchController.Instance.OnSearchResultUpdate += SearchResultUpdated;
            OpenWindows.Instance.UpdateOpenWindowsList();
            this.Hwnd = new WindowInteropHelper(mainWindow).Handle;
            LivePreview.SetWindowExlusionFromLivePreview(this.Hwnd);
        }

        private void SearchResultUpdated(object sender, Window.WindowListUpdateEventArgs e)
        {
            this.Results = WindowSearchController.Instance.SearchMatches;
        }

        private void WindowResultHighlighted()
        {
            Components.LivePreview.ActivateLivePreview(this.SelectedWindowResult.ResultWindow.Hwnd, this.Hwnd);
        }
    }
}
