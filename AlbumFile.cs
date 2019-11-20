using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaLibrary
{
    public class AlbumFile
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // public property
        public string filePath { get; set; }
        public List<Album> Albums { get; set; }

        public AlbumFile(string path)
        {
            Albums = new List<Album>();
            filePath = path;
            // to populate the list with data, read from the data file
            try
            {
                if (File.Exists(path))
                {
                    StreamReader sr = new StreamReader(filePath);
                    while (!sr.EndOfStream)
                    {
                        // create instance of Album class
                        Album album = new Album();
                        string line = sr.ReadLine();
                        // first look for quote(") in string
                        // this indicates a comma(,) in album title
                        int idx = line.IndexOf('"');
                        if (idx == -1)
                        {
                            // no quote = no comma in album title
                            // album details are separated with comma(,)
                            string[] albumDetails = line.Split(',');
                            album.mediaId = UInt64.Parse(albumDetails[0]);
                            album.title = albumDetails[1];
                            album.genres = albumDetails[2].Split('|').ToList();
                            album.artist = albumDetails[3];
                            album.recordLabel = albumDetails[4];
                        }
                        else
                        {
                            // quote = comma or quotes in album title
                            // extract the albumId
                            album.mediaId = UInt64.Parse(line.Substring(0, idx - 1));
                            // remove albumId and first comma from string
                            line = line.Substring(idx);
                            // find the last quote
                            idx = line.LastIndexOf('"');
                            // extract title
                            album.title = line.Substring(0, idx + 1);
                            // remove title and next comma from the string
                            line = line.Substring(idx + 2);
                            // split the remaining string based on commas
                            string[] details = line.Split(',');
                            // the first item in the array should be genres 
                            album.genres = details[0].Split('|').ToList();
                            // the next item in the array should be artist
                            album.artist = details[1];
                            // the next item in the array should be record label
                            album.recordLabel = details[2];
                        }
                        Albums.Add(album);
                    }
                    // close file when done
                    sr.Close();
                    logger.Info("Albums in file {Count}", Albums.Count);
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
            if (Albums.ConvertAll(a => a.title.ToLower()).Contains(title.ToLower()))
            {
                logger.Info("Duplicate album title {Title}", title);
                return false;
            }
            return true;
        }

        public void AddAlbum(Album album)
        {
            try
            {
                // first generate album id
                album.mediaId = Albums.Count == 0 ? 1 : Albums.Max(a => a.mediaId) + 1;
                // if title contains a comma, wrap it in quotes
                string title = album.title.IndexOf(',') != -1 || album.title.IndexOf('"') != -1 ? $"\"{album.title}\"" : album.title;
                StreamWriter sw = new StreamWriter(filePath, true);
                // write album data to file
                sw.WriteLine($"{album.mediaId},{title},{string.Join("|", album.genres)},{album.artist},{album.recordLabel}");
                sw.Close();
                // add album details to List
                Albums.Add(album);
                // log transaction
                logger.Info("Media id {Id} added", album.mediaId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
