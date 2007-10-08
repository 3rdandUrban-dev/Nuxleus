using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus.Entity
{
    [Serializable]
    public struct Entity : IEntity
    {
        string _term;
        string _label;
        string _scheme;
        static string _DEFAULTSCHEME = "http://amp.fm/";

        public Entity(string term)
            : this(term, term, _DEFAULTSCHEME)
        {
        }

        public Entity(string term, string label)
            : this(term, label, _DEFAULTSCHEME)
        {
        }

        public Entity(string term, string label, string scheme)
        {
            _term = term;
            _label = label;
            _scheme = scheme;
        }

        public string Term { get { return _term; } set { this._term = value; } }
        public string Label { get { return _label; } set { this._label = value; } }
        public string Scheme { get { return _scheme; } set { this._scheme = value; } }
    }
}

