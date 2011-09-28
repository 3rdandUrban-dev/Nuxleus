using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDBConsole {
    public interface IOperation {
        string Command { get; }
        List<string> Invoke();
    }
}
