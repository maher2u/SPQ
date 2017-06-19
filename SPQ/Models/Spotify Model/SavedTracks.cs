using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPQ.Models.Spotify_Model
{
    public class SavedTracks
    {
        public string href { get; set; }
        public List<ItemSavedTrack> items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }
}
