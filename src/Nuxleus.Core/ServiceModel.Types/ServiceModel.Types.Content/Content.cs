/*
// File: Content.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright © 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuxleus.ServiceModel.Types.Content
{
    public class Content : IEntity
    {
        public string Name { get; set; }

        public string ID { get; set; }

        public string Term { get; set; }

        public string Label { get; set; }

        public string Scheme { get; set; }

    }
}
