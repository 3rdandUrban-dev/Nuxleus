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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Permissions;
using Org.Mentalis.SecurityServices.Smartcard;
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Permissions {
    /// <summary>
    /// Controls the ability to communicate with a smartcard.
    /// </summary>
    [SerializableAttribute()]
    public sealed class SmartcardPermission : CodeAccessPermission, IUnrestrictedPermission {
        /// <summary>
        /// Initializes a new SmartcardPermission instance.
        /// </summary>
        /// <param name="state">One of the SmartcardConnectOptions values.</param>
        /// <exception cref="ArgumentException">The value of <i>state</i> is invalid.</exception>
        public SmartcardPermission(SmartcardConnectOption state) {
            if (!Enum.IsDefined(typeof(SmartcardConnectOption), state))
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "state");
            Init(state, null);
        }
        /// <summary>
        /// Initializes a new SmartcardPermission instance.
        /// </summary>
        /// <param name="atr">An array of ATR objects that are allowed.</param>
        public SmartcardPermission(Atr[] atr) {
            Init(SmartcardConnectOption.AllowedAtrs, atr);
        }
        /// <summary>
        /// Initializes a new SmartcardPermission instance.
        /// </summary>
        /// <param name="state">One of the PermissionState values.</param>
        public SmartcardPermission(PermissionState state)
            : this(state == PermissionState.Unrestricted ? SmartcardConnectOption.Unrestricted : SmartcardConnectOption.None) {
        }

        internal SmartcardPermission(SmartcardConnectOption state, Atr[] atr) {
            Init(state, atr);
        }
        private SmartcardPermission(SmartcardConnectOption options, LinkedList<Atr> allowed) {
            m_AllowedAtrs = new LinkedList<Atr>();
            foreach (Atr a in allowed) {
                m_AllowedAtrs.AddLast((Atr)a.Clone());
            }
            m_Options = options;
        }
        private void Init(SmartcardConnectOption options, Atr[] atr) {
            m_AllowedAtrs = new LinkedList<Atr>();
            if (atr != null)
                AddAllowedAtrs(atr);
            m_Options = options;
        }
        /// <summary>
        /// Adds an ATR to the allowed list.
        /// </summary>
        /// <param name="atr">The ATR to add.</param>
        /// <exception cref="ArgumentNullException"><i>atr</i> is invalid.</exception>
        public void AddAllowedAtr(Atr atr) {
            if (atr == null)
                throw new ArgumentNullException("atr", ResourceController.GetString("Error_ParamNull"));
            AddAllowedAtrs(new Atr[] { atr });
        }
        /// <summary>
        /// Adds an array of ATRs to the allowed list.
        /// </summary>
        /// <param name="atr">The array of ATRs to add.</param>
        /// <exception cref="ArgumentNullException"><i>atr</i> is invalid.</exception>
        public void AddAllowedAtrs(Atr[] atr) {
            if (atr == null)
                throw new ArgumentNullException("atr", ResourceController.GetString("Error_ParamNull"));
            foreach (Atr a in atr) {
                if (a != null && a.IsValid() && !m_AllowedAtrs.Contains(a)) {
                    m_AllowedAtrs.AddLast((Atr)a.Clone());
                }
            }
            if (m_Options == SmartcardConnectOption.None && m_AllowedAtrs.Count > 0)
                m_Options = SmartcardConnectOption.AllowedAtrs;
        }
        /// <summary>
        /// Gets a list of the allowed ATRs.
        /// </summary>
        /// <returns>An array containing the allowed ATRs.</returns>
        public Atr[] GetAllowedAtrs() {
            Atr[] ret = new Atr[m_AllowedAtrs.Count];
            int i = 0;
            foreach (Atr a in m_AllowedAtrs) {
                ret[i++] = (Atr)a.Clone();
            }
            return ret;
        }
        /// <summary>
        /// Returns a value indicating whether the current permission is unrestricted.
        /// </summary>
        /// <returns><b>true</b> if the current permission is unrestricted; otherwise, <b>false</b>.</returns>
        public bool IsUnrestricted() {
            return m_Options == SmartcardConnectOption.Unrestricted;
        }
        /// <summary>
        /// Retruns a value indicating whether the current permission is empty.
        /// </summary>
        /// <returns><b>true</b> if the current permission is empty; otherwise, <b>false</b>.</returns>
        public bool IsEmpty() {
            return m_Options == SmartcardConnectOption.None;
        }
        /// <summary>
        /// Creates and returns an identical copy of the current permission.
        /// </summary>
        /// <returns>A copy of the current permission. </returns>
        public override IPermission Copy() {
            return new SmartcardPermission(m_Options,  m_AllowedAtrs);
        }
        /// <summary>
        /// Creates and returns a permission that is the intersection of the current permission and the specified permission.
        /// </summary>
        /// <param name="target">A permission to intersect with the current permission. It must be the same type as the current permission. </param>
        /// <returns>A new permission that represents the intersection of the current permission and the specified permission. This new permission is a null reference if the intersection is empty.</returns>
        /// <exception cref="ArgumentException"><i>target</i> is not of the same type as the current permission.</exception>
        public override IPermission Intersect(IPermission target) {
            if (target == null) {
                return null;
            }
            SmartcardPermission permission = target as SmartcardPermission;
            if (permission == null) {
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"));
            }
            if (this.IsUnrestricted() || permission.IsEmpty()) {
                return permission.Copy();
            }
            if (permission.IsUnrestricted() || this.IsEmpty()) {
                return this.Copy();
            }
            // if we get here, both permissions are set to 'AllowedAtrs'
            SmartcardPermission retPerm = new SmartcardPermission(SmartcardConnectOption.AllowedAtrs);
            foreach (Atr a in this.m_AllowedAtrs) {
                if (permission.m_AllowedAtrs.Contains(a))
                    retPerm.m_AllowedAtrs.AddLast(a);
            }
            if (retPerm.m_AllowedAtrs.Count == 0) {
                retPerm.m_Options = SmartcardConnectOption.None;
            }
            return retPerm;
        }
        /// <summary>
        /// Determines whether the current permission is a subset of the specified permission.
        /// </summary>
        /// <param name="target">A permission that is to be tested for the subset relationship. This permission must be the same type as the current permission.</param>
        /// <returns><b>true</b> if the current permission is a subset of the specified permission; otherwise, <b>false</b>.</returns>
        /// <exception cref="ArgumentException"><i>target</i> is not of the same type as the current permission.</exception>
        public override bool IsSubsetOf(IPermission target) {
            if (target == null) {
                return this.IsEmpty();
            }
            SmartcardPermission permission = target as SmartcardPermission;
            if (permission == null) {
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"));
            }
            if (permission.IsUnrestricted() || this.IsEmpty()) {
                return true;
            }
            if (this.IsUnrestricted() || permission.IsEmpty()) {
                return false;
            }
            // if we get here, both permissions are set to 'AllowedAtrs'
            foreach (Atr a in this.m_AllowedAtrs) {
                if (!permission.m_AllowedAtrs.Contains(a)) {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Creates a permission that is the union of the current permission and the specified permission.
        /// </summary>
        /// <param name="target">A permission to combine with the current permission. It must be the same type as the current permission.</param>
        /// <returns>A new permission that represents the union of the current permission and the specified permission.</returns>
        /// <exception cref="ArgumentException"><i>target</i> is not of the same type as the current permission.</exception>
        public override IPermission Union(IPermission target) {
            if (target == null)
                return this.Copy();
            SmartcardPermission permission = target as SmartcardPermission;
            if (permission == null)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"));
            SmartcardPermission retPerm = new SmartcardPermission((this.IsUnrestricted() || permission.IsUnrestricted()) ? SmartcardConnectOption.Unrestricted : this.m_Options, this.m_AllowedAtrs);
            if (!retPerm.IsUnrestricted()) {
                foreach (Atr a in permission.m_AllowedAtrs) {
                    if (!retPerm.m_AllowedAtrs.Contains(a)) {
                        retPerm.m_AllowedAtrs.AddLast((Atr)a.Clone());
                    }
                }
                if ((permission.IsEmpty() && this.IsEmpty()))
                    retPerm.m_Options = SmartcardConnectOption.None;
                else
                    retPerm.m_Options = SmartcardConnectOption.AllowedAtrs;
            }
            return retPerm;
        }
        /// <summary>
        /// Creates an XML encoding of the permission and its current state.
        /// </summary>
        /// <returns>An XML encoding of the permission, including any state information.</returns>
        public override SecurityElement ToXml() {
            // Use the SecurityElement class to encode the permission to XML.
            SecurityElement esd = new SecurityElement("IPermission");
            Type t = this.GetType();
            esd.AddAttribute("class", t.FullName + ", " + t.Module.Assembly.FullName.Replace('"', '\''));
            esd.AddAttribute("version", "1");
            esd.AddAttribute("State", ((int)m_Options).ToString());
            foreach (Atr a in m_AllowedAtrs) {
                esd.AddChild(AtrToXml(a));
            }
            return esd;
        }
        private static SecurityElement AtrToXml(Atr a) {
            SecurityElement ret = new SecurityElement("ATR");
            ret.AddAttribute("Value", Convert.ToBase64String(a.GetValue()));
            ret.AddAttribute("Mask", Convert.ToBase64String(a.GetMask()));
            return ret;
        }
        /// <summary>
        /// Reconstructs a permission with a specified state from an XML encoding.
        /// </summary>
        /// <param name="e">The XML encoding used to reconstruct the permission.</param>
        /// <exception cref="ArgumentNullException"><i>elem</i> is a null reference.</exception>
        /// <exception cref="ArgumentException"><i>elem</i> does not contain valid XML for this permission.</exception>
        public override void FromXml(SecurityElement e) {
            if (e == null)
                throw new ArgumentNullException("elem", ResourceController.GetString("Error_ParamNull"));
            if (!e.Tag.Equals("IPermission") && !e.Tag.Equals("Permission"))
                throw new ArgumentException(ResourceController.GetString("Error_NotAPermissionElement"));
            string version = e.Attribute("version");
            if ((version != null) && !version.Equals("1"))
                throw new ArgumentException(ResourceController.GetString("Error_InvalidXMLBadVersion"));
            m_AllowedAtrs.Clear();
            string state = e.Attribute("State");
            int stateValue = 0;
            if (state == null || !int.TryParse(state, out stateValue)) {
                m_Options = SmartcardConnectOption.None;
            } else {
                if (!Enum.IsDefined(typeof(SmartcardConnectOption), stateValue))
                    throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"));
                m_Options = (SmartcardConnectOption)stateValue;
            }
            if (m_Options == SmartcardConnectOption.AllowedAtrs) {
                foreach(SecurityElement se in e.Children) {
                    if (se != null && se.Tag.Equals("ATR"))
                           m_AllowedAtrs.AddLast(  new Atr(Convert.FromBase64String(se.Attribute("Value")), Convert.FromBase64String(se.Attribute("Mask"))));
                }
            }
        }

        [NonSerializedAttribute]
        private LinkedList<Atr> m_AllowedAtrs;
        private SmartcardConnectOption m_Options;
    }
}
