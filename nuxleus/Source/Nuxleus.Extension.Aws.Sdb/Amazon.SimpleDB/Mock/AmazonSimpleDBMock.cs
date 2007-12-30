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
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Amazon.SimpleDB.Model;

namespace Amazon.SimpleDB.Mock
{

    /// <summary>
    /// AmazonSimpleDBMock is the implementation of AmazonSimpleDB based
    /// on the pre-populated set of XML files that serve local data. It simulates 
    /// responses from Amazon Simple DB service.
    /// </summary>
    /// <remarks>
    /// Use this to test your application without making a call to 
    /// Amazon Simple DB 
    /// 
    /// Note, current Mock Service implementation does not valiadate requests
    /// </remarks>
    public  class AmazonSimpleDBMock : AmazonSimpleDB {
    

        // Public API ------------------------------------------------------------//
    
        
        /// <summary>
        /// Create Domain 
        /// </summary>
        /// <param name="action">Create Domain  action</param>
        /// <returns>Create Domain  Response from the service</returns>
        /// <remarks>
        /// The CreateDomain operation creates a new domain. The domain name must be unique
        /// among the domains associated with the Access Key ID provided in the request. The CreateDomain
        /// operation may take 10 or more seconds to complete.
        ///   
        /// </remarks>
        public CreateDomainResponse CreateDomain(CreateDomain action) 
        {
            return Invoke<CreateDomainResponse>("CreateDomainResponse.xml");
        }
        
        /// <summary>
        /// List Domains 
        /// </summary>
        /// <param name="action">List Domains  action</param>
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
            return Invoke<ListDomainsResponse>("ListDomainsResponse.xml");
        }
        
        /// <summary>
        /// Delete Domain 
        /// </summary>
        /// <param name="action">Delete Domain  action</param>
        /// <returns>Delete Domain  Response from the service</returns>
        /// <remarks>
        /// The DeleteDomain operation deletes a domain. Any items (and their attributes) in the domain
        /// are deleted as well. The DeleteDomain operation may take 10 or more seconds to complete.
        ///   
        /// </remarks>
        public DeleteDomainResponse DeleteDomain(DeleteDomain action) 
        {
            return Invoke<DeleteDomainResponse>("DeleteDomainResponse.xml");
        }
        
        /// <summary>
        /// Put Attributes 
        /// </summary>
        /// <param name="action">Put Attributes  action</param>
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
            return Invoke<PutAttributesResponse>("PutAttributesResponse.xml");
        }
        
        /// <summary>
        /// Get Attributes 
        /// </summary>
        /// <param name="action">Get Attributes  action</param>
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
            return Invoke<GetAttributesResponse>("GetAttributesResponse.xml");
        }
        
        /// <summary>
        /// Delete Attributes 
        /// </summary>
        /// <param name="action">Delete Attributes  action</param>
        /// <returns>Delete Attributes  Response from the service</returns>
        /// <remarks>
        /// Deletes one or more attributes associated with the item. If all attributes of an item are deleted, the item is
        /// deleted.
        ///   
        /// </remarks>
        public DeleteAttributesResponse DeleteAttributes(DeleteAttributes action) 
        {
            return Invoke<DeleteAttributesResponse>("DeleteAttributesResponse.xml");
        }
        
        /// <summary>
        /// Query 
        /// </summary>
        /// <param name="action">Query  action</param>
        /// <returns>Query  Response from the service</returns>
        /// <remarks>
        /// The Query operation returns a set of ItemNames that match the query expression. Query operations that
        /// run longer than 5 seconds will likely time-out and return a time-out error response.
        /// A Query with no QueryExpression matches all items in the domain.
        ///   
        /// </remarks>
        public QueryResponse Query(Query action) 
        {
            return Invoke<QueryResponse>("QueryResponse.xml");
        }

        // Private API ------------------------------------------------------------//

        private T Invoke<T>(String xmlResource)
        {
            XmlSerializer serlizer = new XmlSerializer(typeof(T));
            Stream xmlStream = Assembly.GetAssembly(this.GetType()).GetManifestResourceStream(xmlResource);
            return (T)serlizer.Deserialize(xmlStream);
        }
    }
}