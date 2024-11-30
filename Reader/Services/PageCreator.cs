using System.Text;
using GroupDocs.Parser;
using GroupDocs.Parser.Options;

namespace Reader.Services
{
    public static class PageCreator
    {
        public static List<FormattedString> ExtractPagesWithFormatting(
            double pageWidth, double pageHeight, double fontSize, double lineHeight, Parser parser)
        {
            //Debug.WriteLine("Начало создания страниц");
            //var watch = Stopwatch.StartNew();

            if (!parser.Features.Text)
                throw new NotSupportedException("Документ не поддерживает извлечение текста.");

            var options = new FormattedTextOptions(FormattedTextMode.PlainText);
            string fullText;

            using (var reader = parser.GetFormattedText(options))
            {
                fullText = reader.ReadToEnd();
            }

            //Debug.WriteLine("Текст прочитан.");
            //watch.Stop();
            //Debug.WriteLine($"Время чтения текста: {watch.ElapsedMilliseconds} мс");
            //watch.Reset();
            //watch.Start();

            var paragraphs = fullText.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            //Debug.WriteLine($"Извлечено параграфов: {paragraphs.Length}");

            var aggregatedBlocks = AggregateTextBlocks(paragraphs, 10);
            //Debug.WriteLine($"Агрегированных блоков: {aggregatedBlocks.Count}");

            var localPagesLists = new List<List<FormattedString>>();

            Parallel.ForEach(aggregatedBlocks, aggregatedBlock =>
            {
                var localPages = new List<FormattedString>();
                var blockPages = ProcessTextBlock(aggregatedBlock, pageWidth, pageHeight, fontSize, lineHeight);
                localPages.AddRange(blockPages);
                lock (localPagesLists)
                {
                    localPagesLists.Add(localPages);
                }
            });

            var pages = new List<FormattedString>();
            foreach (var localPages in localPagesLists)
            {
                pages.AddRange(localPages);
            }

            //watch.Stop();
            //Debug.WriteLine($"Время обработки: {watch.ElapsedMilliseconds} мс");

            return pages;
        }

        private static List<string> AggregateTextBlocks(string[] paragraphs, int chunkSize)
        {
            var aggregatedBlocks = new List<string>();
            var stringBuilder = new StringBuilder();

            foreach (var paragraph in paragraphs)
            {
                stringBuilder.AppendLine(paragraph);
                if (stringBuilder.ToString().Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Length >= chunkSize)
                {
                    aggregatedBlocks.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
            }

            if (stringBuilder.Length > 0)
            {
                aggregatedBlocks.Add(stringBuilder.ToString());
            }

            return aggregatedBlocks;
        }

        private static List<FormattedString> ProcessTextBlock(
            string textBlock, double pageWidth, double pageHeight, double fontSize, double lineHeight)
        {
            var pages = new List<FormattedString>();
            var lines = SplitTextIntoLines(textBlock, pageWidth, fontSize);

            var currentPageText = new StringBuilder();
            double currentHeight = 0;

            foreach (var line in lines)
            {
                var lineHeightWithSpacing = fontSize * 1.2 * lineHeight;

                if (currentHeight + lineHeightWithSpacing > pageHeight)
                {
                    pages.Add(CreateFormattedString(currentPageText.ToString()));
                    currentPageText.Clear();
                    currentHeight = 0;
                }

                if (currentPageText.Length > 0)
                {
                    currentPageText.AppendLine();
                }

                currentPageText.Append(line);
                currentHeight += lineHeightWithSpacing;
            }

            if (currentPageText.Length > 0)
            {
                pages.Add(CreateFormattedString(currentPageText.ToString()));
            }

            return pages;
        }

        private static List<string> SplitTextIntoLines(string text, double pageWidth, double fontSize)
        {
            var lines = new List<string>();
            var currentLine = new StringBuilder();
            double currentLineWidth = 0;

            double averageCharWidth = fontSize * 0.6;
            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var wordWidth = word.Length * averageCharWidth;

                if (currentLineWidth + wordWidth > pageWidth)
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                    currentLineWidth = 0;
                }

                if (currentLine.Length > 0)
                {
                    currentLine.Append(' ');
                    currentLineWidth += averageCharWidth; // Add space width
                }

                currentLine.Append(word);
                currentLineWidth += wordWidth;
            }

            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString());
            }

            return lines;
        }

        private static FormattedString CreateFormattedString(string text)
        {
            var formattedString = new FormattedString();

            // Пример: добавление разных стилей в зависимости от содержимого
            foreach (var line in text.Split('\n'))
            {
                if (line.StartsWith("# ")) // Условный заголовок
                {
                    formattedString.Spans.Add(new Span
                    {
                        Text = line.Substring(2) + "\n",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold
                    });
                }
                else
                {
                    formattedString.Spans.Add(new Span
                    {
                        Text = line + "\n",
                        FontSize = 16,
                        FontAttributes = FontAttributes.None
                    });
                }
            }

            return formattedString;
        }
    }
}