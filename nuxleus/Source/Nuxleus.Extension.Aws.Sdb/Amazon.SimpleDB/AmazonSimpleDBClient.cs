/******************************************************************************* 
 *  Copyright 2007 Amazon Technologies, Inc.  
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  
 *  You may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at: http://aws.amazon.com/apache2.0
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 *  specific language governing permissions and limitations under the License.
 * ***************************************************************************** 
 *    __  _    _  ___ 
 *   (  )( \/\/ )/ __)
 *   /__\ \    / \__ \
 *  (_)(_) \/\/  (___/
 * 
 *  Amazon Simple DB CSharp Library
 *  API Version: 2007-11-07
 *  Generated: Thu Dec 27 02:53:43 PST 2007 
 * 
 */

using System;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Globalization;
using System.Xml.Serialization;
using System.Collections.Generic;
using Amazon.SimpleDB.Model;
using Amazon.SimpleDB;


namespace Amazon.SimpleDB
{


   /**

    *
    * AmazonSimpleDBClient is an implementation of AmazonSimpleDB
    *
    */
    public class AmazonSimpleDBClient : AmazonSimpleDB
    {

        private String awsAccessKeyId = null;
        private String awsSecretAccessKey = null;
        private AmazonSimpleDBConfig config = null;

        /// <summary>
        /// Constructs AmazonSimpleDBClient with AWS Access Key ID and AWS Secret Key
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        public AmazonSimpleDBClient(String awsAccessKeyId, String awsSecretAccessKey)
            : this(awsAccessKeyId, awsSecretAccessKey, new AmazonSimpleDBConfig())
        {
        }


        /// <summary>
        /// Constructs AmazonSimpleDBClient with AWS Access Key ID and AWS Secret Key
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        /// <param name="config">configuration</param>
        public AmazonSimpleDBClient(String awsAccessKeyId, String awsSecretAccessKey, AmazonSimpleDBConfig config)
        {
            this.awsAccessKeyId = awsAccessKeyId;
            this.awsSecretAccessKey = awsSecretAccessKey;
            this.config = config;
        }


