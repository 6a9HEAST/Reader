using Reader.Models;
using System.Diagnostics;


namespace Reader.Services
{
    public class BookDataStore : IDataStore<Book>
    {
        readonly List<Book> items;


        public BookDataStore()
        {
            
            //items = new List<Book>();
            //InitializeDataStore(); // Вызов основного метода инициализации
        }

        private async void InitializeDataStore()
        {
            
        }

        public async Task<bool> AddItemAsync(Book item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Book item)
        {
            var oldItem = items.Where((Book arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Book arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Book> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Book>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}