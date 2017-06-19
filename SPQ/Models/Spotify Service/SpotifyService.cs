using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using SPQ.Models.Spotify_Model;
using System.Web.Script.Serialization;

namespace SPQ.Models.Spotify_Model.Spotify_Service
{
    public class SpotifyService
    {
        public SpotifyService()
        {

        }

        //Method that will execute the web request and convert the JSON that is returned from the Web API.

        private T GetSpotifyType<T>(string token, string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Set("Authorization", "Bearer " + token);
                request.ContentType = "application/json; charset=utf-8";

                T type = default(T);

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            string responseFromServer = sr.ReadToEnd();

                            JavaScriptSerializer serializer = new JavaScriptSerializer() { /*MaxJsonLength = 500000000*/ MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };
                            type = serializer.Deserialize<T>(responseFromServer);
                            //type = JsonConvert.DeserializeObject<T>(responseFromServer);
                        }
                    }
                }

                return type;
            }
            catch (WebException)
            {
                return default(T);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Retrieve an access token for the application

        private string GetAccessToken()
        {
            SpotifyToken token = new SpotifyToken();

            string postString = string.Format("grant_type=client_credentials");
            byte[] byteArray = Encoding.UTF8.GetBytes(postString);

            string url = "https://accounts.spotify.com/api/token";

            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.Headers.Add("Authorization", "Basic OWYyOGRmMzgxOTc2NDg5ZDhjNDY3YTJiYWM3NGIxMDk6MDg4Zjc0NWU0YjMwNGJiMDlhNDM3NmRiM2IwYjhhOTA=");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string responseFromServer = reader.ReadToEnd();
                            token = JsonConvert.DeserializeObject<SpotifyToken>(responseFromServer);
                        }
                    }
                }
            }
            return token.access_token;
        }

        public string GetAuthorizationUri()
        {
            string clientId = ConfigurationManager.AppSettings["CLIENT_ID"].ToString();
            string redirectUri = ConfigurationManager.AppSettings["REDIRECT_URI"].ToString();
            string scope = "playlist-read-private playlist-read-collaborative user-read-private user-read-birthdate user-read-email user-library-read user-follow-read"; //ConfigurationManager.AppSettings["SCOPE"].ToString();

            return "https://accounts.spotify.com/en/authorize?client_id=" + clientId + "&response_type=token&redirect_uri=" + redirectUri + "&state=&scope=" + scope + "&show_dialog=true";
        }

        //User Profiles
        //Get detailed profile information about the current user(including the username of the current user).

        public UserProfiles GetCurrentUserProfile(string token)
        {
            string url = "https://api.spotify.com/v1/me";

            UserProfiles spotifyUser = GetSpotifyType<UserProfiles>(token, url);

            return spotifyUser;
        }
       

         //Follow
        //Get the artists followed by the current user.

        public MainArtists GetFollowedArtists(string token)
        {
            string url = "https://api.spotify.com/v1/me/following?limit=50&type=artist";

            var followedArtists = GetSpotifyType<MainArtists>(token, url);

            return followedArtists;
        }
        

         //User Library
        
        public SavedAlbums GetSavedAlbums(string token, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/me/albums?limit={0}&offset={1}", limit, offset);

            var savedAlbums = GetSpotifyType<SavedAlbums>(token, url);

            return savedAlbums;
        }

        //Get a list of songs saved in the "Your Music" library of the current Spotify user.

        public SavedTracks GetSavedTracks(string token, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/me/tracks?limit={0}&offset={1}", limit, offset);

            var savedTracks = GetSpotifyType<SavedTracks>(token, url);

            return savedTracks;
        }
        

        //Playlists
        
        //Get a list of the owned playlists or followed by the current Spotify user.

        public List<Playlist> GetPlaylists(string token, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/me/playlists?limit={0}&offset={1}", limit, offset);

            var playlists = GetSpotifyType<Playlists>(token, url);

            return playlists.items.ToList();
        }

        public List<Playlist> GetUserPlaylists(string token, string userId, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/users/{0}/playlists?limit={1}&offset={2}", userId, limit, offset);

            var playlists = GetSpotifyType<Playlists>(token, url);

            return playlists.items.ToList();
        }

        public Playlist GetPlaylist(string token, string userId, string playlistId)
        {
            string url = string.Format("https://api.spotify.com/v1/users/{0}/playlists/{1}", userId, playlistId);

            var playlist = GetSpotifyType<Playlist>(token, url);

            return playlist;
        }

        public List<PlaylistTrackItem> GetPlaylistTracks(string token, string userId, string playlistId, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/users/{0}/playlists/{1}/tracks?limit={2}&offset={3}", userId, playlistId, limit, offset);

            var tracks = GetSpotifyType<PlaylistTracks>(token, url);

            return tracks.items.ToList();
        }
        

        //Artists
        public ItemArtist GetArtist(string token, string artistId)
        {
            string url = string.Format("https://api.spotify.com/v1/artists/{0}", artistId);

            var artist = GetSpotifyType<ItemArtist>(token, url);

            return artist;
        }

        public Albums GetArtistAlbums(string token, string artistId, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/artists/{0}/albums?album_type=album&limit={1}&offset={2}", artistId, limit, offset);

            var albums = GetSpotifyType<Albums>(token, url);

            return albums;
        }

        public Albums GetArtistSingles(string token, string artistId, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/artists/{0}/albums?album_type=single&limit={1}&offset={2}", artistId, limit, offset);

            var albums = GetSpotifyType<Albums>(token, url);

            return albums;
        }

        public Albums GetArtistAppearsOn(string token, string artistId, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/artists/{0}/albums?album_type=appears_on&limit={1}&offset={2}", artistId, limit, offset);

            var albums = GetSpotifyType<Albums>(token, url);

            return albums;
        }

        public Albums GetArtistCompilations(string token, string artistId, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/artists/{0}/albums?album_type=compilation&limit={1}&offset={2}", artistId, limit, offset);

            var albums = GetSpotifyType<Albums>(token, url);

            return albums;
        }

        public List<ItemArtist> GetSeveralArtists(string token, List<string> artistIds)
        {
            // Add the ids in a string ready to query the service

            StringBuilder builderIds = new StringBuilder();
            foreach (var id in artistIds)
            {
                builderIds.Append(id + ",");
            }

            string ids = builderIds.Remove(builderIds.Length - 1, 1).ToString();

            string url = string.Format("https://api.spotify.com/v1/artists?ids={0}", ids);

            var artists = GetSpotifyType<SeveralArtists>(token, url);

            return artists.artists;
        }

        public List<Track> GetArtistTopTracks(string token, string artistId)
        {
            string url = string.Format("https://api.spotify.com/v1/artists/{0}/top-tracks?country=US", artistId);

            var topTracks = GetSpotifyType<ArtistTopTracks>(token, url);

            return topTracks.tracks;
        }

        public List<ItemArtist> GetRelatedArtists(string token, string artistId)
        {
            string url = string.Format("https://api.spotify.com/v1/artists/{0}/related-artists", artistId);

            var artists = GetSpotifyType<RelatedArtists>(token, url);

            return artists.artists;
        }

        public bool CheckIfCurrentUserFollowsArtists(string token, string artistId)
        {
            string url = string.Format("https://api.spotify.com/v1/me/following/contains?type=artist&ids={0}", artistId);

            var isFollowing = GetSpotifyType<bool>(token, url);

            return isFollowing;
        }

        

        //Tracks
        public Track GetTrack(string token, string trackId)
        {
            string url = string.Format("https://api.spotify.com/v1/tracks/{0}", trackId);

            var track = GetSpotifyType<Track>(token, url);

            return track;
        }

        public AudioFeaturesTrack GetAudioFeaturesTrack(string token, string trackId)
        {
            string url = string.Format("https://api.spotify.com/v1/audio-features/{0}", trackId);

            var features = GetSpotifyType<AudioFeaturesTrack>(token, url);

            return features;
        }
        

        //Albums
        public Album GetAlbum(string token, string albumId)
        {
            string url = string.Format("https://api.spotify.com/v1/albums/{0}", albumId);

            var album = GetSpotifyType<Album>(token, url);

            return album;
        }

        public List<Track> GetAlbumTracks(string token, string albumId, int limit, int offset)
        {
            if (limit <= 0)
                limit = 0;
            if (offset <= 0)
                offset = 0;

            string url = string.Format("https://api.spotify.com/v1/albums/{0}/tracks?limit={1}&offset={2}", albumId, limit, offset);

            var tracks = GetSpotifyType<Tracks>(token, url);

            return tracks.items.ToList();
        }

        public List<Album> GetSeveralAlbums(string token, List<string> albumsIds)
        {
            // Add the ids in a string ready to query the service

            StringBuilder builderIds = new StringBuilder();
            foreach (var id in albumsIds)
            {
                builderIds.Append(id + ",");
            }

            string ids = builderIds.Remove(builderIds.Length - 1, 1).ToString();

            string url = string.Format("https://api.spotify.com/v1/albums?ids={0}", ids);

            var albums = GetSpotifyType<SeveralAlbums>(token, url);

            return albums.albums;
        }
        

    }
}