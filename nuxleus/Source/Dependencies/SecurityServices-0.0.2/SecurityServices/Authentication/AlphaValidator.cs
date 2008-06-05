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
    /// A validator that checks for the presence of letters in the password.
    /// </summary>
    public class AlphaValidator : CharacterValidator {
        /// <summary>
        /// Initializes a new AlphaValidator instance.
        /// </summary>
        public AlphaValidator() : this(1) { }
        /// <summary>
        /// Initializes a new AlphaValidator instance.
        /// </summary>
        /// <param name="required">The required number of matches.</param>
        /// <exception cref="ArgumentException"><i>required</i> has an invalid value.</exception>
        public AlphaValidator(int required) : this(required, true) { }
        /// <summary>
        /// Initializes a new AlphaValidator instance.
        /// </summary>
        /// <param name="required">The required number of matches.</param>
        /// <param name="allowMore"><b>true</b> if more than the specified number of characters are allowed, <b>false</b> otherwise.</param>
        /// <exception cref="ArgumentException"><i>required</i> has an invalid value.</exception>
        public AlphaValidator(int required, bool allowMore) : base(required, allowMore) { }
        /// <summary>
        /// Inspects one character of the password.
        /// </summary>
        /// <param name="input">The character to inspect.</param>
        public override void InspectChar(char input) {
            if (char.IsLetter(input))
                ValidatedChars++;
        }
    }
}
