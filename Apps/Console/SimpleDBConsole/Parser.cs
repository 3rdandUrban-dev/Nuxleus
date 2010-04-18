using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDBConsole {

    public struct Parser {

        public static bool TryParseLine(string line, out IOperation operation) {
            switch (line) {
                case "list\n":
                    operation = new ListDomains(line);
                    return true;
                case "create\n":
                    operation = new CreateDomain(line);
                    return true;
                case "delete\n":
                    operation = new DeleteDomain(line);
                    return true;
                case "put\n":
                    operation = new PutAttributes(line);
                    return true;
                case "get\n":
                    operation = new GetAttributes(line);
                    return true;
                case "query\n":
                    operation = new Query(line);
                    return true;
                default:
                    operation = null;
                    return false;
            }
        }
    }
}
