using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;
using Reader.Models;



namespace Reader.Services
{
    public static class FileScanner
    {
        public static async Task<string> SelectFileAsync()
        {            
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Выберите файл",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
               { DevicePlatform.Android, new[] { "application/octet-stream", "text/plain", "application/fb2", "application/epub+zip", "application/x-mobipocket-ebook" } }
            })
            });

            if (result != null)
            {
                Debug.WriteLine($"Выбран файл: {result.FullPath}");
                return result.FullPath;
            }
            else
            {
                Debug.WriteLine("Файл не выбран.");
                return null;
            }
        }

        public static async Task<Book> GetBookFromFile()
        {            
            string file = await FileScanner.SelectFileAsync();
            if (file == null) return null;
            try
            {
            var reader = DocumentReaderFactory.GetReader(file);
            await reader.ReadDocumentAsync();
            var author = reader.GetAuthor();
            var title = reader.GetTitle();
            var type = reader.GetFormat();
            var size = reader.GetFileSize();
            var cover =  reader.GetCover();//ImageSource.FromFile("dotnet_bot.png");//
            
                var book= new Book()
                {
                    Id = Guid.NewGuid().ToString(),
                    Author = author,
                    Name = title,
                    Type = type + ",",
                    Size = size + " МБ",
                    Cover = cover,
                    Path = file,
                    LastTimeOpened = DateTime.Now
                };
                return book;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("Ошибка при добавлении книги:"+ ex.Message);
                #if ANDROID
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Ошибка при добавлении  книги", Android.Widget.ToastLength.Short)?.Show();
                #endif
            }
            return null;
        }
    
    }
}