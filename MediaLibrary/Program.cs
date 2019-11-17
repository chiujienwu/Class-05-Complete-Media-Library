using System;
using NLog;

namespace MediaLibrary
{
    class MainClass
    {
        // create a class level instance of logger (can be used in methods other than Main)
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)

        /*        {
                    logger.Info("Program started");

                    string scrubbedFile = FileScrubber.ScrubMovies("../../movies.csv");

                    logger.Info("Program ended");
                }*/

        // add new starter lines

        {
            logger.Info("Program started");

            Movie movie = new Movie
            {
                mediaId = 123,
                title = "Greatest Movie Ever, The (2019)",
                director = "Jeff Grissom",
                // timespan (hours, minutes, seconds)
                runningTime = new TimeSpan(2, 21, 23),
                genres = { "Comedy", "Romance" }
            };

            Console.WriteLine(movie.Display());

            logger.Info("Program ended");
        }
    }
}

