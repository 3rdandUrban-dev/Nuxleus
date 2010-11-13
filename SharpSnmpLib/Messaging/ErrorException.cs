// Error exception type.
// Copyright (C) 2008-2010 Malcolm Crowe, Lex Li, and other contributors.
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/4/23
 * Time: 19:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Net;
#if (!SILVERLIGHT)
using System.Runtime.Serialization;
using System.Security.Permissions; 
#endif

namespace Lextm.SharpSnmpLib.Messaging
{
    /// <summary>
    /// Error exception of #SNMP. Raised when an error message is received.
    /// </summary>
    [Serializable]
    public sealed class ErrorException : OperationException
    {
        /// <summary>
        /// Message body.
        /// </summary>
        public ISnmpMessage Body { get; private set; }

        /// <summary>
        /// Creates a <see cref="ErrorException"/> instance.
        /// </summary>
        public ErrorException()
        {
        }
        
        /// <summary>
        /// Creates a <see cref="ErrorException"/> instance with a specific <see cref="string"/>.
        /// </summary>
        /// <param name="message">Message</param>
        public ErrorException(string message) : base(message)
        {
        }
        
        /// <summary>
        /// Creates a <see cref="ErrorException"/> instance with a specific <see cref="string"/> and an <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public ErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }
#if (!SILVERLIGHT && !CF) 
        private ErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            
            Body = (ResponseMessage)info.GetValue("Body", typeof(ResponseMessage));
        }
        
        /// <summary>
        /// Gets object data.
        /// </summary>
        /// <param name="info">Info</param>
        /// <param name="context">Context</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Body", Body);
        }
#endif
        /// <summary>
        /// Details on error.
        /// </summary>
        protected override string Details
        {
            get
            {
                var pdu = Body.Pdu();
                int index = pdu.ErrorIndex.ToInt32();
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}. {1}. Index: {2}. Errored Object ID: {3}",
                    Message,
                    pdu.ErrorStatus.ToErrorCode(),
                    index.ToString(CultureInfo.InvariantCulture),
                    index == 0 ? null : pdu.Variables[index - 1].Id);
            }
        }
         
        /// <summary>
        /// Creates a <see cref="ErrorException"/>.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="agent">Agent address.</param>
        /// <param name="body">Error message body.</param>
            /// <returns></returns>
        public static ErrorException Create(string message, IPAddress agent, ISnmpMessage body)
        {
            ErrorException ex = new ErrorException(message) { Agent = agent, Body = body };
            return ex;
        }
    }
}