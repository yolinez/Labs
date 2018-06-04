using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LibrarySystem
{
    internal class BookInfo
    {
        public string Name;
        public string Author;
        public string Year;
        public string Path;

        public void SetName(string NameToChange)
        {
            Name = NameToChange;
        }
    }

    class MainClass
    {
        private static List<string> Pathes = new List<string>(); //Доступ к закрытым членам можно получить только внутри тела класса или структуры, в которой они объявлены
        private static List<BookInfo> InfoKatalog = new List<BookInfo>();//создаем динамический лист, который хранит экземпляры класса BookInfo
        private static List<BookInfo> RecentlyOpenBooks = new List<BookInfo>();// static можно использовать не изменяя класса данного

        private static int GetNameCount(string BookInfo)
        {
            int NameCount = 0;
            foreach (char ch in BookInfo)
            {
                if (ch != ' ') // если символ есть
                {
                    NameCount++;
                }
                else
                {
                    break;
                }
            }
            return NameCount;
        }

        private static int GetAuthorCount(string BookInfo, int NameCount)
        {
            int AuthorCount = 0;
            string BookInfoWoutName = BookInfo.Substring(NameCount + 1, BookInfo.Length - 1 - NameCount);// получаю строки из подстроки. 
            foreach (char ch in BookInfoWoutName)//первый элем массива выбирается и присваивается переменной на каждом последующем шаге выбирается сл.элем, который сохраняется в переменной
            {
                if (ch != ' ') //если не пусто,то добавляем
                {
                    AuthorCount++;
                }
                else
                {
                    break;
                }
            }
            return AuthorCount;
        }

        private static int GetYearCount(string BookInfo, int NameAndAuthorCount)
        {
            int YearCount = 0;
            string BookInfoWithoutNameAndAuthor = BookInfo.Substring(NameAndAuthorCount + 2, BookInfo.Length - 2 - NameAndAuthorCount - 4);
            foreach (char ch in BookInfoWithoutNameAndAuthor)
            {
                if (ch != ' ')
                {
                    YearCount++;
                }
                else
                {
                    break;
                }
            }
            return YearCount;
        }

        private static void ReLoadBooks()
        {
            Console.Clear();
            InfoKatalog.Clear();
            for (int i = 0; i <= Pathes.Count - 1; i++)
            {
                string dirName = Pathes[i];
                if (Directory.Exists(dirName))
                {
                    var Direct = new DirectoryInfo(dirName);//Выполняет инициализацию нового экземпляра класса DirectoryInfo для заданного пути.
                    FileInfo[] dirs = Direct.GetFiles();//Возвращает имена файлов (с указанием пути к ним) в указанном каталоге.Предоставляет свойства и методы экземпляра для создания, копирования, удаления, перемещения и открытия файлов
                    foreach (var dir in dirs)
                    {
                        var NewBook = new BookInfo();//С помощью оператора new можно вызвать заданный по умолчанию конструктор для неявного типа var(может быть использован только для объявления и инициализации)
                        int NameCount = GetNameCount(dir.Name);
                        int AuthorCount = GetAuthorCount(dir.Name, NameCount);
                        int YearCount = GetYearCount(dir.Name, NameCount + AuthorCount);
                        NewBook.Name = dir.Name.Substring(0, NameCount); //Извлекает подстроку из данного экземпляра.Подстрока начинается с указанной позиции знака и имеет указанную длину.
                        NewBook.Author = dir.Name.Substring(GetNameCount(dir.Name) + 1,
                            GetAuthorCount(dir.Name, GetNameCount(dir.Name)) + 1);
                        NewBook.Year = dir.Name.Substring(AuthorCount + NameCount + 2, YearCount);
                        NewBook.Path = dir.FullName;

                        InfoKatalog.Add(NewBook);
                    }
                }
            }
        }


        private static void printBookList(List<BookInfo> InfoKatalog)// Вывод книг на экран + путь к ним
        {
            Console.Clear();
            int i = 0;
            foreach (var item in InfoKatalog)
            {
                Console.WriteLine(i + " " + item.Name + " " + item.Author + " " + item.Year + " " + item.Path + "\n");
                i++;
            }
            Console.ReadKey();
        }

        private static List<BookInfo> OrderByName(List<BookInfo> InfoKatalog)//сортировка по названию
        {
            var sortedList = from book in InfoKatalog
                             orderby book.Name //Сортирует элементы последовательности в возрастающем порядке
                             select book; //В выражении запроса предложение select задает тип значений, которые будут получены при выполнении запроса.
            //Этот метод реализуется с использованием отложенного выполнения. Немедленно возвращаемое значение — это объект, который хранит все сведения, необходимые для выполнения действия.
            return sortedList.ToList();
        }

        private static List<BookInfo> OrderByAuthor(List<BookInfo> InfoKatalog)// сортировка по автору
        {
            var sortedList = from book in InfoKatalog
                             orderby book.Author//Сортирует элементы последовательности в возрастающем порядке
                             select book;//В выражении запроса предложение select задает тип значений, которые будут получены при выполнении запроса
            return sortedList.ToList();
        }

        private static List<BookInfo> OrderByData(List<BookInfo> InfoKatalog)//сортировка по дате
        {
            var sortedList = from book in InfoKatalog
                             orderby book.Year//Сортирует элементы последовательности в возрастающем порядке
                             select book;//В выражении запроса предложение select задает тип значений, которые будут получены при выполнении запроса
            return sortedList.ToList();
        }

        private static void InformationAboutBook() // в списке книг информация о книге
        {
            Console.Clear();
            Console.WriteLine("Введите название книги");
            string bookName = Console.ReadLine();
            int slideMenu = 0;
            while (slideMenu != 3)
            {
                Console.WriteLine("1)Читать\n2)Удалить издание\n3)Выход");
                slideMenu = Convert.ToInt32(Console.ReadLine());// //Преобразует заданное строковое представление числа в эквивалентное 32-битовое целое число со знаком
                switch (slideMenu)
                {
                    case 1:
                        Read(bookName);
                        break;
                    case 2:
                        DeleteBookByName(bookName);
                        slideMenu = 3;
                        break;
                }
            }
        }

        private static void OrderBooksFunction() //в списке книг доп. функции
        {
            int slideMenu = 0;



            while (slideMenu != 5)
            {
                Console.Clear();
                printBookList(InfoKatalog);
                Console.WriteLine("1)Отсортировать по названию\n2)Отсортировать по автору\n3)Отсортировать по дате\n4)Просмотр информации об издании\n5)Выход");
                slideMenu = Convert.ToInt32(Console.ReadLine());
                switch (slideMenu)
                {
                    case 1:
                        InfoKatalog = OrderByName(InfoKatalog);
                        printBookList(InfoKatalog);
                        slideMenu = 0;
                        break;
                    case 2:
                        InfoKatalog = OrderByAuthor(InfoKatalog);
                        printBookList(InfoKatalog);
                        slideMenu = 0;
                        break;
                    case 3:
                        InfoKatalog = OrderByData(InfoKatalog);
                        printBookList(InfoKatalog);
                        slideMenu = 0;
                        break;
                    case 4:
                        InformationAboutBook();
                        break;
                }
            }
            Console.Clear();
        }

        private static void AddNewBook()//Добавление новой книги
        {
            Console.Clear();
            string path = Console.ReadLine();
            FileInfo dir = new FileInfo(path);

            var NewBook = new BookInfo();
            int NameCount = GetNameCount(dir.Name);
            int AuthorCount = GetAuthorCount(dir.Name, NameCount);
            int YearCount = GetYearCount(dir.Name, NameCount + AuthorCount);
            NewBook.Name = dir.Name.Substring(0, NameCount);
            NewBook.Author = dir.Name.Substring(GetNameCount(dir.Name) + 1,
                GetAuthorCount(dir.Name, GetNameCount(dir.Name)) + 1);
            NewBook.Year = dir.Name.Substring(AuthorCount + NameCount + 2, YearCount);
            NewBook.Path = dir.FullName;

            InfoKatalog.Add(NewBook);

            Console.WriteLine("Книга успешно добавлена");
            Console.ReadKey();
        }

        private static BookInfo FindBookByName(string Name)//поиск по названию
        {
            BookInfo FindedBook = InfoKatalog[0];
            foreach (var Book in InfoKatalog)
            {
                if (Book.Name == Name)
                {
                    FindedBook = Book;
                    break;
                }
            }

            return FindedBook;
        }

        private static void Read(string Name)// доп. функция чтения
        {
            Console.Clear();
            BookInfo FindBookByNameAnswer;

            FindBookByNameAnswer = FindBookByName(Name);
            RecentlyOpenBooks.Add(FindBookByNameAnswer);
            Process.Start(FindBookByNameAnswer.Path);
        }

        private static void DeleteBookByName(string Name)//доп.функция удаления
        {
            Console.Clear();
            BookInfo FindedBook;


            foreach (var Book in InfoKatalog)
            {
                if (Book.Name == Name)
                {
                    FindedBook = Book;
                    InfoKatalog.Remove(Book);
                    return;
                }
            }

            Console.WriteLine("Такой книги нет в каталоге");
        }

        private static void ChangeBookNameByName() //переименовать книгу
        {
            Console.Clear();
            BookInfo FindedBook;

            Console.WriteLine("Введите название книги");

            string Name = Console.ReadLine();

            Console.WriteLine("Введите новое название");

            string NameToChange = Console.ReadLine();

            foreach (var Book in InfoKatalog)
            {
                if (Book.Name == Name)
                {
                    Book.Name = NameToChange;
                    return;
                }
            }

            Console.WriteLine("Такой книги нет в каталоге");
        }

        private static void FindBookBySubStringName()//поиск по части названия
        {
            Console.Clear();
            Console.WriteLine("Введите часть названия издания");
            string SubString = Console.ReadLine();
            Console.WriteLine("Найденные книги");
            foreach (var Book in InfoKatalog)
            {
                if (Book.Name.Contains(SubString))
                {
                    Console.WriteLine(Book.Name + " " + Book.Author + " " + Book.Year + " " + Book.Path + "\n");
                }
            }

            Console.ReadKey();
        }

        private static void FindBookBySubStringAuthor()//поиск по части имени автора
        {
            Console.Clear();
            Console.WriteLine("Введите часть имени автора");
            string SubString = Console.ReadLine();
            Console.WriteLine("Найденные книги");
            foreach (var Book in InfoKatalog)
            {
                if (Book.Author.Contains(SubString))
                {
                    Console.WriteLine(Book.Name + " " + Book.Author + " " + Book.Year + " " + Book.Path + "\n");
                }
            }

            Console.ReadKey();
        }

        private static void FindBookBySubStringYear()// поиск по части года
        {
            Console.Clear();
            Console.WriteLine("Введите часть года");
            string SubString = Console.ReadLine();
            Console.WriteLine("Найденные книги");
            foreach (var Book in InfoKatalog)
            {
                if (Book.Year.Contains(SubString))
                {
                    Console.WriteLine(Book.Name + " " + Book.Author + " " + Book.Year + " " + Book.Path + "\n");
                }
            }

            Console.ReadKey();
        }

        private static void FindFunction() //поиск издания
        {
            int SlideMenu = 0;
            while (SlideMenu != 4)
            {
                Console.WriteLine("1)Поиск по названию 2)Поиск по автору 3)Поиск по году 4)Выход");
                SlideMenu = Convert.ToInt32(Console.ReadLine());
                switch (SlideMenu)
                {
                    case 1:
                        FindBookBySubStringName();
                        break;
                    case 2:
                        FindBookBySubStringAuthor();
                        break;
                    case 3:
                        FindBookBySubStringYear();
                        break;
                }
            }
        }

        public static void Main()
        {
            int Menu = 0;

            Pathes.Add("D:\\Books");

            ReLoadBooks();

            while (Menu != 7)
            {
                Console.Clear();
                Console.WriteLine("Меню:\n1)Вывести книги на экран\n2)Сброс\n3)Добавить книгу\n4)Изменить имя\n5)Недавно открытые издания\n6)Поиск издания\n7)Выход");
                Menu = Convert.ToInt32(Console.ReadLine());
                switch (Menu)
                {
                    case 1:
                        OrderBooksFunction();
                        break;
                    case 2:
                        ReLoadBooks();
                        break;
                    case 3:
                        AddNewBook();
                        break;
                    case 4:
                        ChangeBookNameByName();
                        break;
                    case 5:
                        printBookList(RecentlyOpenBooks);
                        Console.ReadKey();
                        break;
                    case 6:
                        FindFunction();
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}


