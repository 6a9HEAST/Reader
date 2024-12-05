using GroupDocs.Parser;
using GroupDocs.Parser.Options;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using Reader.Models;
using System.Diagnostics;

namespace Reader.Services
{
    public static class PageCreator
    {

        public static List<HtmlWebViewSource> ExtractPagesWithFormatting(
    double pageWidth, double pageHeight, int fontSize, double lineHeight,
    Parser parser, ObservableCollection<Title> tableOfContents)
        {
            if (!parser.Features.Text)
                throw new NotSupportedException("Документ не поддерживает извлечение текста.");

            var options = new FormattedTextOptions(FormattedTextMode.Html);
            string fullHtml;
            using (var reader = parser.GetFormattedText(options))
            {
                fullHtml = reader.ReadToEnd();
            }
            fullHtml = fullHtml.Replace("<br>", "").Replace("</br>", "");
            
            var pages = new List<HtmlWebViewSource>();
            //tableOfContents.Clear();

            // Используем HtmlAgilityPack для обработки разметки
            var document = new HtmlDocument();
            document.LoadHtml(fullHtml);
            var nodes = document.DocumentNode.SelectNodes("//body//node()")
                       ?? document.DocumentNode.SelectNodes("//*")
                      ?? new HtmlNodeCollection(document.DocumentNode);

            if (nodes == null || nodes.Count == 0)
                throw new InvalidOperationException("Не удалось найти узлы в документе.");

            string currentPageContent="";
            var currentHeight = 0.0;
            Title currentTitle = null;

            foreach (var node in nodes)
            {
                if (IsHeader(node))
                {
                    int headerLevel = GetHeaderLevel(node.Name);

                    // Если заголовок нового типа — завершаем текущую страницу
                    if (currentTitle != null && headerLevel != currentTitle.SubItems.Count + 1)
                    {
                        pages.Add(new HtmlWebViewSource { Html = WrapInHtml(currentPageContent, fontSize) });
                        currentPageContent="";
                        currentHeight = 0;
                    }

                    // Добавляем заголовок в оглавление
                    var newTitle = new Title { Name = node.InnerText.Trim(),PageNumber=pages.Count+1 };
                    if (currentTitle == null || headerLevel == 1)
                    {
                        tableOfContents.Add(newTitle);
                    }
                    else
                    {
                        AddToTitleHierarchy(tableOfContents, newTitle, headerLevel);
                    }

                    currentTitle = newTitle;

                    // Добавляем заголовок на текущую страницу
                    currentPageContent+=node.OuterHtml;
                    currentHeight += CalculateBlockHeight(node.OuterHtml, pageWidth, fontSize, lineHeight);
                }
                else
                {
                    // Обрабатываем текстовые блоки
                    var blockHeight = CalculateBlockHeight(node.OuterHtml, pageWidth, fontSize, lineHeight);

                    if (currentHeight + blockHeight > pageHeight)
                    {
                        // Заканчиваем текущую страницу
                        pages.Add(new HtmlWebViewSource { Html = WrapInHtml(currentPageContent.ToString(), fontSize) });
                        currentPageContent = "";
                        currentHeight = 0;
                    }

                    currentPageContent += node.OuterHtml;
                    currentHeight += blockHeight;
                }
            }

            // Добавляем последнюю страницу
            if (currentPageContent.Length > 0)
            {
                pages.Add(new HtmlWebViewSource { Html = WrapInHtml(currentPageContent.ToString(), fontSize) });
            }

            Debug.WriteLine($"TableOfContents Count: {tableOfContents.Count}");
            foreach (var item in tableOfContents)
            {
                Debug.WriteLine($"Title: {item.Name}");
            }

            return pages;
        }

        private static bool IsHeader(HtmlNode node) =>
    node.Name.StartsWith("h", StringComparison.OrdinalIgnoreCase) &&
    int.TryParse(node.Name.Substring(1), out _);

        private static int GetHeaderLevel(string tagName)
        {
            if (int.TryParse(tagName.Substring(1), out int level))
                return level;

            return int.MaxValue; // Если уровень не определен
        }

        private static void AddToTitleHierarchy(ObservableCollection<Title> titles, Title newTitle, int level)
        {
            Title parent = null;
            var currentLevel = titles;

            for (int i = 1; i < level; i++)
            {
                if (currentLevel.Count == 0)
                    break;

                parent = currentLevel.Last();
                currentLevel = parent.SubItems;
            }

            currentLevel.Add(newTitle);
        }


        private static double CalculateBlockHeight(string block, double pageWidth, int fontSize, double lineHeight)
        {
            int charCount = block.Length;
            double charsPerLine = pageWidth / (fontSize * 0.6);
            double lines = Math.Ceiling(charCount / charsPerLine);
            return lines * fontSize * lineHeight;
        }


        private static string WrapInHtml(string content, int fontSize)
        {
            string settings = $"<style type=\"text/css\"> " +
                $"p {{ margin: 0; text-indent: 1.5em; line-height: 1.5; }} " +
                $"h1, h2, h3, h4, h5, h6 {{ text-align: center; margin: 0; }} " +
                $"body {{ font-family: serif; font-size: {fontSize}px; text-align: justify; }}" +
                $"</style>";

            return $"<html><head>{settings}</head><body>{content}</body></html>";
        }



        private static void ParseHtmlNode(HtmlNode node, FormattedString formattedString, double fontSize)
        {
            // Добавляем содержимое узла в `FormattedString` с учетом стилей
            foreach (var line in node.InnerText.Split('\n'))
            {
                var span = new Span
                {
                    Text = line + "\n",
                    FontSize = fontSize,
                    FontAttributes = node.Name.StartsWith("h") ? FontAttributes.Bold : FontAttributes.None
                };
                formattedString.Spans.Add(span);
            }
        }


    }
}