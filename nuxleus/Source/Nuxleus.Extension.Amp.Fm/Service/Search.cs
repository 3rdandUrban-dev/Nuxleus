using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Web.Services;
using System.Web;
using Nuxleus.Entity;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Amp.Fm {
    /// <summary>
    /// Summary description for Search service
    /// </summary>
    [WebService(Namespace="http://amp.fm/service")]
    [WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class Search : WebService {

        public enum TYPE { TITLE, ARTIST, GENRE, LYRICS }

        [WebMethod(
            Description="Returns a list of media file titles that contain the specified search phrase",
            EnableSession=false)]
        [XmlInclude(typeof(Entity.Entity))]
        public Entity.Entity[] SearchTitle ( string searchPhrase, params string[] additionalSearchParams ) {
            return search(searchPhrase, TYPE.TITLE);
        }

        [WebMethod(
            Description="Returns a list of artists that contain the specified search phrase",
            EnableSession=false)]
        [XmlInclude(typeof(Entity.Entity))]
        public Entity.Entity[] SearchArtist ( string artistName ) {
            return search(artistName, TYPE.ARTIST);
        }

        [WebMethod(
            Description="Returns a list of genres that contain the specified search phrase",
            EnableSession=false)]
        [XmlInclude(typeof(Entity.Entity))]
        public Entity.Entity[] SearchGenre ( string genre ) {
            return search(genre, TYPE.GENRE);
        }

        [WebMethod(
            Description="Returns a list of media files whos associated lyrics contain the specified search phrase",
            EnableSession=false)]
        [XmlInclude(typeof(Entity.Entity))]
        public Entity.Entity[] SearchLyrics ( string searchPhrase ) {
            return search(searchPhrase, TYPE.LYRICS);
        }

        [WebMethod]
        [XmlInclude(typeof(Entity.Entity))]
        private Entity.Entity[] search ( string searchPhrase, TYPE type ) {

            string scheme = "http://amp.fm/";

            Entity.Entity[] entityList = new Entity.Entity[] {
                new Entity.Entity("search", searchPhrase, scheme),
                new Entity.Entity("pearljam", "Pearl Jam", scheme),
                new Entity.Entity("foobarfighters", "FooBar Fighters", scheme),
                new Entity.Entity("elliottesmith", "Elliotte Smith", scheme),
            };

            switch (type) {
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

