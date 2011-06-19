/*
// File: Artist.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright © 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
using System.ComponentModel;
using System.Runtime.Serialization;
using Nuxleus.ServiceModel.Types;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;

namespace Nuxleus.ServiceModel.Operations
{
    [Description("GET the Artist info related to the artist specified by {EntityID}\n"
           + "PUT multipart/formdata to update the fields of an existing Artist based on {EntityID} as the account identifier.\n"
           + "POST multipart/formdata to create a new artist using the {ArtistName} as the user chosen identifier.\n")]
    [RestService("/artist/{EntityID}")]
    public class Artist : IEntity
    {
        public string Name { get; set; }

        public string ID { get; set; }

        public string CollectionID { get; set; }

        public string Term { get; set; }

        public string Label { get; set; }

        public string Scheme { get; set; }

    }

    public class ArtistInfo
    {
        public IEntity Entity { get; set; }

    }

    public class ArtistResponse : IHasResponseStatus
    {
        public ArtistInfo ArtistInfo { get; set; }

        //Auto inject and serialize web service exceptions
        public ResponseStatus ResponseStatus { get; set; }
    }
}