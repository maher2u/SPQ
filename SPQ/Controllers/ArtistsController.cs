using SPQ.Models.Spotify_Model;
using SPQ.Models.Spotify_Model.Spotify_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPQ.Controllers
{
    public class ArtistsController : Controller
    {
        SpotifyService spotifyService = new SpotifyService();

        // GET: Artists
        public ActionResult Index()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            var artists = GetAllArtists(token);

            HttpContext.Session.Remove("artists");
            Session["artists"] = artists;

            return View();
        }


        public ActionResult GetData(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var artists = (List<ItemArtist>)Session["artists"];

            var retorno = (from c in artists select c)
                          .Skip(page.Value * pageSize.Value)
                          .Take(pageSize.Value);

            return Json(retorno.ToList(), JsonRequestBehavior.AllowGet);
        }

        // Returns all artists in playlists

        private List<ItemArtist> GetAllArtists(string token)
        {
            if (token == null)
                return null;

            List<ItemArtist> artists = new List<ItemArtist>();
            List<ItemArtist> artistsAux = new List<ItemArtist>();


            List<string> ids_artists = new List<string>();

            UserProfiles spotifyUser = spotifyService.GetCurrentUserProfile(token);
            List<Playlist> userPlaylists = spotifyService.GetUserPlaylists(token, spotifyUser.id, 50, 0);
            List<PlaylistTrackItem> tracks = new List<PlaylistTrackItem>();

            // For each user playlist, get the songs
            foreach (var playlist in userPlaylists)
            {
                var tracksFromPlaylist = GetTracksFromPlaylist(token, playlist, spotifyUser);

                // For each playlist music, store the artist id in the list
                foreach (var track in tracksFromPlaylist)
                {
                    var id = track.track.artists[0].id;
                    ids_artists.Add(id);
                }
            }

            //Get the artists in a row

            var followedArtists = GetFollowedArtists(token);

            foreach (var artistFollowed in followedArtists.items)
            {
                var id = artistFollowed.id;
                ids_artists.Add(id);
            }

            //  different ids
            ids_artists = ids_artists.Distinct().ToList();

            int qtdeIds = ids_artists.Count;
            int maximumIds = 50;
            int qtdePartes = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(qtdeIds) / Convert.ToDouble(maximumIds)));

            var idsSeparadosEmPartes = SpotifyHelpers.SplitList<string>(ids_artists, qtdePartes);

            IEnumerable<string>[] partesDosIds = new IEnumerable<string>[qtdePartes];

            //Get artists from playlists

            for (int i = 0; i < idsSeparadosEmPartes.Count(); i++)
            {
                partesDosIds[i] = idsSeparadosEmPartes.ElementAt(i);
                artistsAux = spotifyService.GetSeveralArtists(token, partesDosIds[i].ToList());
                artistsAux.ForEach(c => artists.Add(c));
            }

            return artists;
        }

        // Returns the artists followed by the user

        private Artists GetFollowedArtists(string token)
        {
            var followedArtists = spotifyService.GetFollowedArtists(token);

            return followedArtists.artists;
        }

        // Returns the songs of all user playlists

        private List<PlaylistTrackItem> GetTracksFromPlaylist(string token, Playlist playlist, UserProfiles spotifyUser)
        {
            // Adding the First Query in the List 

            List<PlaylistTrackItem> listTracks = new List<PlaylistTrackItem>();

            int totalTracks = playlist.tracks.total;
            int limit = 50;
            int offset = 0;

            var tracksFromPlaylist = spotifyService.GetPlaylistTracks(token, spotifyUser.id, playlist.id, limit, offset);

            foreach (var track in tracksFromPlaylist)
            {
                listTracks.Add(track);
            }

            int index = offset + 1;
            offset = limit * index;
            

            if (tracksFromPlaylist.Count > 0)
            {
                // Other queries to the method 

                for (int i = 0; i < totalTracks; i++)
                {
                    if (!(offset > totalTracks))
                    {
                        var tracksFromPlaylistOnLoop = spotifyService.GetPlaylistTracks(token, spotifyUser.id, playlist.id, limit, offset);

                        foreach (var track in tracksFromPlaylistOnLoop)
                        {
                            listTracks.Add(track);
                        }

                        index = index + 1;
                        offset = limit * index;
                    }
                }
                                                              
            }

            return listTracks;
        }



        //Artist Details

        public ActionResult ArtistDetails(string artistId)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            Session["artistId"] = artistId;

            return View();
        }

        public ActionResult GetArtist()
        {
            if (Session["token"] == null)
                return null;

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            var artist = spotifyService.GetArtist(token, artistId);

            return Json(artist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPopularTracks()
        {
            if (Session["token"] == null)
                return null;

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            var popularTracks = spotifyService.GetArtistTopTracks(token, artistId);

            return Json(popularTracks, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRelatedArtists()
        {
            if (Session["token"] == null)
                return null;

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            var relatedArtists = spotifyService.GetRelatedArtists(token, artistId);

            return Json(relatedArtists, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArtistAlbums(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var albums = spotifyService.GetArtistAlbums(token, artistId, limit, offset);

            if (!(albums.items.Count > 0))
                return Json(new { Message = "Fim" }, JsonRequestBehavior.AllowGet);

            var ids_albums = new List<string>();
            albums.items.ForEach(c => ids_albums.Add(c.id));

            // different ids

            ids_albums = ids_albums.Distinct().ToList();

            var albumsCompletos = spotifyService.GetSeveralAlbums(token, ids_albums);

            albumsCompletos = albumsCompletos.GroupBy(c => c.name).Select(g => g.First()).ToList();

            return Json(albumsCompletos.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArtistSingles(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var albums = spotifyService.GetArtistSingles(token, artistId, limit, offset);

            if (!(albums.items.Count > 0))
                return Json(new { Message = "Fim" }, JsonRequestBehavior.AllowGet);

            var ids_albums = new List<string>();
            albums.items.ForEach(c => ids_albums.Add(c.id));

            // different ids
            ids_albums = ids_albums.Distinct().ToList();

            var albumsCompletos = spotifyService.GetSeveralAlbums(token, ids_albums);

            albumsCompletos = albumsCompletos.GroupBy(c => c.name).Select(g => g.First()).ToList();

            return Json(albumsCompletos.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArtistAppearsOn(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var albums = spotifyService.GetArtistAppearsOn(token, artistId, limit, offset);

            if (!(albums.items.Count > 0))
                return Json(new { Message = "Fim" }, JsonRequestBehavior.AllowGet);

            var ids_albums = new List<string>();
            albums.items.ForEach(c => ids_albums.Add(c.id));

            // different ids

            ids_albums = ids_albums.Distinct().ToList();

            var albumsCompletos = spotifyService.GetSeveralAlbums(token, ids_albums);

            albumsCompletos = albumsCompletos.GroupBy(c => c.name).Select(g => g.First()).ToList();

            return Json(albumsCompletos.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetArtistCompilations(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var artistId = Session["artistId"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var albums = spotifyService.GetArtistCompilations(token, artistId, limit, offset);

            if (!(albums.items.Count > 0))
                return Json(new { Message = "Fim" }, JsonRequestBehavior.AllowGet);

            var ids_albums = new List<string>();
            albums.items.ForEach(c => ids_albums.Add(c.id));

            // different ids
            ids_albums = ids_albums.Distinct().ToList();

            var albumsCompletos = spotifyService.GetSeveralAlbums(token, ids_albums);

            albumsCompletos = albumsCompletos.GroupBy(c => c.name).Select(g => g.First()).ToList();

            return Json(albumsCompletos.ToList(), JsonRequestBehavior.AllowGet);
        }

      
    }
}