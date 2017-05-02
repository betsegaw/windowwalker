using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowWalker.Components;

namespace WindowWalker.ViewModels
{
    class WindowWalkerViewModel: MVVMHelpers.PropertyChangedBase
    {
        private string _searchText = string.Empty;
        private List<WindowSearchResult> _results = new List<WindowSearchResult>();
        private static WindowWalkerViewModel _instance;

        public string SearchText {
            get => _searchText;

            set {
                _searchText = value;
                WindowSearchController.Instance.SearchText = value;
                NotifyPropertyChanged("SearchText");
            }
        }

        public static WindowWalkerViewModel Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new WindowWalkerViewModel();
                }

                return _instance;
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

        private WindowWalkerViewModel()
        {
            WindowSearchController.Instance.OnSearchResultUpdate += SearchResultUpdated;
            OpenWindows.Instance.UpdateOpenWindowsList();
        }

        private void SearchResultUpdated(object sender, Window.WindowListUpdateEventArgs e)
        {
            this.Results = WindowSearchController.Instance.SearchMatches;
        }
    }
}
