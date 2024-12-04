using GroupDocs.Parser;
using GroupDocs.Parser.Data;
using System.Diagnostics;
using Microsoft.Maui.Devices;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Reader.Models;

namespace Reader.Services.DocReaders
{
    public class Fb2DocumentReader:IDocumentReader
    {
        Parser parser { get; set; }
        string _filePath { get; set; }
        IEnumerable<MetadataItem> metadata { get;set; }
        public Fb2DocumentReader(string filePath)
        {
            _filePath = filePath;
            //ReadDocumentAsync();
        }

        public async Task ReadDocumentAsync()
        {
            try
            {
                // Выполнение на отдельном потоке
                await Task.Run(() =>
                {
                    parser = new Parser(_filePath);
                    metadata = parser.GetMetadata();
                });

                // Обработка результата на UI-потоке
                foreach (var property in metadata)
                {
                    Debug.WriteLine($"{property.Name}: {property.Value}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while reading document: {ex.Message}");
            }
        }

        public async Task <List<HtmlWebViewSource>> GetText(ObservableCollection<Title> tableOfContents)
        {
            var displayInfo = DeviceDisplay.MainDisplayInfo;
            Debug.WriteLine($"Width: {displayInfo.Width}, Height: {displayInfo.Height}");
            Debug.WriteLine($"Density: {displayInfo.Density}");
            Debug.WriteLine($"Orientation: {displayInfo.Orientation}");
            Debug.WriteLine($"Rotation: {displayInfo.Rotation}");



            return PageCreator.ExtractPagesWithFormatting(displayInfo.Width / displayInfo.Density, displayInfo.Height / displayInfo.Density, 16, 1.5, parser, tableOfContents);
        }

        public ImageSource GetCover()
        {
            var images = parser.GetImages();
            foreach (var image in images)
            {
                if (image != null)
                {
                    using (var imageStream = image.GetImageStream())
                    {
                        // Скопировать поток в новый MemoryStream
                        var memoryStream = new MemoryStream();
                        imageStream.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        // Создать ImageSource из копии
                        return ImageSource.FromStream(() => memoryStream);
                    }
                }
            }
            return null;
        }
        public string GetTitle()
        {  
            return metadata.FirstOrDefault(item => item.Name == "title").Value;
        }
        public string GetAuthor()
        {
            
            return  metadata.FirstOrDefault(item => item.Name == "author").Value;
        }
        public string GetFormat()
        {
            return "FB2";
        }
        public string GetFileSize()
        {
            var fileInfo = new FileInfo(_filePath);
            return ((double)fileInfo.Length/1024/1024).ToString("F1");
        }
    }

        
    
}
