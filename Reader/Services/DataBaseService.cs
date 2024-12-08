using Reader.Models;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Aspose.Pdf;



namespace Reader.Services
{
    public class DataBaseService
    {
        
        
        public async Task<ObservableCollection<Book>> Read()
        {
            
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "data.json");
            
            //File.Delete(filePath);
            if (!File.Exists(filePath)) return null;
            string jsonFromFile = File.ReadAllText(filePath);
            List<string> loadedData=new List<string>();
            try
            {
                loadedData = JsonSerializer.Deserialize<List<string>>(jsonFromFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message + "\ndeleting the file");
                File.Delete(filePath);
                return null;
            }
            ObservableCollection <Book> results = new ObservableCollection<Book>();
            foreach (var item in loadedData)
            {
                try
                {
                    var reader = DocumentReaderFactory.GetReader(item);
                    await reader.ReadDocumentAsync();
                    var author = reader.GetAuthor();
                    var title = reader.GetTitle();
                    var type = reader.GetFormat();
                    var size = reader.GetFileSize();
                    var cover = reader.GetCover();//ImageSource.FromFile("dotnet_bot.png");//

                    var book = new Book()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Author = author,
                        Name = title,
                        Type = type + ",",
                        Size = size + " МБ",
                        Cover = cover,
                        Path = item,
                        Progress = 0,
                        LastTimeOpened = DateTime.Now
                    };
                    results.Add(book);
                }
                catch (Exception ex) 
                {
                    Debug.WriteLine(ex.Message);
                }
                
            }
            return results;
            //foreach (var item in loadedData)
            //{
            //    item.Cover=LoadCachedImage(item.Name+item.Type+".jpg");
            //}
            //return loadedData;
        }

        public async void Write(Book book)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "data.json");
            List<string> books = new List<string>(); ;
            
                if (File.Exists(filePath))
                {
                string jsonFromFile;
                using (var reader = new StreamReader(filePath))
                {
                    jsonFromFile = await reader.ReadToEndAsync();
                }
                books = JsonSerializer.Deserialize<List<string>>(jsonFromFile) ?? new List<string>();
            }
                if (books == null) books = new List<string>();
                books.Add(book.Path);
                string json = JsonSerializer.Serialize(books);
            using (var writer = new StreamWriter(filePath, false)) // false - перезаписываем файл
            {
                await writer.WriteAsync(json);
            }
            // });

        }
            
            
            //ObservableCollection<Book> items;
            //items = Read();
            //if (items == null) items = new ObservableCollection<Book>();
            //items.Add(book);
            //foreach (var item in items) 
            //{
            //    var imageStream = await ImageSourceToStreamAsync(item.Cover);
            //    await CacheImageAsync(imageStream, item.Name + item.Type + ".jpg");
            //    item.Cover = null;
            //}
            //string json = JsonSerializer.Serialize(items);
            //if (!File.Exists(filePath)) File.Create(filePath);
            //File.WriteAllText(filePath,json);
        }

        //public ImageSource LoadCachedImage(string fileName)
        //{
        //    string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        //    // Проверка существования файла
        //    if (File.Exists(filePath))
        //    {
        //        return ImageSource.FromFile(filePath);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Файл изображения не найден.");
        //        return null;
        //    }
        //}

        //public async Task CacheImageAsync(Stream imageStream, string fileName)
        //{

        //        // Путь для сохранения файла
        //    string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        //    if (File.Exists(filePath)) return;
        //    // Создание файла
        //    using (var fileStream = File.Create(filePath))
        //    {
        //        // Копирование данных из исходного потока в файл
        //        await imageStream.CopyToAsync(fileStream);
        //    }

        //    Console.WriteLine($"Изображение сохранено по пути: {filePath}");
        //}

        //public async Task<Stream> ImageSourceToStreamAsync(ImageSource imageSource)
        //{
        //    switch (imageSource)
        //    {
        //        case FileImageSource fileImageSource:
        //            // Если изображение из файла
        //            if (File.Exists(fileImageSource.File))
        //                return File.OpenRead(fileImageSource.File);
        //            break;

        //        case StreamImageSource streamImageSource:
        //            // Если изображение уже поток
        //            return await streamImageSource.Stream.Invoke(CancellationToken.None);
        //    }

        //    Console.WriteLine("ImageSource не поддерживается или источник недоступен.");
        //    return null;
        //}
    }

