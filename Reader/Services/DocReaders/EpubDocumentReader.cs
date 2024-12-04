using EpubSharp;
using GroupDocs.Parser;
using GroupDocs.Parser.Data;
using RtfPipe;
using System.Diagnostics;
using Parser = GroupDocs.Parser.Parser;

namespace Reader.Services.DocReaders
{
    public class EpubDocumentReader //: IDocumentReader
    {
        Parser parser { get; set; }
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
                byte[] fileBytes = await File.ReadAllBytesAsync(_filePath);

                // Parse the document using Parser with MemoryStream
                using (MemoryStream ms = new MemoryStream(fileBytes))
                {
                    parser = new Parser(ms);
                }

                // Create a temporary file
                string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".epub");
                await File.WriteAllBytesAsync(tempFilePath, fileBytes);

                try
                {
                    // Read the EPUB using EpubReader
                    epub = EpubReader.Read(tempFilePath);
                }
                finally
                {
                    // Ensure the temporary file is deleted
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while reading document: {ex.Message}");
            }


        }
        //public async Task<List<FormattedString>> GetText()
        //{
        //    DisplayInfo displayInfo = new DisplayInfo();


        //    return PageCreator.ExtractPagesWithFormatting(displayInfo.Width, displayInfo.Height, 25, 1.5, parser);
        //}

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
