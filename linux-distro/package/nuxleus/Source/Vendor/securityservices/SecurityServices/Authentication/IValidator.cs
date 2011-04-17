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

namespace Org.Mentalis.SecurityServices.Authentication {
    /// <summary>
    /// Defines methods that inspect a password, character by character, to determine whether it is a secure password or not.
    /// </summary>
    public interface IValidator {
        /// <summary>
        /// Inspects one character of the password.
        /// </summary>
        /// <param name="input">The character to inspect.</param>
        void InspectChar(char input);
        /// <summary>
        /// Returns a boolean that indicates whether the inspected password can be considered secure by this validator.
        /// </summary>
        /// <returns>A boolean that indicates whether the inspected password can be considered secure by this validator.</returns>
        bool IsValidated();
        /// <summary>
        /// Resets the internal status of the validator.
        /// </summary>
        void Reset();
        /// <summary>
        /// Gives an indication of how 'far' the password is from being validated.
        /// </summary>
        /// <value>A double precision floating point number betzeen 0.0 and 1.0 that gives an indication of how strong the password is,
        /// considering the validation requirements. A value of 1.0 corresponds with a validated password.</value>
        double PasswordStrength {
            get;
        }
    }
}