        // Public API ------------------------------------------------------------//

        
        /// <summary>
        /// Create Domain 
        /// </summary>
        /// <param name="action">Create Domain  Action</param>
        /// <returns>Create Domain  Response from the service</returns>
        /// <remarks>
        /// The CreateDomain operation creates a new domain. The domain name must be unique
        /// among the domains associated with the Access Key ID provided in the request. The CreateDomain
        /// operation may take 10 or more seconds to complete.
        ///   
        /// </remarks>
        public CreateDomainResponse CreateDomain(CreateDomain action)
        {
            return Invoke<CreateDomainResponse>(action.ToMap());
        }

        
        /// <summary>
        /// List Domains 
        /// </summary>
        /// <param name="action">List Domains  Action</param>
        /// <returns>List Domains  Response from the service</returns>
        /// <remarks>
        /// The ListDomains operaton lists all domains associated with the Access Key ID. It returns
        /// domain names up to the limit set by MaxNumberOfDomains. A NextToken is returned if there are more
        /// than MaxNumberOfDomains domains. Calling ListDomains successive times with the
        /// NextToken returns up to MaxNumberOfDomains more domain names each time.
        ///   
        /// </remarks>
        public ListDomainsResponse ListDomains(ListDomains action)
        {
            return Invoke<ListDomainsResponse>(action.ToMap());
        }

        
        /// <summary>
        /// Delete Domain 
        /// </summary>
        /// <param name="action">Delete Domain  Action</param>
        /// <returns>Delete Domain  Response from the service</returns>
        /// <remarks>
        /// The DeleteDomain operation deletes a domain. Any items (and their attributes) in the domain
        /// are deleted as well. The DeleteDomain operation may take 10 or more seconds to complete.
        ///   
        /// </remarks>
        public DeleteDomainResponse DeleteDomain(DeleteDomain action)
        {
            return Invoke<DeleteDomainResponse>(action.ToMap());
        }

        
        /// <summary>
        /// Put Attributes 
        /// </summary>
        /// <param name="action">Put Attributes  Action</param>
        /// <returns>Put Attributes  Response from the service</returns>
        /// <remarks>
        /// The PutAttributes operation creates or replaces attributes within an item. You specify new attributes
        /// using a combination of the Attribute.X.Name and Attribute.X.Value parameters. You specify
        /// the first attribute by the parameters Attribute.0.Name and Attribute.0.Value, the second
        /// attribute by the parameters Attribute.1.Name and Attribute.1.Value, and so on.
        /// 
        /// Attributes are uniquely identified within an item by their name/value combination. For example, a single
        /// item can have the attributes { "first_name", "first_value" } and { "first_name",
        /// second_value" }. However, it cannot have two attribute instances where both the Attribute.X.Name and
        /// Attribute.X.Value are the same.
        /// Optionally, the requestor can supply the Replace parameter for each individual value. Setting this value
        /// to true will cause the new attribute value to replace the existing attribute value(s). For example, if an
        /// item has the attributes { 'a', '1' }, { 'b', '2'} and { 'b', '3' } and the requestor does a
        /// PutAttributes of { 'b', '4' } with the Replace parameter set to true, the final attributes of the
        /// item will be { 'a', '1' } and { 'b', '4' }, replacing the previous values of the 'b' attribute
        /// with the new value.
        ///   
        /// </remarks>
        public PutAttributesResponse PutAttributes(PutAttributes action)
        {
            return Invoke<PutAttributesResponse>(action.ToMap());
        }

        
        /// <summary>
        /// Get Attributes 
        /// </summary>
        /// <param name="action">Get Attributes  Action</param>
        /// <returns>Get Attributes  Response from the service</returns>
        /// <remarks>
        /// Returns all of the attributes associated with the item. Optionally, the attributes returned can be limited to
        /// the specified AttributeName parameter.
        /// If the item does not exist on the replica that was accessed for this operation, an empty attribute is
        /// returned. The system does not return an error as it cannot guarantee the item does not exist on other
        /// replicas.
        ///   
        /// </remarks>
        public GetAttributesResponse GetAttributes(GetAttributes action)
        {
            return Invoke<GetAttributesResponse>(action.ToMap());
        }

        
        /// <summary>
        /// Delete Attributes 
        /// </summary>
        /// <param name="action">Delete Attributes  Action</param>
        /// <returns>Delete Attributes  Response from the service</returns>
        /// <remarks>
        /// Deletes one or more attributes associated with the item. If all attributes of an item are deleted, the item is
        /// deleted.
        ///   
        /// </remarks>
        public DeleteAttributesResponse DeleteAttributes(DeleteAttributes action)
        {
            return Invoke<DeleteAttributesResponse>(action.ToMap());
        }

        
        /// <summary>
        /// Query 
        /// </summary>
        /// <param name="action">Query  Action</param>
        /// <returns>Query  Response from the service</returns>
        /// <remarks>
        /// The Query operation returns a set of ItemNames that match the query expression. Query operations that
        /// run longer than 5 seconds will likely time-out and return a time-out error response.
        /// A Query with no QueryExpression matches all items in the domain.
        ///   
        /// </remarks>
        public QueryResponse Query(Query action)
        {
            return Invoke<QueryResponse>(action.ToMap());
        }

        // Private API ------------------------------------------------------------//

        /**
         * Configure HttpClient with set of defaults as well as configuration
         * from AmazonSimpleDBConfig instance
         */
        private HttpWebRequest ConfigureWebRequest(int contentLength)
        {
            HttpWebRequest request = WebRequest.Create(config.ServiceURL) as HttpWebRequest;

            if (config.IsSetProxyHost())
            {
                request.Proxy = new WebProxy(config.ProxyHost, config.ProxyPort);
            }
            request.UserAgent = config.UserAgent;
            request.Method = "POST";
            request.Timeout = 50000;
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            request.ContentLength = contentLength;

            return request;
        }

        /**
         * Invoke request and return response
         */
        private T Invoke<T>(IDictionary<String, String> parameters)
        {
            String actionName = parameters["Action"];
            T response = default(T);
            String responseBody = null;
            HttpStatusCode statusCode = default(HttpStatusCode);

            /* Add required request parameters */
            AddRequiredParameters(parameters);

            String queryString = GetParametersAsString(parameters);

            byte[] requestData = new UTF8Encoding().GetBytes(queryString);
            bool shouldRetry = true;
            int retries = 0;
            do
            {
                HttpWebRequest request = ConfigureWebRequest(requestData.Length);
                /* Submit the request and read response body */
                try
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(requestData, 0, requestData.Length);
                    }
                    using (HttpWebResponse httpResponse = request.GetResponse() as HttpWebResponse)
                    {
                        statusCode = httpResponse.StatusCode;
                        StreamReader reader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8);
                        responseBody = reader.ReadToEnd();
                    }

