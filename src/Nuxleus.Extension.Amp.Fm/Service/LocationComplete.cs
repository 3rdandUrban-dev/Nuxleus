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
    public class LocationComplete : WebService {

        [WebMethod(
            Description="Returns a collection of locations that most closely match the given RegularExpression",
            EnableSession=false
            )]
        [XmlInclude(typeof(Entity.Entity))]
        public Entity.Entity[] GetLocation ( string regularExpression ) {
            return search(regularExpression);
        }

        [WebMethod]
        [XmlInclude(typeof(Entity.Entity))]
        private Entity.Entity[] search ( string regularExpression ) {
            string scheme = "http://amp.fm/";
            Entity.Entity[] entityArray = new Entity.Entity[] {
                new Entity.Entity{ Label = "SeaFair", Term = "seafair", Scheme = scheme},
                new Entity.Entity{ Label = "Seattle", Term = "seattle", Scheme = scheme},
                new Entity.Entity{ Label = "PSeattle Symphony", Term = "seattlesymphony", Scheme = scheme},
            };

            return entityArray;
        }
    }
}

