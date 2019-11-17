using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary
{
    class MovieFile
    {
        /*Add the MovieFile class – this will be similar(but not identical)
         * to the MovieFile class from the Movie Library application
        that we completed in class last time*/

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // public property
        public string filePath { get; set; }
        public List<Movie> Movies { get; set; }

        // constructor is a special method that is invoked
        // when an instance of a class is created
        public MovieFile(string path)
        {
            Movies = new List<Movie>();
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
                    Movie movie = new Movie();
                    string line = sr.ReadLine();
                    // first look for quote(") in string
                    // this indicates a comma(,) in movie title
                    int idx = line.IndexOf('"');

                    if (idx == -1)
                    {
                        // no quote = no comma in movie title
                        // movie details are separated with comma(,)

                        string[] movieDetails = line.Split(',');
                        movie.mediaId = UInt64.Parse(movieDetails[0]);
                        movie.title = movieDetails[1];
                        movie.genres = movieDetails[2].Split('|').ToList();
                        movie.director = movieDetails.Length > 3 ? movieDetails[3] : "unassigned";
                        movie.runningTime = movieDetails.Length > 4 ? TimeSpan.Parse(movieDetails[4]) : new TimeSpan(0);
                    }
                    else
                    {
                        // quote = comma or quotes in movie title
                        // extract the mediaId
                        movie.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                        // remove mediaID and first comma from string
                        line = line.Substring(idx);
                        // find the last quote
                        idx = line.LastIndexOf('"');
                        // extract title
                        movie.title = line.Substring(0, idx + 1);
                        // remove title and next comma from the string
                        line = line.Substring(idx + 2);
                        // split the remaining string based on commas
                        string[] details = line.Split(',');
                        // the first item in the array should be genres 
                        movie.genres = details[0].Split('|').ToList();
                        // if there is another item in the array it should be director
                        movie.director = details.Length > 1 ? details[1] : "unassigned";
                        // if there is another item in the array it should be run time
                        movie.runningTime = details.Length > 2 ? TimeSpan.Parse(details[2]) : new TimeSpan(0);
                        
                    }
                    Movies.Add(movie);
                }
                // close file when done
                sr.Close();
                logger.Info("Movies in file {Count}", Movies.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        // public method
        public bool isUniqueTitle(string title)
        {
            if (Movies.ConvertAll(m => m.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate movie title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddMovie(Movie movie)
        {
            try
            {
                // first generate movie id
                movie.mediaId = Movies.Max(m => m.mediaId) + 1;
                // if title contains a comma, wrap it in quotes
                string title = movie.title.IndexOf(',') != -1 ? $"\"{movie.title}\"" : movie.title;
                StreamWriter sw = new StreamWriter(filePath, true);

                /*Copied from FileScrubber
                sw.WriteLine($"{movie.mediaId},{movie.title},{genres},{movie.director},{movie.runningTime}");
                used to modify the following line below

                original line:  sw.WriteLine($"{movie.mediaId},{title},{string.Join("|", movie.genres)}");
                 */

                sw.WriteLine($"{movie.mediaId},{movie.title},{string.Join("|", movie.genres)},{movie.director},{movie.runningTime}");
              

                sw.Close();
                // add movie details to Lists
                Movies.Add(movie);
                // log transaction
                logger.Info("Movie id {Id} added", movie.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

    }
}
