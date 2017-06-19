using SPQ.Models.Spotify_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPQ.ViewModel
{
    public class PlaylistWithTracksViewModel
    {
        public List<Image> images { get; set; }
        public Followers followers { get; set; }
        public Owner owner { get; set; }
        public List<PlaylistTrackItem> tracks { get; set; }
        public string name { get; set; }
        public bool publicc { get; set; }
        public int tracks_total { get; set; }
    }
}