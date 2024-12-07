using Reader.Models;
using Reader.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Xceed.Document.NET;
namespace Reader.ViewModels;

public class ContentViewModel : BaseViewModel
{
    protected readonly IDataStore<Book> DataStore;

    public Command<Title> ItemTapped { get; }
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
        ItemTapped=new Command<Title>(OnItemSelected);
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

    async void OnItemSelected(Title item)
    {

        if (item == null)
            return;  
        CurrentPageIndex=item.PageNumber-1;
        await Shell.Current.GoToAsync("..");


        // This will push the ItemDetailPage onto the navigation stack
        //await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
    }


}