                    /* Attempt to deserialize response into <Action> Response type */
                    XmlSerializer serlizer = new XmlSerializer(typeof(T));
                    response = (T)serlizer.Deserialize(new StringReader(responseBody));
                    shouldRetry = false;
                }
                /* Web exception is thrown on unsucessful responses */
                catch (WebException we)
                {
                    shouldRetry = false;
                    using (HttpWebResponse httpErrorResponse = (HttpWebResponse)we.Response as HttpWebResponse)
                    {
                        if (httpErrorResponse == null)
                        {
                            throw new AmazonSimpleDBException(we);
                        }
                        statusCode = httpErrorResponse.StatusCode;
                        StreamReader reader = new StreamReader(httpErrorResponse.GetResponseStream(), Encoding.UTF8);
                        responseBody = reader.ReadToEnd();
                    }

                    if (statusCode == HttpStatusCode.InternalServerError || statusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        shouldRetry = true;
                        PauseOnRetry(++retries, statusCode);
                    }
                    else
                    {

                        /* Attempt to deserialize response into ErrorResponse type */
                        try
                        {
                            XmlSerializer serlizer = new XmlSerializer(typeof(ErrorResponse));
                            ErrorResponse errorResponse = (ErrorResponse)serlizer.Deserialize(new StringReader(responseBody));
                            Error error = errorResponse.Error[0];

                            /* Throw formatted exception with information available from the error response */
                            throw new AmazonSimpleDBException(
                                error.Message,
                                statusCode,
                                error.Code,
                                error.Type,
                                null,
                                errorResponse.RequestId,
                                errorResponse.ToXML());
                        }
                        /* Rethrow on deserializer error */
                        catch (Exception e)
                        {
                            if (e is AmazonSimpleDBException)
                            {
                                throw e;
                            }
                            else
                            {
                                AmazonSimpleDBException se = ReportAnyErrors(responseBody, statusCode, e);
                                throw se;
                            }
                        }
                    }
                }

