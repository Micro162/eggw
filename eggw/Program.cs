using System;
using System.IO;
using System.Threading;

namespace WordSearchApp
{
    // Клас для передачі аргументів у потік
    class SearchParams
    {
        public string FilePath { get; set; }
        public string SearchWord { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string filePath;
            string searchWord;

            if (args.Length >= 2)
            {
                filePath = args[0];
                searchWord = args[1];
            }
            else
            {
                Console.Write("Введіть шлях до файлу: ");
                filePath = Console.ReadLine();
                Console.Write("Введіть слово для пошуку: ");
                searchWord = Console.ReadLine();
            }

            var searchParams = new SearchParams
            {
                FilePath = filePath,
                SearchWord = searchWord
            };

            Thread childThread = new Thread(SearchWordInFile);
            childThread.Start(searchParams);

            childThread.Join();

            Console.WriteLine("\nГоловний потік завершив роботу.");
            Console.ReadKey();
        }

        static void SearchWordInFile(object obj)
        {
            SearchParams p = (SearchParams)obj;

            Console.WriteLine($"[Дочірній потік {Thread.CurrentThread.ManagedThreadId}] " +
                               $"Пошук слова '{p.SearchWord}' у файлі '{p.FilePath}'...");

            try
            {
                if (!File.Exists(p.FilePath))
                {
                    Console.WriteLine("Помилка: файл не знайдено.");
                    return;
                }

                string text = File.ReadAllText(p.FilePath);

                string[] words = text.Split(new char[] { ' ', '\t', '\n', '\r', '.', ',', ';', '!', '?', ':', '"', '\'' },
                                             StringSplitOptions.RemoveEmptyEntries);

                int count = 0;
                foreach (string w in words)
                {
                    if (string.Equals(w, p.SearchWord, StringComparison.OrdinalIgnoreCase))
                        count++;
                }

                Console.WriteLine($"Слово '{p.SearchWord}' зустрічається у файлі {count} раз(ів).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при читанні файлу: {ex.Message}");
            }
        }
    }
}