using Reader.Models;
using Reader.Services;
namespace Reader.ViewModels;

public class SearchViewModel : BaseViewModel
{
    protected readonly IDataStore<Book> DataStore;
    public SearchViewModel(IDataStore<Book> dataStore) : base(dataStore)
    {
        DataStore = dataStore;

    }
}