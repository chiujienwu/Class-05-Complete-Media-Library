using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary
{
    class BookFile
    { 
        /*Add the MovieFile class – this will be similar(but not identical)
         * to the MovieFile class from the Movie Library application
        that we completed in class last time*/

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // public property
        public string filePath { get; set; }
        public List<Book> Books { get; set; }

        // constructor is a special method that is invoked
        // when an instance of a class is created
        public BookFile(string path)
        {
            Books = new List<Book>();

            filePath = path;
            // to populate the list with data, read from the data file
            try
            {
                StreamReader sr = new StreamReader(filePath);
                // first line contains column headers
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    // create instance of Movie class
                    Book book = new Book();
                    string line = sr.ReadLine();
                    // first look for quote(") in string
                    // this indicates a comma(,) in movie title
                    int idx = line.IndexOf('"');

                    if (idx == -1)
                    {
                        // no quote = no comma in movie title
                        // movie details are separated with comma(,)
                        // return $"Id: {mediaId}\nTitle: {title}\nArtist: {artist}\nLabel: {recordLabel}\nGenres: {string.Join(", ", genres)}\n";
                        //  return $"Id: {mediaId}\nTitle: {title}\nAuthor: {author}\nPages: {pageCount}\nPublisher: {publisher}\nGenres: {string.Join(", ", genres)}\n";
                    
                    string[] bookDetails = line.Split(',');
                        book.mediaId = UInt64.Parse(bookDetails[0]);
                        book.title = bookDetails[1];
                        book.author = bookDetails[2];
                        book.pageCount = UInt16.Parse(bookDetails[3]);
                        book.publisher = bookDetails[4];
                        book.genres = bookDetails[5].Split('|').ToList();

                    }
                    else
                    {
                        // quote = comma or quotes in album title
                        // return $"Id: {mediaId}\nTitle: {title}\nArtist: {artist}\nLabel: {recordLabel}\nGenres: {string.Join(", ", genres)}\n";
                        // extract the mediaId
                        book.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                        // remove mediaID and first comma from string
                        line = line.Substring(idx);
                        // find the last quote
                        idx = line.LastIndexOf('"');
                        // extract title
                        book.title = line.Substring(0, idx + 1);
                        // remove title and next comma from the string
                        line = line.Substring(idx + 2);
                        // split the remaining string based on commas
                        string[] details = line.Split(',');
                        // the first item in the array should be artist
                        book.author = details[0];
                        // the first item in the array should be genres
                        book.genres = details[1].Split('|').ToList();
                    }
                    Books.Add(book);
                }
                // close file when done
                sr.Close();
                logger.Info("Books in file {Count}", Books.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        // public method
        public bool isUniqueTitle(string title)
        {
            if (Books.ConvertAll(m => m.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate book title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddBooks(Book book)
        {
            try
            {
                // first generate movie id
                book.mediaId = Books.Max(m => m.mediaId) + 1;
                // if title contains a comma, wrap it in quotes
                string title = book.title.IndexOf(',') != -1 ? $"\"{book.title}\"" : book.title;
                StreamWriter sw = new StreamWriter(filePath, true);

                /*Copied from FileScrubber
                sw.WriteLine($"{movie.mediaId},{movie.title},{genres},{movie.director},{movie.runningTime}");
                used to modify the following line below

                original line:  sw.WriteLine($"{movie.mediaId},{title},{string.Join("|", movie.genres)}");

                return $"Id: {mediaId}\nTitle: {title}\nAuthor: {author}\nPages: {pageCount}\nPublisher: {publisher}\nGenres: {string.Join(", ", genres)}\n";
                 */

                sw.WriteLine($"{book.mediaId},{book.title},{book.author},{book.pageCount},{book.publisher},{string.Join("|", book.genres)}");


                sw.Close();
                // add movie details to Lists
                Books.Add(book);
                // log transaction
                logger.Info("Book id {Id} added", book.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

    }
}
