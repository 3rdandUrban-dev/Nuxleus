using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus.Entity
{
    public interface IEntity
    {
        string Term { get; set; }
        string Label { get; set; }
        string Scheme { get; set; }
    }
}
