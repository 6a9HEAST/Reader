using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reader.Models;

namespace Reader.Services
{
    public interface IDocumentReader
    {
        
        Task ReadDocumentAsync();
        Task<List<HtmlWebViewSource>> GetText(ObservableCollection<Title> tableOfContents); // Метод для извлечения текста
        ImageSource GetCover(); // Метод для получения обложки
        string GetTitle(); // Название
        string GetAuthor(); // Автор
        string GetFormat(); // Формат документа (например, EPUB, FB2)
        string GetFileSize(); // Размер файла
    }
}
