using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPQ.Models.Spotify_Model
{
    public class ItemSavedAlbum
    {
        public string added_at { get; set; }
        public Album album { get; set; }
    }
}
