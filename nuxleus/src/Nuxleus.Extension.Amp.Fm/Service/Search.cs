using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Web.Services;
using System.Web;
using Nuxleus;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Amp.Fm
{
    /// <summary>
    /// Summary description for Search service
    /// </summary>
    [WebService(Namespace = "http://amp.fm/service")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Search : WebService
    {
        public enum TYPE { TITLE, ARTIST, GENRE, LYRICS }

        [WebMethod(
            Description = "Returns a list of media file titles that contain the specified search phrase",
            EnableSession = false)]
        [XmlInclude(typeof(Entity))]
        public ArrayList SearchTitle(string searchPhrase, params string[] additionalSearchParams)
        {
            return search(searchPhrase, TYPE.TITLE);
        }

        [WebMethod(
            Description = "Returns a list of artists that contain the specified search phrase",
            EnableSession = false)]
        [XmlInclude(typeof(Entity))]
        public ArrayList SearchArtist(string artistName)
        {
            return search(artistName, TYPE.ARTIST);
        }

        [WebMethod(
            Description = "Returns a list of genres that contain the specified search phrase",
            EnableSession = false)]
        [XmlInclude(typeof(Entity))]
        public ArrayList SearchGenre(string genre)
        {
            return search(genre, TYPE.GENRE);
        }

        [WebMethod(
            Description = "Returns a list of media files whos associated lyrics contain the specified search phrase",
            EnableSession = false)]
        [XmlInclude(typeof(Entity))]
        public ArrayList SearchLyrics(string searchPhrase)
        {
            return search(searchPhrase, TYPE.LYRICS);
        }

        [WebMethod]
        [XmlInclude(typeof(Entity))]
        private ArrayList search(string searchPhrase, TYPE type)
        {
            ArrayList entityList = new ArrayList();
            string scheme = "http://amp.fm/";
            Entity entity1 = new Entity("pearljam", "Pearl Jam", scheme);
            Entity entity2 = new Entity("foobarfighters", "FooBar Fighters", scheme);
            Entity entity3 = new Entity("elliottesmith", "Elliotte Smith", scheme);

            entityList.Add(entity1);
            entityList.Add(entity2);
            entityList.Add(entity3);

            switch (type)
            {
                case TYPE.ARTIST:
                    return entityList;
                    break;
                case TYPE.GENRE:
                    return entityList;
                    break;
                case TYPE.LYRICS:
                    return entityList;
                    break;
                case TYPE.TITLE:
                    return entityList;
                    break;
                default:
                    return entityList;
                    break;
            }
        }
    }
}

