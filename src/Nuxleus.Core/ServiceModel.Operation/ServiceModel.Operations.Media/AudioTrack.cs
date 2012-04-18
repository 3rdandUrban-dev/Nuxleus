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
    [RestService("/audiotrack/{CollectionID}/{TrackName}")]
    [Description
        (
            "GET the TrackInfo for {TrackName} contained within the collection specified by {CollectionID}.\n" +
            "PUT {TrackName} to update the fields of an existing album contained within the collection specified by {CollectionID}.\n" +
            "POST multipart/formdata to create a new album using the {TrackName} to uniquely identify the album within the collection specified by {CollectionID}.\n" +
            "DELETE {TrackName} contained within the {CollectionID} collection.\n"
        )
    ]
    public class AudioTrack
    {
        public string TrackName { get; set; }

        public string CollectionID { get; set; }
    }

    public class AudioTrackEntity : IEntity
    {
        public string Name { get; set; }

        public string ID { get; set; }

        public string Term { get; set; }

        public string Label { get; set; }

        public string Scheme { get; set; }

    }

    public class AudioTrackInfo
    {
        public AudioTrackEntity Entity { get; set; }

    }

    public class AudioTrackResponse : IHasResponseStatus
    {
        public AudioTrackInfo AudioTrackInfo { get; set; }

        //Auto inject and serialize web service exceptions
        public ResponseStatus ResponseStatus { get; set; }
    }
}