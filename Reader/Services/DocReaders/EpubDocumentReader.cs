using EpubSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Services.DocReaders
{
    public class EpubDocumentReader : IDocumentReader
    {
        EpubBook epub;

        public void ReadDocument(string filePath)
        {
            epub = EpubReader.Read(filePath);
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

        public IEnumerable<string> GetAuthor()
        {
            return epub.Authors;
        }

        public string GetCollection()
        {
            return ""; // доделать
        }

        public string GetFormat()
        {
            return "EPUB";
        }

        public string GetFileSize(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length.ToString();
        }
    }
}
