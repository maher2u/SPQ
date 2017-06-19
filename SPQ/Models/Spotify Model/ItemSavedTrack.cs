using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPQ.Models.Spotify_Model
{
    public class ItemSavedTrack
    {
        public string added_at { get; set; }
        public Track track { get; set; }
    }
}
