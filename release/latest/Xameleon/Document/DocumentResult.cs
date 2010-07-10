using javax.xml.transform;

namespace Xameleon.Document {

    class DocumentResult : Result {

        string _systemID;

        public DocumentResult() { }

        public string getSystemId() {
            return _systemID;
        }

        public void setSystemId(string str) {
            _systemID = str;
        }
    }
}
