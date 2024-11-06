using Reader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reader.Services
{
    public class BookDataStore : IDataStore<Book>
    {
        readonly List<Book> items;

        public BookDataStore()
        {
            items = new List<Book>()
            {
                new Book { Id = Guid.NewGuid().ToString(), Author = "Author1", Name="Name1", Collection="Collection1", Type="FB2",Size=54 },
                new Book { Id = Guid.NewGuid().ToString(), Author = "Author2", Name="Name2", Collection="Collection2", Type="DOCX",Size=5000 }
            };
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