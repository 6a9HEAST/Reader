using EpubSharp;
using GroupDocs.Parser;
using GroupDocs.Parser.Data;
using System.Diagnostics;

namespace Reader.Services.DocReaders
{
    public class EpubDocumentReader : IDocumentReader
    {
        EpubBook epub { get; set; }
        string _filePath { get; set; }
        public EpubDocumentReader(string filePath)
        {
            _filePath = filePath;
            
        }

        public async Task ReadDocumentAsync()
        {
            try
            {
                // Выполнение на отдельном потоке
                await Task.Run(() =>
                {
                    epub = EpubReader.Read(_filePath);

                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while reading document: {ex.Message}");
            }
        }
        public string ExtractText()
        {
            
            return epub.ToPlainText();
        }

        public ImageSource GetCover()
        {
            var coverImage = epub.CoverImage;
            return coverImage != null ? ImageSource.FromStream(() => new MemoryStream(coverImage)) : null;
        }
        public string GetTitle()
        {
            return epub.Title ?? "Без названия";
        }

        public string GetAuthor()
        {
            return epub.Authors.FirstOrDefault<string>();
        }

        public string GetFormat()
        {
            return "EPUB";
        }

        public string GetFileSize()
        {
            var fileInfo = new FileInfo(_filePath);
            return ((double)fileInfo.Length / 1024 / 1024).ToString("F1");
        }
    }
}
