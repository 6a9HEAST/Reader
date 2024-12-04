using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Aspose.Words.Saving;
using Microsoft.Maui.Controls;
using Reader.Models;
using Reader.Services;
using Xceed.Document.NET;

namespace Reader.ViewModels
{
    public class ReadViewModel : BaseViewModel
    {
        private bool _isOverlayVisible;
        public bool IsOverlayVisible
        {
            get => _isOverlayVisible;
            set => SetProperty(ref _isOverlayVisible, value);
        }

        private ObservableCollection<Title> tableOfContents;
        public ObservableCollection<Title> TableOfContents
        {
            get => tableOfContents;
            set
            {
                if (tableOfContents != value)
                {
                    tableOfContents = value;
                    OnPropertyChanged(nameof(TableOfContents));
                }
            }
        }
        public string _filePath{get;set;}
        public WebView _webView { get; set; }
        public ICommand ShowOverlayCommand { get; }
        public ICommand HideOverlayCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand OpenSearchCommand { get; }
        public ICommand OpenContentCommand { get; }
        
        public ICommand GoToPreviousPageCommand { get; }
        public ICommand GoToNextPageCommand { get; }
        public string _name { get; set; }

        protected readonly IDataStore<Book> DataStore;

        public event PropertyChangedEventHandler PropertyChanged;

        private int _currentPageIndex;
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                _currentPageIndex = value;
                OnPropertyChanged(nameof(CurrentPageIndex));
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        private List<HtmlWebViewSource> _pages = new List<HtmlWebViewSource>();
        public List<HtmlWebViewSource> Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                OnPropertyChanged(nameof(Pages));
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
        private HtmlWebViewSource _htmlPage;
        public HtmlWebViewSource HtmlPage
        {
            get => _htmlPage;
            set
            {
                _htmlPage = value;
                OnPropertyChanged(nameof(HtmlPage));
            }
        }

        public string PageNumberIndicator { get; set; }

        public HtmlWebViewSource CurrentPage
        {
            get
            {
                if (_pages != null && _currentPageIndex >= 0 && _currentPageIndex < _pages.Count)
                {
                    PageNumberIndicator = (_currentPageIndex+1) + " из " + _pages.Count;
                    OnPropertyChanged(nameof(PageNumberIndicator));
                    return _pages[_currentPageIndex];
                }
                    
                return null;
            }
        }

        public ReadViewModel(IDataStore<Book> dataStore) : base(dataStore)
        {
            IsOverlayVisible = false;
            DataStore = dataStore;
            GoToPreviousPageCommand = new Command(() => GoToPreviousPage());
            GoToNextPageCommand = new Command(() => GoToNextPage());
            ShowOverlayCommand = new Command(ShowOverlay);
            HideOverlayCommand = new Command(HideOverlay);
            GoBackCommand = new Command(GoBack);
            OpenSearchCommand = new Command(OpenSearch);
            OpenContentCommand= new Command(OpenContent);
        }

        public async Task InitializeAsync(string path,string name)
        {
            _filePath = path;
            _name = name;
            var reader = DocumentReaderFactory.GetReader(_filePath);
            await reader.ReadDocumentAsync();
            OnPropertyChanged(nameof(_name));
            TableOfContents = new ObservableCollection<Title>();
            Pages = await reader.GetText(TableOfContents);
            foreach (var table in TableOfContents)
            {
                Debug.WriteLine(table.Name);
            }
        }
        
        private void ShowOverlay()
        {
            IsOverlayVisible = true;
        }

        private void HideOverlay()
        {
            IsOverlayVisible = false;
        }

        private async void GoBack()
        {
            await Shell.Current.GoToAsync("..");
            IsOverlayVisible = false;
        }

        private async void OpenSearch()
        {
            await Shell.Current.GoToAsync("SearchView");
        }
        private async void OpenContent()
        {
            await Shell.Current.GoToAsync("ContentView");
        }

        private void GoToPreviousPage()
        {
            //if (_webView?.CanGoBack==true)
            //{
            //    _webView.GoBack();
            //}

            if (CurrentPageIndex > 0)
            {
                CurrentPageIndex--;
                Debug.WriteLine(_currentPageIndex);
            }
        }

        private void GoToNextPage()
        {
            if (CurrentPageIndex < Pages.Count - 1)
            {

                CurrentPageIndex++;
                Debug.WriteLine(_currentPageIndex);
            }
        }

        public async Task OnWebViewReadyAsync(WebView webView)
        {
            // Теперь WebView готов, можно выполнять операции
            _webView = webView;

            // Дополнительная логика
            await Task.CompletedTask;
        }
    }
}
