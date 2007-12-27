using System;
using System.Collections.Generic;
using System.Text;

namespace Nuxleus.Entity
{
    [Serializable]
    public struct Entity : IEntity
    {
        string m_term;
        string m_label;
        string m_scheme;
        static string m_DEFAULTSCHEME = "http://amp.fm/";

        public Entity(string term)
            : this(term, term, m_DEFAULTSCHEME)
        {
        }

        public Entity(string term, string label)
            : this(term, label, m_DEFAULTSCHEME)
        {
        }

        public Entity(string term, string label, string scheme)
        {
            m_term = term;
            m_label = label;
            m_scheme = scheme;
        }

        public string Term { get { return m_term; } set { m_term = value; } }
        public string Label { get { return m_label; } set { m_label = value; } }
        public string Scheme { get { return m_scheme; } set { m_scheme = value; } }
    }
}

