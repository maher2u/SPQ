using SPQ.Models.Spotify_Model.Spotify_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPQ.Controllers
{
    public class TracksController : Controller
    {
        SpotifyService spotifyService = new SpotifyService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData(int? page, int? pageSize)
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();

            int limit = pageSize.Value;
            int offset = (page.Value * pageSize.Value);

            var savedTracksList = spotifyService.GetSavedTracks(token, limit, offset);

            return Json(savedTracksList.items.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult playsong()
        {
            
            return View();
        }
    }
}