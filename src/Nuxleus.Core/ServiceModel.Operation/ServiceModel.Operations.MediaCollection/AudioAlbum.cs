/*
// File: Album.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
using System.ComponentModel;
using System.Runtime.Serialization;
using Nuxleus.ServiceModel.Types;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;

namespace Nuxleus.ServiceModel.Operations.MediaCollections
{
    [RestService("/album/{CollectionName}/{AlbumName}")]
    [Description
        (
            "GET the AlbumInfo for {AlbumName} contained in the {CollectionName} collection.\n" + 
            "PUT {AlbumName} to update the fields of an existing album contained within the collection specified by {CollectionName}.\n" + 
            "POST multipart/formdata to create a new album using the {AlbumName} to uniquely identify the album within the collection specified by {CollectionName}.\n" + 
            "DELETE {AlbumName} contained in the {CollectionName} collection.\n"
        )
    ]
    public class AudioAlbum : IEntity
    {
        public string Name { get; set; }

        public string ID { get; set; }

        public string Term { get; set; }

        public string Label { get; set; }

        public string Scheme { get; set; }
    }

    public class AudioAlbumInfo
    {
        public AudioAlbum Entity { get; set; }

    }

    public class AudioAlbumResponse : IHasResponseStatus
    {
        public AudioAlbumInfo AudioAlbumInfo { get; set; }

        //Auto inject and serialize web service exceptions
        public ResponseStatus ResponseStatus { get; set; }
    }
}