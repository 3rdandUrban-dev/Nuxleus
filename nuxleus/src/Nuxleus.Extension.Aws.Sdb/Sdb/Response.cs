/* -*- Mode: Java; c-basic-offset: 2 -*- */
/*
 * This software code is made available "AS IS" without warranties of any
 * kind.  You may copy, display, modify and redistribute the software
 * code either by itself or as incorporated into your code; provided that
 * you do not remove any proprietary notices.  Your use of this software
 * code is at your own risk and you waive any claim against Amazon
 * Web Services LLC or its affiliates with respect to your use of
 * this software code.
 * 
 * @copyright 2007 Amazon Web Services LLC or its affiliates.
 *            All rights reserved.
 */
using System.Xml;


namespace Nuxleus.Extension.Aws.Sdb
{
    public class Response
    {
        private string requestId;
        public string RequestId
        {
            get
            {
                return requestId;
            }
        }

        private string boxUsage;
        public string BoxUsage
        {
            get
            {
                return boxUsage;
            }
        }

        static protected void CheckErrorResponse (XmlDocument response)
        {
            bool isError = false;
            string errorCode = "";
            string errorMessage = "";
            string requestId = "";

            foreach (XmlNode node in response.ChildNodes)
            {
                if (node.Name.Equals("Response"))
                {
                    foreach (XmlNode responseNode in node.ChildNodes)
                    {
                        if (responseNode.Name.Equals("RequestID"))
                        {
                            requestId = responseNode.InnerText;
                        }
                        if (responseNode.Name.Equals("Errors"))
                        {
                            isError = true;
                            foreach (XmlNode errorsNode in responseNode.ChildNodes)
                            {
                                if (errorsNode.Name.Equals("Error"))
                                {
                                    foreach (XmlNode errorNode in errorsNode.ChildNodes)
                                    {
                                        if (errorNode.Name.Equals("Code"))
                                        {
                                            errorCode = errorNode.InnerText;
                                        }
                                        if (errorNode.Name.Equals("Message"))
                                        {
                                            errorMessage = errorNode.InnerText;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (isError)
            {
                throw new SdbException(requestId, errorCode, errorMessage);
            }
        }

        protected void processResponse (string responseName, XmlDocument response)
        {
            bool success = false;

            foreach (XmlNode node in response.ChildNodes)
            {
                if (node.Name.Equals(responseName))
                {
                    foreach (XmlNode responseNode in node.ChildNodes)
                    {
                        if (responseNode.Name.Equals("ResponseStatus"))
                        {
                            success = true;
                            foreach (XmlNode statusNode in responseNode.ChildNodes)
                            {
                                if (statusNode.Name.Equals("RequestId"))
                                {
                                    requestId = statusNode.InnerText;
                                }
                                if (statusNode.Name.Equals("BoxUsage"))
                                {
                                    boxUsage = statusNode.InnerText;
                                }
                            }
                        }
                    }
                }
            }

            if (!success)
            {
                throw new SdbException("BadResponse",
                               "Sdb returned an improper response.");
            }
        }

        public Response ()
        {
        }

        public Response (string responseName, string response)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            CheckErrorResponse(doc);

            processResponse(responseName, doc);
        }

        public static void CheckSimpleResponse (string responseName,
                           string response)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);
            bool success = false;

            CheckErrorResponse(doc);

            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.Name.Equals(responseName))
                {
                    foreach (XmlNode responseNode in node.ChildNodes)
                    {
                        if (responseNode.Name.Equals("Success"))
                        {
                            success = true;
                        }
                    }
                }
            }

            if (!success)
            {
                throw new SdbException("BadResponse",
                               "Sdb returned an improper response.");
            }
        }
    }
}
