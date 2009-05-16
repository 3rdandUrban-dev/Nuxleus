using System;
using System.Collections;
using System.Diagnostics;
using System.Web;

namespace Xameleon.Function {

    public class HttpRequestCollection {

        static string notSet = "not-set";

        public static string GetValue ( HttpRequest request, string type, string key ) {
            try {
                switch (type) {
                    case "cookie":
                        if (request.Cookies.Count > 0) {
                            IEnumerator enumerator = request.Cookies.GetEnumerator();
                            for (int i = 0; enumerator.MoveNext(); i++) {
                                string local = request.Cookies.AllKeys[i].ToString();
                                if (local == key) {
                                    return request.Cookies[local].Value;
                                }
                            }
                            return notSet;
                        }
                        return notSet;

                    case "form":
                        if (request.Form.Count > 0) {
                            IEnumerator enumerator = request.Form.GetEnumerator();
                            for (int i = 0; enumerator.MoveNext(); i++) {

                                string local = request.Form.AllKeys[i].ToString();
                                Console.WriteLine("Form Value {0}", local);
                                if (local == key) {
                                    return request.Form[local];
                                }
                            }
                            return notSet;
                        }
                        return notSet;

                    case "query-string":
                        if (request.QueryString.Count > 0) {
                            IEnumerator enumerator = request.QueryString.GetEnumerator();
                            for (int i = 0; enumerator.MoveNext(); i++) {
                                string local = request.QueryString.AllKeys[i].ToString();
                                if (local == key) {
                                    return request.QueryString[local];
                                }
                            }
                            return notSet;
                        }
                        return notSet;
                    case "server-variable":
                        if (request.ServerVariables.Count > 0) {
                            IEnumerator enumerator = request.ServerVariables.GetEnumerator();
                            for (int i = 0; enumerator.MoveNext(); i++) {
                                string local = request.ServerVariables.AllKeys[i].ToString();
                                if (local == key) {
                                    return request.ServerVariables[local];
                                }
                            }
                            return notSet;
                        }
                        return notSet;

                    case "header":
                        if (request.Headers.Count > 0) {
                            IEnumerator enumerator = request.Headers.GetEnumerator();
                            for (int i = 0; enumerator.MoveNext(); i++) {
                                string local = request.Headers.AllKeys[i].ToString();
                                if (local == key) {
                                    return request.Headers[local];
                                }
                            }
                            return notSet;
                        }
                        return notSet;
                    case "file":
                        if (request.Files.Count > 0) {
                            foreach (string fieldName in request.Files.AllKeys) {
                                if (fieldName == key) {
                                    return fieldName;
                                }
                            }
                        }
                        return notSet;
                    default:
                        return notSet;
                }

            } catch (Exception e) {
                Debug.WriteLine("Error: " + e.Message);
                return e.Message;
            }
        }
    }
}
//EXPERIMENTAL: PERFORMANCE SEEMS NOTICEABLY SLOWER WITH THE CODE BELOW, 
//BUT WANT TO KEEP IT AROUND UNTIL I HAVE A CHANCE TO BENCHMARK
//using System;
//using System.Collections;
//using System.IO;
//using System.Net;
//using System.Diagnostics;
//using System.Collections.Specialized;
//using System.Web;

//namespace Xameleon.Function {

//    public class HttpRequestCollection {

//        static string notSet = "not-set";

//        public static string GetValue(HttpRequest request, string type, string key) {
//            try {
//                switch (type) {
//                    case "cookie":
//                        if (request.Cookies.Count > 0) {
//                            try {
//                                return request.Cookies[key].Value;
//                            } catch {
//                                return notSet;
//                            }
//                        } else {
//                            return notSet;
//                        }
//                        break;

//                    case "form":
//                        if (request.Form.Count > 0) {
//                            try {
//                                return request.Form[key];
//                            } catch {
//                                return notSet;
//                            }
//                        } else {
//                            return notSet;
//                        }
//                        break;

//                    case "query-string":
//                        if (request.QueryString.Count > 0) {
//                            try {
//                                return request.QueryString[key];
//                            } catch {
//                                return notSet;
//                            }
//                        } else {
//                            return notSet;
//                        }
//                        break;
//                    case "server-variable":
//                        if (request.ServerVariables.Count > 0) {
//                            try {
//                                return request.ServerVariables[key];
//                            } catch {
//                                return notSet;
//                            }
//                        } else {
//                            return notSet;
//                        }
//                        break;

//                    case "header":
//                        if (request.Headers.Count > 0) {
//                            try {
//                                return request.Headers[key];
//                            } catch {
//                                return notSet;
//                            }
//                        } else {
//                            return notSet;
//                        }
//                        break;
//                    default:
//                        return notSet;
//                        break;
//                }

//            } catch (Exception e) {
//                Debug.WriteLine("Error: " + e.Message);
//                return e.Message;
//            }

//            return notSet;
//        }
//    }
//}



