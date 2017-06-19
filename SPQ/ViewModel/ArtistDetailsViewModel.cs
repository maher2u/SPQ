using SPQ.Models.Spotify_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPQ.ViewModel
{
    public class ArtistDetailsViewModel
    {
        public ItemArtist Artist { get; set; }
        public List<Album> ArtistAlbums { get; set; }
        public List<Album> ArtistSingles { get; set; }
        public List<Album> ArtistAppearsOn { get; set; }
        public List<Album> ArtistCompilation { get; set; }
        public List<Track> ArtistTopTracks { get; set; }
        public List<ItemArtist> RelatedArtists { get; set; }
    }
}