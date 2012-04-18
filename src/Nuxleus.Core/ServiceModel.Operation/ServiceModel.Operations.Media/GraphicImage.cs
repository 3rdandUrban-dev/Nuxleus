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

namespace Nuxleus.ServiceModel.Operations.Media
{
    [RestService("/image/{CollectionID}/{ImageName}")]
    [Description
        (
            "GET the ImageInfo for {ImageName} contained within the collection specified by {CollectionID}.\n" +
            "PUT {ImageName} to update the fields of an existing image contained within the collection specified by {CollectionID}.\n" +
            "POST multipart/formdata to create a new image using the {ImageName} to uniquely identify the image within the collection specified by {CollectionID}.\n" +
            "DELETE {ImageName} contained within the {CollectionID} collection.\n"
        )
    ]
    public class GraphicImage : IEntity
    {
        public string Name { get; set; }

        public string ID { get; set; }

        public string Term { get; set; }

        public string Label { get; set; }

        public string Scheme { get; set; }

    }

    public class GraphicImageInfo
    {
        public IEntity Entity { get; set; }

    }

    public class GraphicImageResponse : IHasResponseStatus
    {
        public GraphicImageInfo GraphicImageInfo { get; set; }

        //Auto inject and serialize web service exceptions
        public ResponseStatus ResponseStatus { get; set; }
    }
}