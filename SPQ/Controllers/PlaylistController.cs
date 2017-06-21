﻿using SPQ.Models.Spotify_Model;
using SPQ.Models.Spotify_Model.Spotify_Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SPQ.Controllers
{
    public class PlaylistController : Controller
    {
        SpotifyService spotifyService = new SpotifyService();

        //List playlists

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetPlaylists(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            var spotifyUser = spotifyService.GetCurrentUserProfile(token);

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var playlists = spotifyService.GetUserPlaylists(token, spotifyUser.id, limit, offset);

            return Json(playlists.ToList(), JsonRequestBehavior.AllowGet);
        }


        // Playlist by Id       

        public ActionResult Playlist(string id)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            UserProfiles spotifyUser = spotifyService.GetCurrentUserProfile(token);
            Playlist playlist = spotifyService.GetPlaylist(token, spotifyUser.id, id);

            Session["SpotifyUser"] = spotifyUser;
            Session["Playlist"] = playlist;
            Session["spotifyUserId"] = spotifyUser.id;
            Session["playlistId"] = playlist.id;
            Session["totalTracksCount"] = playlist.tracks.total;

            ViewBag.PlaylistId = playlist.id;
            ViewBag.Image = playlist.images[0].url;
            ViewBag.PlaylistName = playlist.name;
            ViewBag.Followers = playlist.followers.total;
            ViewBag.Owner = playlist.owner.id;
            ViewBag.Tracks = playlist.tracks.total;
            ViewBag.publicc = playlist.publicc == true ? "Yes" : "No";

            var tracksFromPlaylist = spotifyService.GetPlaylistTracks(token, spotifyUser.id, playlist.id, 50, 0);

            return View(tracksFromPlaylist.ToList());
        }
        public ActionResult GetTracks(int? page)
        {
            if (string.IsNullOrEmpty(Session["token"].ToString()))
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            string userId = Session["spotifyUserId"].ToString();
            string playlistId = Session["playlistId"].ToString();
            int limit = 50;
            var pagina = page ?? 0;
            int offset = (pagina * limit);
            int totalRecordCount = (int)Session["totalTracksCount"];

            var tracksFromPlaylist = spotifyService.GetPlaylistTracks(token, userId, playlistId, limit, offset);

            

            return Json(tracksFromPlaylist.ToList(), JsonRequestBehavior.AllowGet);
        }

    }
}