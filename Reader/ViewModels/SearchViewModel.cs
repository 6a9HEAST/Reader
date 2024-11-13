using Reader.Models;
using Reader.Services;
using System.Windows.Input;
namespace Reader.ViewModels;

public class SearchViewModel : BaseViewModel
{
    protected readonly IDataStore<Book> DataStore;
    public ICommand GoBackCommand { get; }
    public SearchViewModel(IDataStore<Book> dataStore) : base(dataStore)
    {
        GoBackCommand = new Command(GoBack);
        DataStore = dataStore;

    }

    private async void GoBack()
    {
        await Shell.Current.GoToAsync("..");
        
    }

}