using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Services
{
    public interface IDocumentReader
    {
        void ReadDocument(string filePath);
        string ExtractText(); // Метод для извлечения текста
        ImageSource GetCover(); // Метод для получения обложки
        string GetTitle(); // Название
        IEnumerable<string> GetAuthor(); // Автор
        string GetCollection(); // Собрание (если есть)
        string GetFormat(); // Формат документа (например, EPUB, FB2)
        string GetFileSize(string filePath); // Размер файла
    }
}
