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

namespace Nuxleus.ServiceModel.Operations.MediaCollections
{
    [RestService("/photoalbum/{CollectionName}/{PhotoAlbumName}")]
    [Description
        (
            "GET the AlbumInfo for {PhotoPhotoAlbumName} contained in the {CollectionName} collection.\n" +
            "PUT {PhotoAlbumName} to update the fields of an existing album contained within the collection specified by {CollectionName}.\n" + 
            "POST multipart/formdata to create a new album using the {PhotoAlbumName} to uniquely identify the album within the collection specified by {CollectionName}.\n" +
            "DELETE {PhotoAlbumName} contained in the {CollectionName} collection.\n"
        )
    ]
    public class PhotoAlbum
    {
        public string PhotoAlbumName { get; set; }

        public string CollectionName { get; set; }
    }
}