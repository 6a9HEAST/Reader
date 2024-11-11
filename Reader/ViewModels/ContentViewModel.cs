using Reader.Models;
using Reader.Services;
namespace Reader.ViewModels;

public class ContentViewModel : BaseViewModel
{
    protected readonly IDataStore<Book> DataStore;
    public ContentViewModel(IDataStore<Book> dataStore) : base(dataStore)
    {
        DataStore = dataStore;

    }
}