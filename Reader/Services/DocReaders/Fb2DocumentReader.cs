using Fb2.Document;
using GroupDocs.Parser;
using GroupDocs.Parser.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Services.DocReaders
{
    public class Fb2DocumentReader:IDocumentReader
    {
        Parser parser { get; set; }

        public async void ReadDocument(string filePath)
        {
            parser = new Parser(filePath);
            var metadata = parser.GetMetadata();
            foreach (var property in metadata)
            {
                Console.WriteLine($"{property.Name}: {property.Value}");
            }
        }
        public string ExtractText()
        {
            return "";
        }

        public ImageSource GetCover()
        {
            var images = parser.GetImages();
            foreach (var image in images)
            {
                // Получаем первый рисунок, который может быть обложкой
                if (image != null)
                {
                    using (var imageStream = image.GetImageStream())
                    {
                        // Преобразуем в ImageSource для Xamarin
                        ImageSource coverImage = ImageSource.FromStream(() => imageStream);
                        return coverImage;
                    };
                }
            }
            return null;


        }
        public string GetTitle()
        {
            return "";
        }
        public IEnumerable<string> GetAuthor()
        {
            IEnumerable<string> ans = new List<string>();
            return ans;
        }
        public string GetCollection()
        {
            return "";
        }
        public string GetFormat()
        {
            return "";
        }
        public string GetFileSize(string filePath)
        {
            return "";
        }
    }

        
    
}
