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
using System.Runtime.InteropServices;
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Authentication {
    /// <summary>
    /// Validates a password according to a set of validation rules.
    /// </summary>
    public class PasswordValidator {
        /// <summary>
        /// Initializes a new PasswordValidator instance.
        /// </summary>
        /// <remarks>By default, passwords must contain at least one alphanumerical character, one symbol and must have a length of at lease 8 characters.</remarks>
        public PasswordValidator() {
            m_Validators = new IValidator[] { new NumericValidator(), new AlphaValidator(), new SymbolValidator(), new LengthValidator() };
        }
        /// <summary>
        /// Initializes a new PasswordValidator instance.
        /// </summary>
        /// <param name="validators">The set of validators to use.</param>
        /// <exception cref="ArgumentNullException"><i>validators</i> is a null reference.</exception>
        /// <exception cref="ArgumentException">At least one of the validators in <i>validators</i> is a null reference.</exception>
        public PasswordValidator(IValidator[] validators) {
            if (validators == null)
                throw new ArgumentNullException("validators", ResourceController.GetString("Error_ParamNull"));
            for (int i = 0; i < validators.Length; i++) {
                if (validators[i] == null)
                    throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "validators");
            }
            m_Validators = validators;
        }
        /// <summary>
        /// Validates a password.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns><b>true</b> if the password could be validated, <b>false</b> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The password is a null reference.</exception>
        public bool Validate(string password) {
            if (password == null)
                throw new ArgumentNullException("password", ResourceController.GetString("Error_ParamNull"));
            return InternalValidate(new CharEnumerator(password));
        }
        /// <summary>
        /// Validates a password.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns><b>true</b> if the password could be validated, <b>false</b> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The password is a null reference.</exception>
        public bool Validate(SecureString password) {
            if (password == null)
                throw new ArgumentNullException("password", ResourceController.GetString("Error_ParamNull"));
            return InternalValidate(new CharEnumerator(password));
        }
        /// <summary>
        /// Gives an indication of how strong the password is.
        /// </summary>
        /// <param name="password">The password to test.</param>
        /// <returns>A double precision floating point number betzeen 0.0 and 1.0 that gives an indication of how strong the password is,
        /// considering the validation requirements. A value of 1.0 corresponds with a validated password.</returns>
        public double PasswordStrength(string password) {
            Validate(password);
            return m_LastStrength;
        }
        /// <summary>
        /// Gives an indication of how strong the password is.
        /// </summary>
        /// <param name="password">The password to test.</param>
        /// <returns>A double precision floating point number betzeen 0.0 and 1.0 that gives an indication of how strong the password is,
        /// considering the validation requirements. A value of 1.0 corresponds with a validated password.</returns>
        public double PasswordStrength(SecureString password) {
            Validate(password);
            return m_LastStrength;
        }

        private bool InternalValidate(CharEnumerator password) {
            while(password.MoveNext()){
                for (int i = 0; i < m_Validators.Length; i++) {
                    m_Validators[i].InspectChar(password.Current);
                }
            }
            bool result = true;
            m_LastStrength = 0;
            for (int i = 0; i < m_Validators.Length; i++) {
                result &= m_Validators[i].IsValidated();
                m_LastStrength += m_Validators[i].PasswordStrength;
                m_Validators[i].Reset();
            }
            m_LastStrength /= m_Validators.Length;
            return result;
        }

        private IValidator[] m_Validators;
        private double m_LastStrength;
    }
}