                /* Catch other exceptions, attempt to convert to formatted exception, 
                 * else rethrow wrapped exception */
                catch (Exception e)
                {
                    throw new AmazonSimpleDBException(e);
                }
            } while (shouldRetry);

            return response;
        }


        /**
         * Look for additional error strings in the response and return formatted exception
         */
        private AmazonSimpleDBException ReportAnyErrors(String responseBody, HttpStatusCode status, Exception e)
        {

            AmazonSimpleDBException ex = null;

            if (responseBody != null && responseBody.StartsWith("<"))
            {
                Match errorMatcherOne = Regex.Match(responseBody, "<RequestId>(.*)</RequestId>.*<Error>" +
                        "<Code>(.*)</Code><Message>(.*)</Message></Error>.*(<Error>)?", RegexOptions.Multiline);
                Match errorMatcherTwo = Regex.Match(responseBody, "<Error><Code>(.*)</Code><Message>(.*)" +
                        "</Message></Error>.*(<Error>)?.*<RequestID>(.*)</RequestID>", RegexOptions.Multiline);
                Match errorMatcherThree = Regex.Match(responseBody, "<Error><Code>(.*)</Code><Message>(.*)" +
                        "</Message><BoxUsage>(.*)</BoxUsage></Error>.*(<Error>)?.*<RequestID>(.*)</RequestID>", RegexOptions.Multiline);

                if (errorMatcherOne.Success)
                {
                    String requestId = errorMatcherOne.Groups[1].Value;
                    String code = errorMatcherOne.Groups[2].Value;
                    String message = errorMatcherOne.Groups[3].Value;

                    ex = new AmazonSimpleDBException(message, status, code, "Unknown", null, requestId, responseBody);

                }
                else if (errorMatcherTwo.Success)
                {
                    String code = errorMatcherTwo.Groups[1].Value;
                    String message = errorMatcherTwo.Groups[2].Value;
                    String requestId = errorMatcherTwo.Groups[4].Value;

                    ex = new AmazonSimpleDBException(message, status, code, "Unknown", null, requestId, responseBody);
                }
                else if (errorMatcherThree.Success)
                {
                    String code = errorMatcherThree.Groups[1].Value;
                    String message = errorMatcherThree.Groups[2].Value;
                    String boxUsage = errorMatcherThree.Groups[3].Value;
                    String requestId = errorMatcherThree.Groups[5].Value;

                    ex = new AmazonSimpleDBException(message, status, code, "Unknown", boxUsage, requestId, responseBody);
                }
                else
                {
                    ex = new AmazonSimpleDBException("Internal Error", status);
                }
            }
            else
            {
                ex = new AmazonSimpleDBException("Internal Error", status);
            }
            return ex;
        }

        /**
         * Exponential sleep on failed request
         * @param retries current retry
         * @throws AmazonSimpleDBException if maximum number of retries has been reached
         * @throws java.lang.InterruptedException 
         */
        private void PauseOnRetry(int retries, HttpStatusCode status)
        {
            if (retries <= config.MaxErrorRetry)
            {
                int delay = (int)Math.Pow(4, retries) * 100;
                System.Threading.Thread.Sleep(delay);
            }
            else
            {
                throw new AmazonSimpleDBException("Maximum number of retry attempts reached : " + (retries - 1), status);
            }
        }

        /**
         * Add authentication related and version parameters
         */
        private void AddRequiredParameters(IDictionary<String, String> parameters)
        {
            parameters.Add("AWSAccessKeyId", this.awsAccessKeyId);
            parameters.Add("Timestamp", GetFormattedTimestamp());
            parameters.Add("Version", config.ServiceVersion);
            parameters.Add("SignatureVersion", config.SignatureVersion);
            parameters.Add("Signature", SignParameters(parameters, this.awsSecretAccessKey));
        }

        /**
         * Convert Disctionary of paremeters to Url encoded query string
         */
        private string GetParametersAsString(IDictionary<String, String> parameters)
        {
            StringBuilder data = new StringBuilder();
            foreach (String key in (IEnumerable<String>)parameters.Keys)
            {
                String value = parameters[key];
                if (value != null && value.Length > 0)
                {
                    data.Append(key);
                    data.Append('=');
                    data.Append(HttpUtility.UrlEncodeUnicode (value));
                    data.Append('&');
                }    
            }
            String stringData = data.ToString();
            if (stringData.EndsWith("&")) 
            {
                stringData = stringData.Remove(stringData.Length - 1, 1);
            }
            return stringData;
        }

        /**
         * Computes RFC 2104-compliant HMAC signature for request parameters
         * Implements AWS Signature, as per following spec:
         *
         * If Signature Version is 0, it signs concatenated Action and Timestamp
         *
         * If Signature Version is 1, it performs the following:
         *
         * Sorts all  parameters (including SignatureVersion and excluding Signature,
         * the value of which is being created), ignoring case.
         *
         * Iterate over the sorted list and append the parameter name (in original case)
         * and then its value. It will not URL-encode the parameter values before
         * constructing this string. There are no separators.
         */
        private String SignParameters(IDictionary<String, String> parameters, String key)
        {
            String signatureVersion = parameters["SignatureVersion"];
            StringBuilder data = new StringBuilder();

            if ("0".Equals(signatureVersion))
            {
                data.Append(parameters["Action"]).Append(parameters["Timestamp"]);
            }
            else if ("1".Equals(signatureVersion))
            {
                IDictionary<String, String> sorted =
                    new SortedDictionary<String, String>(parameters, StringComparer.InvariantCultureIgnoreCase);
                parameters.Remove("Signature");
                foreach (KeyValuePair<String, String> pair in sorted)
                {
                    if (pair.Value != null && pair.Value.Length > 0)
                    {
                        data.Append(pair.Key);
                        data.Append(pair.Value);
                    }
                }
            }
            else
            {
                throw new Exception("Invalid Signature Version specified");
            }
            return Sign(data.ToString(), key);
        }

        /**
         * Computes RFC 2104-compliant HMAC signature.
         */
        private String Sign(String data, String key)
        {
            Encoding encoding = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1(encoding.GetBytes(key));
            return Convert.ToBase64String(signature.ComputeHash(
                encoding.GetBytes(data.ToCharArray())));
        }


        /**
         * Formats date as ISO 8601 timestamp
         */
        private String GetFormattedTimestamp()
        {
            DateTime dateTime = DateTime.Now;
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                                 dateTime.Hour, dateTime.Minute, dateTime.Second,
                                 dateTime.Millisecond
                                 , DateTimeKind.Local
                               ).ToUniversalTime().ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z",
                                CultureInfo.InvariantCulture);
        }

    }
}