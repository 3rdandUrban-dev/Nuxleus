using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDBConsole {
    public struct GetAttributes : IOperation {
        public string m_command;
        public string Command { get { return m_command; } }

        public GetAttributes(string command) {
            m_command = command;
        }
        public List<string> Invoke() {
            List<string> list = new List<string>();
            list.Add(Command);
            return list;
        }
    }
}
