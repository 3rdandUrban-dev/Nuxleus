/*
// File: IEntity.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright © 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
namespace Nuxleus
{
    public interface IEntity
    {
        string Name { get; set; }

        string ID { get; set; }

        string Term { get; set; }

        string Label { get; set; }

        string Scheme { get; set; }
    }
}