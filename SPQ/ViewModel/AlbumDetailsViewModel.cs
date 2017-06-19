using SPQ.Models.Spotify_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPQ.ViewModel
{
    public class AlbumDetailsViewModel
    {
        public string AlbumName { get; set; }
        public string ArtistName { get; set; }
        public string ImageUrl { get; set; }
        public List<Copyright> Copyrights { get; set; }
        public int TracksCount { get; set; }
        public string ExtendedDuration { get; set; }
        public string ReleaseDate { get; set; }
        public List<Track> Tracks { get; set; }
    }
}