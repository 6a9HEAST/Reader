using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Reader.Models;
using Reader.Services;

namespace Reader.ViewModels
{
    public class ReadViewModel : BaseViewModel
    {
        private bool _isOverlayVisible;
        //public List<FormattedString> Pages { get; set; }

        public bool IsOverlayVisible
        {
            get => _isOverlayVisible;
            set => SetProperty(ref _isOverlayVisible, value);
        }
        public string _filePath{get;set;}

        public ICommand ShowOverlayCommand { get; }
        public ICommand HideOverlayCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand OpenSearchCommand { get; }
        public ICommand OpenContentCommand { get; }

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

        private List<FormattedString> _myPages = new List<FormattedString>();
        public List<FormattedString> MyPages
        {
            get { return _myPages; }
            set
            {
                _myPages = value;
                OnPropertyChanged(nameof(MyPages));
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public FormattedString CurrentPage
        {
            get
            {
                if (_myPages != null && _currentPageIndex >= 0 && _currentPageIndex < _myPages.Count)
                    return _myPages[_currentPageIndex];
                return null;
            }
        }

        public ReadViewModel(IDataStore<Book> dataStore) : base(dataStore)
        {
            
            

            

            IsOverlayVisible = false;
            DataStore = dataStore;
            ShowOverlayCommand = new Command(ShowOverlay);
            HideOverlayCommand = new Command(HideOverlay);
            GoBackCommand = new Command(GoBack);
            OpenSearchCommand = new Command(OpenSearch);
            OpenContentCommand= new Command(OpenContent);
        }

        public async Task InitializeAsync(string path)
        {
            _filePath = path;
            var reader = DocumentReaderFactory.GetReader(_filePath);
            await reader.ReadDocumentAsync();
            MyPages = await reader.GetText();

            foreach (var page in MyPages)
            {
                Debug.WriteLine(page);
            }

            // Здесь вы можете добавить логику загрузки книги
            //Console.WriteLine($"Book Path Initialized: {path}");
            Debug.WriteLine(CurrentPageIndex);
        }
        
        private void ShowOverlay()
        {
            //CurrentPageIndex--;
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

    }
}
