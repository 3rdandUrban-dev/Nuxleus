using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus.Entity
{
    public class IPerson : IEntity
    {
        string IEntity.Term
        { get; set; }

        string IEntity.Label
        { get; set; }

        string IEntity.Scheme
        { get; set; }
    }
}
