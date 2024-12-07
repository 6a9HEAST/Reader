using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;
using Android.Text;
using Aspose.Words.Saving;
using HtmlAgilityPack;
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
        private bool _isInitialized = false;
        public int CurrentPageIndex
        {
            get { return _sharedDataService.CurrentPageIndex; }
            set
            {
                _sharedDataService.CurrentPageIndex = value;
                
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
        

        public string PageNumberIndicator { get; set; }

        public HtmlWebViewSource CurrentPage
        {
            get
            {
                if (_pages != null && CurrentPageIndex >= 0 && CurrentPageIndex < _pages.Count)
                {
                    PageNumberIndicator = (CurrentPageIndex + 1) + " из " + _pages.Count;
                    OnPropertyChanged(nameof(PageNumberIndicator));
                   
                    return _pages[CurrentPageIndex];
                }
                    
                return null;
            }
        }

        private readonly SharedDataService _sharedDataService;

        public ReadViewModel(IDataStore<Book> dataStore, SharedDataService sharedDataService) : base(dataStore)
        {
            _sharedDataService = sharedDataService;
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
            OnPropertyChanged(nameof(CurrentPage));
            if (_isInitialized) return;
            _filePath = path;
            _name = name;
            var reader = DocumentReaderFactory.GetReader(_filePath);
            await reader.ReadDocumentAsync();
            OnPropertyChanged(nameof(_name));
            TableOfContents = new ObservableCollection<Title>();
            Pages = await reader.GetText(TableOfContents);
            _isInitialized = true;
            //foreach (var table in TableOfContents)
            //{
            //    Debug.WriteLine(table.Name);
            //}
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
            Preferences.Remove("SearchData");
            _isInitialized = false;
            await Shell.Current.GoToAsync("..");
            IsOverlayVisible = false;
        }

        private async void OpenSearch()
        {
            List<string> data = new List<string>();
            foreach (var page in Pages) 
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(page.Html);
                string plainText = doc.DocumentNode.SelectSingleNode("//body")?.InnerText ?? string.Empty;
                data.Add(plainText);
            }
            
            var json = JsonSerializer.Serialize(data);
            Preferences.Set("SearchData", json);
            await Shell.Current.GoToAsync("SearchView");
        }
        private async void OpenContent()
        {
            var json = JsonSerializer.Serialize(TableOfContents); // Преобразуем в JSON
            await Shell.Current.GoToAsync($"ContentView?data={Uri.EscapeDataString(json)}");
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
                Debug.WriteLine(CurrentPageIndex);
            }
        }

        private void GoToNextPage()
        {
            if (CurrentPageIndex < Pages.Count - 1)
            {

                CurrentPageIndex++;
                Debug.WriteLine(CurrentPageIndex);
            }
        }

        
        
    }
}
