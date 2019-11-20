using System;
using NLog;

namespace MediaLibrary
{
    class MainClass
    {
        // create a class level instance of logger (can be used in methods other than Main)
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            logger.Info("Program started");

            string scrubbedFile = FileScrubber.ScrubMovies("../../movies.csv");
            string albumFileName = "../../albums.csv";
            string bookFileName = "../../books.csv";
            MovieFile movieFile = new MovieFile(scrubbedFile);
            AlbumFile albumFile = new AlbumFile(albumFileName);
            BookFile bookFile = new BookFile(bookFileName);

            string choice = "";
            do
            {
                // display choices to user
                Console.WriteLine("1) Add Movie");
                Console.WriteLine("2) Display All Movies");
                Console.WriteLine("3) Add Album");
                Console.WriteLine("4) Display All Albums");
                Console.WriteLine("5) Add Book");
                Console.WriteLine("6) Display All Books");
                Console.WriteLine("Enter to quit");
                // input selection
                choice = Console.ReadLine();
                logger.Info("User choice: {Choice}", choice);
                if (choice == "1")
                {
                    // Add movie
                    Movie movie = new Movie();
                    // ask user to input movie title
                    Console.WriteLine("Enter movie title");
                    // input title
                    movie.title = Console.ReadLine();
                    // verify title is unique
                    if (movieFile.isUniqueTitle(movie.title))
                    {
                        // input genres
                        string input;
                        do
                        {
                            // ask user to enter genre
                            Console.WriteLine("Enter genre (or done to quit)");
                            // input genre
                            input = Console.ReadLine();
                            // if user enters "done"
                            // or does not enter a genre do not add it to list
                            if (input != "done" && input.Length > 0)
                            {
                                movie.genres.Add(input);
                            }
                        } while (input != "done");
                        // specify if no genres are entered
                        if (movie.genres.Count == 0)
                        {
                            movie.genres.Add("(no genres listed)");
                        }
                        // ask user to enter director
                        Console.WriteLine("Enter movie director");
                        input = Console.ReadLine();
                        movie.director = input.Length == 0 ? "unassigned" : input;
                        // ask user to enter running time
                        Console.WriteLine("Enter running time (h:m:s)");
                        input = Console.ReadLine();
                        movie.runningTime = input.Length == 0 ? new TimeSpan(0) : TimeSpan.Parse(input);
                        // add movie
                        movieFile.AddMovie(movie);
                    }
                    else
                    {
                        Console.WriteLine("Movie title already exists\n");
                    }

                }
                else if (choice == "2")
                {
                    // Display All Movies
                    foreach (Movie m in movieFile.Movies)
                    {
                        Console.WriteLine(m.Display());
                    }
                }
                else if (choice == "3")
                {
                    // Add Album
                    Album album = new Album();
                    // ask user to input album title
                    Console.WriteLine("Enter album title");
                    // input title
                    album.title = Console.ReadLine();
                    // verify title is unique
                    if (albumFile.isUniqueTitle(album.title))
                    {
                        // input genres
                        string input;
                        do
                        {
                            // ask user to enter genre
                            Console.WriteLine("Enter genre (or done to quit)");
                            // input genre
                            input = Console.ReadLine();
                            // if user enters "done"
                            // or does not enter a genre do not add it to list
                            if (input != "done" && input.Length > 0)
                            {
                                album.genres.Add(input);
                            }
                        } while (input != "done");
                        // specify if no genres are entered
                        if (album.genres.Count == 0)
                        {
                            album.genres.Add("(no genres listed)");
                        }
                        // ask user to enter director
                        Console.WriteLine("Enter album artist");
                        input = Console.ReadLine();
                        album.artist = input.Length == 0 ? "unassigned" : input;
                        // ask user to enter record label
                        Console.WriteLine("Enter record label");
                        input = Console.ReadLine();
                        album.recordLabel = input.Length == 0 ? "unassigned" : input;
                        // add album
                        albumFile.AddAlbum(album);
                    }
                    else
                    {
                        Console.WriteLine("Album title already exists\n");
                    }
                }
                else if (choice == "4")
                {
                    // Display All Albums
                    foreach (Album a in albumFile.Albums)
                    {
                        Console.WriteLine(a.Display());
                    }
                }
                else if (choice == "5")
                {
                    // Add Book
                    Book book = new Book();
                    // ask user to input book title
                    Console.WriteLine("Enter book title");
                    // input title
                    book.title = Console.ReadLine();
                    // verify title is unique
                    if (bookFile.isUniqueTitle(book.title))
                    {
                        // input genres
                        string input;
                        do
                        {
                            // ask user to enter genre
                            Console.WriteLine("Enter genre (or done to quit)");
                            // input genre
                            input = Console.ReadLine();
                            // if user enters "done"
                            // or does not enter a genre do not add it to list
                            if (input != "done" && input.Length > 0)
                            {
                                book.genres.Add(input);
                            }
                        } while (input != "done");
                        // specify if no genres are entered
                        if (book.genres.Count == 0)
                        {
                            book.genres.Add("(no genres listed)");
                        }
                        // ask user to enter author
                        Console.WriteLine("Enter book author");
                        input = Console.ReadLine();
                        book.author = input.Length == 0 ? "unassigned" : input;
                        // ask user to enter publisher
                        Console.WriteLine("Enter publisher");
                        input = Console.ReadLine();
                        book.publisher = input.Length == 0 ? "unassigned" : input;
                        // ask user to enter number of pages
                        Console.WriteLine("Enter number of pages");
                        input = Console.ReadLine();
                        book.pageCount = input.Length == 0 ? (UInt16)0 : UInt16.Parse(input);
                        // add book
                        bookFile.AddBook(book);
                    }
                    else
                    {
                        Console.WriteLine("Book title already exists\n");
                    }
                }
                else if (choice == "6")
                {
                    // Display All Books
                    foreach (Book b in bookFile.Books)
                    {
                        Console.WriteLine(b.Display());
                    }
                }
            } while (choice == "1" || choice == "2" || choice == "3" || choice == "4" || choice == "5" || choice == "6");

            logger.Info("Program ended");
        }
    }
}
