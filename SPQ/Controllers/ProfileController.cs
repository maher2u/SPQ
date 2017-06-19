using SPQ.Models.Spotify_Model.Spotify_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPQ.Controllers
{
    public class ProfileController : Controller
    {
        SpotifyService spotifyService = new SpotifyService();

        // GET: Profile
        public ActionResult Index()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var token = Session["token"].ToString();
            var spotifyUser = spotifyService.GetCurrentUserProfile(token);

            ViewBag.DateOfBirth = SpotifyHelpers.FormattedBirthdate(spotifyUser.birthdate);
            ViewBag.country = SpotifyHelpers.GetCountryName(spotifyUser.country);

            return View(spotifyUser);
        }
    }
}