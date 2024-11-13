using Reader.Models;
using Reader.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xceed.Document.NET;
namespace Reader.ViewModels;

public class ContentViewModel : BaseViewModel
{
    protected readonly IDataStore<Book> DataStore;
    public ObservableCollection<Title> Titles { get; set; }
    public ICommand GoBackCommand { get; }
    public Command<Title> ToggleExpandCommand { get; }

    public ContentViewModel(IDataStore<Book> dataStore) : base(dataStore)
    {
        GoBackCommand = new Command(GoBack);
        DataStore = dataStore;
        ToggleExpandCommand = new Command<Title>(ToggleExpand);
        Titles = new ObservableCollection<Title>
        {
            new Title
            {
                Name = "Chapter 1",
                SubItems = new ObservableCollection<Title>
                {
                    new Title { Name = "Section 1.1" },
                    new Title
                    {
                        Name = "Section 1.2",
                        SubItems = new ObservableCollection<Title>
                        {
                            new Title { Name = "Subsection 1.2.1" }
                        }
                    }
                }
            }
        };

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