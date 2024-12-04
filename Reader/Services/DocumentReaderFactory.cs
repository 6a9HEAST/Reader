using Reader.Services.DocReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reader.Services
{
    public class DocumentReaderFactory
    {
        public static IDocumentReader GetReader(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                //".epub" => new EpubDocumentReader(filePath),
                ".fb2" => new Fb2DocumentReader(filePath),
                //".mobi" => new MobiDocumentReader(filePath),
                //".doc" => new DocDocumentReader(filePath),
                //".docx" => new DocxDocumentReader(filePath),
                //".rtf" => new RtfDocumentReader(filePath),
                //".txt" => new TxtDocumentReader(filePath),
                //".chm" => new ChmDocumentReader(filePath),
                _ => throw new NotSupportedException($"Unsupported format: {extension}")
            };
        }
    }

}
