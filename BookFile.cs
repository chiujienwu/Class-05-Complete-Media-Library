using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaLibrary
{
    public class BookFile
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // public property
        public string filePath { get; set; }
        public List<Book> Books { get; set; }

        public BookFile(string path)
        {
            Books = new List<Book>();
            filePath = path;
            // to populate the list with data, read from the data file
            try
            {
                if (File.Exists(path))
                {
                    StreamReader sr = new StreamReader(filePath);
                    while (!sr.EndOfStream)
                    {
                        // create instance of Book class
                        Book book = new Book();
                        string line = sr.ReadLine();
                        // first look for quote(") in string
                        // this indicates a comma(,) in book title
                        int idx = line.IndexOf('"');
                        if (idx == -1)
                        {
                            // no quote = no comma in album title
                            // book details are separated with comma(,)
                            string[] bookDetails = line.Split(',');
                            book.mediaId = UInt64.Parse(bookDetails[0]);
                            book.title = bookDetails[1];
                            book.genres = bookDetails[2].Split('|').ToList();
                            book.author = bookDetails[3];
                            book.publisher = bookDetails[4];
                            book.pageCount = UInt16.Parse(bookDetails[5]);
                        }
                        else
                        {
                            // quote = comma or quotes in book title
                            // extract the bookId
                            book.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                            // remove bookId and first comma from string
                            line = line.Substring(idx);
                            // find the last quote
                            idx = line.LastIndexOf('"');
                            // extract title
                            book.title = line.Substring(0, idx + 1);
                            // remove title and next comma from the string
                            line = line.Substring(idx + 2);
                            // split the remaining string based on commas
                            string[] details = line.Split(',');
                            // the first item in the array should be genres 
                            book.genres = details[0].Split('|').ToList();
                            // the next item in the array should be author
                            book.author = details[1];
                            // the next item in the array should be publisher
                            book.publisher = details[2];
                            // the next item in the array should be page count
                            book.pageCount = UInt16.Parse(details[3]);
                        }
                        Books.Add(book);
                    }
                    // close file when done
                    sr.Close();
                    logger.Info("Books in file {Count}", Books.Count);
                }
                else
                {
                    logger.Info("The file does not exist {Path}", path);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        // public method
        public bool isUniqueTitle(string title)
        {
            if (Books.ConvertAll(b => b.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate book title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddBook(Book book)
        {
            try
            {
                // first generate book id
                book.mediaId = Books.Count == 0 ? 1 : Books.Max(b => b.mediaId) + 1;
                // if title contains a comma, wrap it in quotes
                string title = book.title.IndexOf(',') != -1 || book.title.IndexOf('"') != -1 ? $"\"{book.title}\"" : book.title;
                StreamWriter sw = new StreamWriter(filePath, true);
                // write book data to file
                sw.WriteLine($"{book.mediaId},{title},{string.Join("|", book.genres)},{book.author},{book.publisher},{book.pageCount}");
                sw.Close();
                // add book details to List
                Books.Add(book);
                // log transaction
                logger.Info("Media id {Id} added", book.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
