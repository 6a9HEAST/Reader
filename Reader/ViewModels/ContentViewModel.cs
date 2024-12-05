using Reader.Models;
using Reader.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xceed.Document.NET;
namespace Reader.ViewModels;

public class ContentViewModel : BaseViewModel
{
    protected readonly IDataStore<Book> DataStore;
    private ObservableCollection<Title> titles { get; set; }
    public ObservableCollection<Title> Titles 
    { 
        get => titles;
        set
        {
            titles = value;
            OnPropertyChanged(nameof(Titles));
        }
    }

    public int CurrentPageIndex
    {
        get => _sharedDataService.CurrentPageIndex;
        set => _sharedDataService.CurrentPageIndex = value;
    }

    public ICommand GoBackCommand { get; }
    public Command<Title> ToggleExpandCommand { get; }

    private readonly SharedDataService _sharedDataService;

    public ContentViewModel(IDataStore<Book> dataStore, SharedDataService sharedDataService) : base(dataStore)
    {
        _sharedDataService = sharedDataService;
        GoBackCommand = new Command(GoBack);
        DataStore = dataStore;
        ToggleExpandCommand = new Command<Title>(ToggleExpand);

    }

    public async Task InitializeAsync(ObservableCollection<Title> _tableOfContents)
    {
        Titles = _tableOfContents;
    }
    private async void GoBack()
    {
        await Shell.Current.GoToAsync("..");

    }

    private void ToggleExpand(Title item)
    {
        item.IsExpanded = !item.IsExpanded;
    }

    
}