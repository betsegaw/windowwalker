using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using WindowWalker.Components;
using WindowWalker.MVVMHelpers;

namespace WindowWalker.ViewModels
{
    class WindowWalkerViewModel: MVVMHelpers.PropertyChangedBase
    {
        private string _searchText = string.Empty;
        private List<WindowSearchResult> _results = new List<WindowSearchResult>();
        private WindowSearchResult _selectedWindow;
        private bool _windowVisibility = true;
        private HotKeyHandler hotKeyHandler;

        private void WireCommands()
        {
            SwitchToSelectedWindowCommand = new RelayCommand(SwitchToSelectedWindow);
            SwitchToSelectedWindowCommand.IsEnabled = true;
            WindowNavigateToNextResultCommand = new RelayCommand(WindowNavigateToNextResult);
            WindowNavigateToNextResultCommand.IsEnabled = true;
            WindowNavigateToPreviousResultCommand = new RelayCommand(WindowNavigateToPreviousResult);
            WindowNavigateToPreviousResultCommand.IsEnabled = true;
            WindowHideCommand = new RelayCommand(WindowHide);
            WindowHideCommand.IsEnabled = true;
            WindowShowCommand = new RelayCommand(WindowShow);
            WindowShowCommand.IsEnabled = true;
        }

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

        public IntPtr Hwnd { get; private set; }

        public bool WindowVisibility
        {
            get
            {
                return _windowVisibility;
            }
            set
            {
                if (_windowVisibility != value)
                {
                    _windowVisibility = value;
                    NotifyPropertyChanged("WindowVisibility");
                }
            }
        }

        public RelayCommand SwitchToSelectedWindowCommand
        {
            get;
            private set;
        }

        public RelayCommand WindowNavigateToNextResultCommand
        {
            get;
            private set;
        }

        public RelayCommand WindowNavigateToPreviousResultCommand
        {
            get;
            private set;
        }

        public RelayCommand WindowHideCommand
        {
            get;
            private set;
        }

        public RelayCommand WindowShowCommand
        {
            get;
            private set;
        }

        public WindowWalkerViewModel(System.Windows.Window mainWindow)
        {
            WindowSearchController.Instance.OnSearchResultUpdate += SearchResultUpdated;
            OpenWindows.Instance.UpdateOpenWindowsList();
            this.Hwnd = new WindowInteropHelper(mainWindow).Handle;
            LivePreview.SetWindowExlusionFromLivePreview(this.Hwnd);

            this.hotKeyHandler = new HotKeyHandler(mainWindow);
            this.hotKeyHandler.OnHotKeyPressed += this.HotKeyPressedHandler;

            WireCommands();
        }

        private void HotKeyPressedHandler(object sender, EventArgs e)
        {
            this.WindowShow();
        }

        private void WindowResultSelected()
        {
            Components.LivePreview.ActivateLivePreview(this.SelectedWindowResult.ResultWindow.Hwnd, this.Hwnd);
        }

        private void WindowNavigateToPreviousResult()
        {
            if(this.SelectedWindowResult == null && this.Results.Count > 0)
            {
                this.SelectedWindowResult = this.Results.Last();
                return;
            }

            if (this.Results.Count > 0)
            {
                this.SelectedWindowResult = this.Results[(this.Results.IndexOf(this.SelectedWindowResult) + this.Results.Count - 1) % this.Results.Count];
            }
        }

        private void WindowNavigateToNextResult()
        {
            if (this.SelectedWindowResult == null && this.Results.Count > 0)
            {
                this.SelectedWindowResult = this.Results.First();
                return;
            }

            if (this.Results.Count > 0)
            {
                this.SelectedWindowResult = this.Results[(this.Results.IndexOf(this.SelectedWindowResult) + 1) % this.Results.Count];
            }
        }

        private void WindowHide()
        {
            Components.LivePreview.DeactivateLivePreview();
            this.WindowVisibility = false;
        }

        private void WindowShow()
        {
            this.SearchText = string.Empty;
            Components.LivePreview.DeactivateLivePreview();
            this.WindowVisibility = true;
            InteropAndHelpers.SetForegroundWindow(this.Hwnd);
        }

        public void SwitchToSelectedWindow()
        {
            if (this.SelectedWindowResult != null)
            {
                Components.LivePreview.DeactivateLivePreview();
                this.SelectedWindowResult.ResultWindow.SwitchToWindow();
                this.WindowHide();
            }
            else if (this.Results != null)
            {
                Components.LivePreview.DeactivateLivePreview();
                this.Results.First().ResultWindow.SwitchToWindow();
                this.WindowHide();
            }
        }

        private void SearchResultUpdated(object sender, Window.WindowListUpdateEventArgs e)
        {
            this.Results = WindowSearchController.Instance.SearchMatches;
        }
    }
}
