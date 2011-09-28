/*
 *   Mentalis.org Security Services for .NET 2.0
 * 
 *     Copyright © 2006, The Mentalis.org Team
 *     All rights reserved.
 *     http://www.mentalis.org/
 *
 *
 *   Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions
 *   are met:
 *
 *     - Redistributions of source code must retain the above copyright
 *        notice, this list of conditions and the following disclaimer. 
 *
 *     - Neither the name of the Mentalis.org Team, nor the names of its contributors
 *        may be used to endorse or promote products derived from this
 *        software without specific prior written permission. 
 *
 *   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 *   FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 *   THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 *   INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 *   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 *   HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 *   STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 *   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 *   OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Permissions;
using Org.Mentalis.SecurityServices.Smartcard;

namespace Org.Mentalis.SecurityServices.Permissions {
    /// <summary>
    /// Controls the ability to communicate with a smartcard.
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Constructor)]
    public sealed class SmartcardPermissionAttribute : CodeAccessSecurityAttribute {
        /// <summary>
        /// Initializes a new SmartcardPermissionAttribute instance.
        /// </summary>
        /// <param name="action">One of the SecurityAction values.</param>
        public SmartcardPermissionAttribute(SecurityAction action)
            : base(action) {
        }
        /// <summary>
        /// Initializes a new SmartcardPermissionAttribute instance.
        /// </summary>
        /// <param name="action">One of the SecurityAction values.</param>
        /// <param name="options">One of the SmartcardConnectOptions values.</param>
        public SmartcardPermissionAttribute(SecurityAction action, SmartcardConnectOption options)
            : base(action) {
            m_Options = options;
            UpdateUnrestricted();
        }
        /// <summary>
        /// Initializes a new SmartcardPermissionAttribute instance.
        /// </summary>
        /// <param name="action">One of the SecurityAction values.</param>
        /// <param name="allowed">A list of ATRs to allow.</param>
        public SmartcardPermissionAttribute(SecurityAction action, Atr[] allowed)
            : base(action) {
            m_Options = SmartcardConnectOption.AllowedAtrs;
            m_AllowedAtrs = allowed;
            UpdateUnrestricted();
        }
        /// <summary>
        /// Gets or sets the list of allowed ATRs.
        /// </summary>
        /// <value>An array of ATR instances.</value>
        public Atr[] AllowedAtrs {
            get {
                return m_AllowedAtrs;
            }
            set {
                m_AllowedAtrs = value;
                m_Options = (m_AllowedAtrs == null || m_AllowedAtrs.Length == 0) ? SmartcardConnectOption.None : SmartcardConnectOption.AllowedAtrs;
                UpdateUnrestricted();
            }
        }
        private void UpdateUnrestricted() {
            this.Unrestricted = (m_Options == SmartcardConnectOption.Unrestricted);
        }
        /// <summary>
        /// Creates and returns a new SmartcardPermission.
        /// </summary>
        /// <returns>A SmartcardPermission that corresponds to this attribute.</returns>
        public override IPermission CreatePermission() {
            if (this.Unrestricted)
                m_Options = SmartcardConnectOption.Unrestricted;
            return new SmartcardPermission(m_Options, m_AllowedAtrs);
        }

        private SmartcardConnectOption m_Options;
        private Atr[] m_AllowedAtrs;
    }
}
