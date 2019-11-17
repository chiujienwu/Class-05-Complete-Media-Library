using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLibrary
{
    class AlbumFile
    {

        /*Add the MovieFile class – this will be similar(but not identical)
         * to the MovieFile class from the Movie Library application
        that we completed in class last time*/

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // public property
        public string filePath { get; set; }
        public List<Album> Albums { get; set; }

        // constructor is a special method that is invoked
        // when an instance of a class is created
        public AlbumFile(string path)
        {
            Albums = new List<Album>();
            
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
                    Album album = new Album();
                    string line = sr.ReadLine();
                    // first look for quote(") in string
                    // this indicates a comma(,) in movie title
                    int idx = line.IndexOf('"');

                    if (idx == -1)
                    {
                        // no quote = no comma in movie title
                        // movie details are separated with comma(,)
                        // return $"Id: {mediaId}\nTitle: {title}\nArtist: {artist}\nLabel: {recordLabel}\nGenres: {string.Join(", ", genres)}\n";

                        string[] albumDetails = line.Split(',');
                        album.mediaId = UInt64.Parse(albumDetails[0]);
                        album.title = albumDetails[1];
                        album.artist = albumDetails[2];
                        album.recordLabel = albumDetails[3];
                        album.genres = albumDetails[4].Split('|').ToList();

                    }
                    else
                    {
                        // quote = comma or quotes in album title
                        // return $"Id: {mediaId}\nTitle: {title}\nArtist: {artist}\nLabel: {recordLabel}\nGenres: {string.Join(", ", genres)}\n";
                        // extract the mediaId
                        album.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                        // remove mediaID and first comma from string
                        line = line.Substring(idx);
                        // find the last quote
                        idx = line.LastIndexOf('"');
                        // extract title
                        album.title = line.Substring(0, idx + 1);
                        // remove title and next comma from the string
                        line = line.Substring(idx + 2);
                        // split the remaining string based on commas
                        string[] details = line.Split(',');
                        // the first item in the array should be artist
                        album.artist = details[0];
                        // the first item in the array should be genres
                        album.genres = details[1].Split('|').ToList();
                    }
                    Albums.Add(album);
                }
                // close file when done
                sr.Close();
                logger.Info("Albums in file {Count}", Albums.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        // public method
        public bool isUniqueTitle(string title)
        {
            if (Albums.ConvertAll(m => m.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate movie title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddAlbums(Album album)
        {
            try
            {
                // first generate movie id
                album.mediaId = Albums.Max(m => m.mediaId) + 1;
                // if title contains a comma, wrap it in quotes
                string title = album.title.IndexOf(',') != -1 ? $"\"{album.title}\"" : album.title;
                StreamWriter sw = new StreamWriter(filePath, true);

                /*Copied from FileScrubber
                sw.WriteLine($"{movie.mediaId},{movie.title},{genres},{movie.director},{movie.runningTime}");
                used to modify the following line below

                original line:  sw.WriteLine($"{movie.mediaId},{title},{string.Join("|", movie.genres)}");
                 */

                sw.WriteLine($"{album.mediaId},{album.title},{album.artist},{string.Join("|", album.genres)}");


                sw.Close();
                // add movie details to Lists
                Albums.Add(album);
                // log transaction
                logger.Info("Movie id {Id} added", album.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

    }
}
