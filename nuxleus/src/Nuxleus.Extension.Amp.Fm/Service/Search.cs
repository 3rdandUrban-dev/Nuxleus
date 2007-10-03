using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Web.Services;
using System.Web;

namespace Nuxleus.Extension.Amp.Fm
{
    /// <summary>
    /// Summary description for Authenticate service
    /// </summary>
    [WebService(Namespace = "http://amp.fm/service")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Search : WebService
    {
        public enum TYPE { TITLE, ARTIST, GENRE, LYRICS }

        public Hashtable SearchTitle(string searchPhrase, params string[] additionalSearchParams)
        {
            return search(searchPhrase, TYPE.TITLE);
        }

        public Hashtable SearchArtist(string artistName, params string[] additionalSearchParams)
        {
            return search(artistName, TYPE.ARTIST);
        }

        public Hashtable SearchGenre(string genre, params string[] additionalSearchParams)
        {
            return search(genre, TYPE.GENRE, additionalSearchParams);
        }

        public Hashtable SearchLyrics(string searchPhrase, params string[] additionalSearchParams)
        {
            return search(searchPhrase, TYPE.LYRICS, additionalSearchParams);
        }

        [WebMethod(EnableSession = true)]
        private Hashtable search(string searchPhrase, TYPE type, params string[] additionalSearchParams)
        {
            Hashtable searchResults = new Hashtable();
            searchResults.Add("foo", "bar");
            return searchResults;
        }
    }
}

