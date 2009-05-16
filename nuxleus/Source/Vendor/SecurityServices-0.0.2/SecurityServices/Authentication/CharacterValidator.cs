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
using System.Globalization;
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Authentication {
    /// <summary>
    /// Defines a base class for the validators that check for the presence of a specific type of character.
    /// </summary>
    public abstract class CharacterValidator : IValidator {
        /// <summary>
        /// Initializes a new CharacterValidator instance.
        /// </summary>
        protected CharacterValidator()
            : this(1, true) {
        }
        /// <summary>
        /// Initializes a new CharacterValidator instance.
        /// </summary>
        /// <param name="required">The required number of matches.</param>
        /// <param name="allowMore"><b>true</b> if more than the specified number of characters are allowed, <b>false</b> otherwise.</param>
        /// <exception cref="ArgumentException"><i>required</i> has an invalid value.</exception>
        protected CharacterValidator(int required, bool allowMore) {
            if (required < 1)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "required");
            m_MinRequired = required;
            m_AllowMore = allowMore;
        }
        /// <summary>
        /// Returns a boolean that indicates whether the inspected password can be considered secure by this validator.
        /// </summary>
        /// <returns>A boolean that indicates whether the inspected password can be considered secure by this validator.</returns>
        public bool IsValidated() {
            return (m_ValidatedChars == m_MinRequired) || (m_AllowMore && m_ValidatedChars > m_MinRequired);
        }
        /// <summary>
        /// Resets the internal status of the validator.
        /// </summary>
        public void Reset() {
            m_ValidatedChars = 0;
        }
        /// <summary>
        /// Gives an indication of how 'far' the password is from being validated.
        /// </summary>
        /// <value>A double precision floating point number betzeen 0.0 and 1.0 that gives an indication of how strong the password is,
        /// considering the validation requirements. A value of 1.0 corresponds with a validated password.</value>
        public double PasswordStrength {
            get {
                if (m_AllowMore) {
                    if (m_ValidatedChars > m_MinRequired)
                        return 1.0;
                    else
                        return (double)m_ValidatedChars / (double)m_MinRequired;
                } else {
                    if (m_ValidatedChars == m_MinRequired)
                        return 1.0;
                    else
                        return Math.Abs(Math.Min(m_ValidatedChars - m_MinRequired, m_MinRequired)) / (double)m_MinRequired;
                }
            }
        }
        /// <summary>
        /// Inspects one character of the password.
        /// </summary>
        /// <param name="input">The character to inspect.</param>
        public abstract void InspectChar(char input);

        /// <summary>
        /// Holds the number of validated characters.
        /// </summary>
        /// <value>An integer that holds the number of validated characters.</value>
        protected int ValidatedChars {
            get {
                return m_ValidatedChars;
            }
            set {
                m_ValidatedChars = value;
            }
        }
        /// <summary>
        /// Holds the number of required valid characters.
        /// </summary>
        /// <value>An integer that holds the number of required valid characters.</value>
        protected int MinRequired {
            get {
                return m_MinRequired;
            }
            set {
                m_MinRequired = value;
            }
        }
        /// <summary>
        /// Specifies whether more valid characters than the specified number of minimum characters are allowed.
        /// </summary>
        /// <value><b>true</b> if more valid characters than the specified number of minimum characters are allowed, <b>false</b> otherwise.</value>
        protected bool AllowMore {
            get {
                return m_AllowMore;
            }
            set {
                m_AllowMore = value;
            }
        }

        private int m_ValidatedChars;
        private int m_MinRequired;
        private bool m_AllowMore;
    }
}