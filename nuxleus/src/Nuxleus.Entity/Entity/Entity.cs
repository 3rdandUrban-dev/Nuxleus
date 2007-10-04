using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus
{
    [Serializable]
    public class Entity : IEntity
    {
        string _term;
        string _label;
        string _scheme;

        public Entity() { }

        public string Term { get { return _term; } set { this._term = value; } }
        public string Label { get { return _label; } set { this._label = value; } }
        public string Scheme { get { return _scheme; } set { this._scheme = value; } }
    }
}

