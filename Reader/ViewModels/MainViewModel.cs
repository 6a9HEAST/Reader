using Microsoft.VisualBasic;
using Reader.Models;
using Reader.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Reader.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Book _selectedItem;

        public ObservableCollection<Book> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Book> ItemTapped { get; }
        

        public MainViewModel(IDataStore<Book> dataStore) : base(dataStore)
        {
            
            Title = "About";
            Items = new ObservableCollection<Book>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            
            LoadItemsCommand.Execute(null);
            ItemTapped = new Command<Book>(OnItemSelected);
            

            AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
                OnPropertyChanged(nameof(Items));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("LoadItemsCommand executed");
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
            LoadItemsCommand.Execute(null);
        }

        public Book SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            var book = await FileScanner.GetBookFromFile();

            if (book != null) Items.Add(book);
        }

        async void OnItemSelected(Book item)
        {
            
                await Shell.Current.GoToAsync("ReadView");

            // This will push the ItemDetailPage onto the navigation stack
            //await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
        public ICommand OpenWebCommand { get; }
    }
}
