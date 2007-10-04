using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Web.Services;
using System.Web;
using Amp.Fm.EntityType;
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
        public ArrayList SearchArtist(string artistName, params string[] additionalSearchParams)
        {
            return search(artistName, TYPE.ARTIST);
        }

        [WebMethod(
            Description = "Returns a list of genres that contain the specified search phrase",
            EnableSession = false)]
        [XmlInclude(typeof(Entity))]
        public ArrayList SearchGenre(string genre, params string[] additionalSearchParams)
        {
            return search(genre, TYPE.GENRE, additionalSearchParams);
        }

        [WebMethod(
            Description = "Returns a list of media files whos associated lyrics contain the specified search phrase",
            EnableSession = false)]
        [XmlInclude(typeof(Entity))]
        public ArrayList SearchLyrics(string searchPhrase, params string[] additionalSearchParams)
        {
            return search(searchPhrase, TYPE.LYRICS, additionalSearchParams);
        }

        [WebMethod]
        [XmlInclude(typeof(Entity))]
        private ArrayList search(string searchPhrase, TYPE type, params string[] additionalSearchParams)
        {
            ArrayList entityList = new ArrayList();
            Entity entity1 = new Entity();
            Entity entity2 = new Entity();
            Entity entity3 = new Entity();

            entity1.Scheme = "http://amp.fm/";
            entity2.Scheme = "http://amp.fm/";
            entity3.Scheme = "http://amp.fm/";

            entity1.Term = "pearljam";
            entity2.Term = "foobarfighters";
            entity3.Term = "elliottesmith";

            entity1.Label = "Pearl Jam";
            entity2.Label = "FooBar Fighters";
            entity3.Label = "Elliotte Smith";

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

