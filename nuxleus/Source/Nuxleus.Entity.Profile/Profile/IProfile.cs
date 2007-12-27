using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus.Entity.Profile
{
    public interface IProfile
    {
        IEntity Entity { get; set; }
        Personal Personal { get; set; }
        Preferences Preferences { get; set; }
        Subscriptions Subscriptions { get; set; }
    }
}
