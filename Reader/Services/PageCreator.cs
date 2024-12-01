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

            var options = new FormattedTextOptions(FormattedTextMode.Markdown);
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

            var localPagesLists = new List<(int Index, List<FormattedString> Pages)>();

            Parallel.ForEach(aggregatedBlocks.Select((block, index) => (block, index)), item =>
            {
                var localPages = ProcessTextBlock(item.block, pageWidth, pageHeight, fontSize, lineHeight);
                lock (localPagesLists)
                {
                    localPagesLists.Add((item.index, localPages));
                }
            });

            var pages = localPagesLists
                .OrderBy(pair => pair.Index) // Сортируем по индексу
                .SelectMany(pair => pair.Pages)
                .ToList();

            //var pages = new List<FormattedString>();
            //foreach (var localPages in localPagesLists)
            //{
            //    pages.AddRange(localPages);
            //}

            //watch.Stop();
            //Debug.WriteLine($"Время обработки: {watch.ElapsedMilliseconds} мс");

            return pages;
        }

        private static List<string> AggregateTextBlocks(string[] paragraphs, int chunkSize)
        {
            var aggregatedBlocks = new List<string>();
            var stringBuilder = new StringBuilder();
            int currentLineCount = 0;

            foreach (var paragraph in paragraphs)
            {
                var lines = paragraph.Split('\n'); // Считаем строки в параграфе
                currentLineCount += lines.Length;

                stringBuilder.AppendLine(paragraph);

                if (currentLineCount >= chunkSize)
                {
                    aggregatedBlocks.Add(stringBuilder.ToString().Trim()); // Добавляем текущий блок
                    stringBuilder.Clear();
                    currentLineCount = 0;
                }
            }

            if (stringBuilder.Length > 0) // Добавляем оставшийся текст
            {
                aggregatedBlocks.Add(stringBuilder.ToString().Trim());
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
                var lineHeightWithSpacing = fontSize * lineHeight;

                // Проверяем, помещается ли текущая строка на страницу
                if (currentHeight + lineHeightWithSpacing > pageHeight)
                {
                    // Создаем текущую страницу
                    pages.Add(CreateFormattedString(currentPageText.ToString(), fontSize));
                    currentPageText.Clear();
                    currentHeight = 0;
                }

                // Добавляем строку на текущую страницу
                if (currentPageText.Length > 0)
                {
                    currentPageText.AppendLine();
                }

                currentPageText.Append(line);
                currentHeight += lineHeightWithSpacing;
            }


            if (currentPageText.Length > 0) // Добавляем оставшийся текст
            {
                pages.Add(CreateFormattedString(currentPageText.ToString(), fontSize));
            }

            return pages;
        }


        private static List<string> SplitTextIntoLines(string text, double pageWidth, double fontSize)
        {
            var lines = new List<string>();
            var currentLine = new StringBuilder();
            double currentLineWidth = 0;

            double averageCharWidth = fontSize * 0.6; // Средняя ширина символа
            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var wordWidth = word.Length * averageCharWidth;

                // Если слово само по себе слишком длинное, оно должно быть перенесено
                if (wordWidth > pageWidth)
                {
                    if (currentLine.Length > 0)
                    {
                        lines.Add(currentLine.ToString().Trim());
                        currentLine.Clear();
                        currentLineWidth = 0;
                    }

                    // Разбиваем длинное слово
                    for (int i = 0; i < word.Length; i++)
                    {
                        currentLine.Append(word[i]);
                        currentLineWidth += averageCharWidth;

                        if (currentLineWidth > pageWidth)
                        {
                            lines.Add(currentLine.ToString());
                            currentLine.Clear();
                            currentLineWidth = 0;
                        }
                    }
                    continue; // Переходим к следующему слову
                }

                if (currentLineWidth + wordWidth > pageWidth)
                {
                    lines.Add(currentLine.ToString().Trim());
                    currentLine.Clear();
                    currentLineWidth = 0;
                }

                if (currentLine.Length > 0)
                {
                    currentLine.Append(' ');
                    currentLineWidth += averageCharWidth; // Учитываем пробел
                }

                currentLine.Append(word);
                currentLineWidth += wordWidth;
            }

            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString().Trim());
            }

            return lines;
        }

        private static FormattedString CreateFormattedString(string text,double fontSize)
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
                        FontSize = fontSize+4,
                        FontAttributes = FontAttributes.Bold
                    });
                }
                else if (line.StartsWith("## ")) // Условный заголовок
                {
                    formattedString.Spans.Add(new Span
                    {
                        Text = line.Substring(2) + "\n",
                        FontSize = fontSize + 3,
                        FontAttributes = FontAttributes.Bold
                    });
                }
                else
                {
                    formattedString.Spans.Add(new Span
                    {
                        Text = line + "\n",
                        FontSize = fontSize,
                        FontAttributes = FontAttributes.None
                    });
                }
            }

            return formattedString;
        }
    }
}