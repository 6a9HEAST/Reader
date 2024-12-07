using Reader.Models;
using Reader.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace Reader.ViewModels;

public class SearchViewModel : BaseViewModel
{
    protected readonly IDataStore<Book> DataStore;
    public ICommand GoBackCommand { get; }
    public List<string> Pages { get; set; }
    public Command<SearchResult> ItemTapped { get; }
    public List<SearchResult> SearchResults {get;set;}

    private string _inputText;
    public string InputText
    {
        get => _inputText;
        set
        {
            _inputText = value;
            OnPropertyChanged(nameof(InputText));
        }
    }
    public int CurrentPageIndex
    {
        get => _sharedDataService.CurrentPageIndex;
        set => _sharedDataService.CurrentPageIndex = value;
    }
    private readonly SharedDataService _sharedDataService;
    public ICommand SubmitCommand { get; }
    public SearchViewModel(IDataStore<Book> dataStore, SharedDataService sharedDataService) : base(dataStore)
    {
        GoBackCommand = new Command(GoBack);
        DataStore = dataStore;
        SubmitCommand = new Command(OnSubmit);
        ItemTapped = new Command<SearchResult>(OnItemSelected);
        _sharedDataService = sharedDataService;
    }

    public async Task InitializeAsync(List<string> _pages)
    {
        Pages = _pages;
    }

    private async void GoBack()
    {
        await Shell.Current.GoToAsync("..");
        
    }

    private void OnSubmit()
    {

       SearchResults = Pages
            .SelectMany((line, index) => line.Split(' ')
                                             .Where(word => string.Equals(word, InputText, StringComparison.OrdinalIgnoreCase))
                                             .Select(word => new SearchResult
                                             {
                                                 Text = word,
                                                 PageNumber = index+1
                                             }))
            .ToList();
        OnPropertyChanged(nameof(SearchResults));
        
    }

    async void OnItemSelected(SearchResult item)
    {

        if (item == null)
            return;
        CurrentPageIndex = item.PageNumber - 1;
        SearchResults.Clear();
        
        await Shell.Current.GoToAsync("..");
        InputText = "";

        // This will push the ItemDetailPage onto the navigation stack
        //await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
    }


}