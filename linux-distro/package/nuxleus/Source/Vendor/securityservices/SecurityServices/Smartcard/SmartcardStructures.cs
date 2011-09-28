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
using Org.Mentalis.SecurityServices.Win32;

namespace Org.Mentalis.SecurityServices.Smartcard {
    /// <summary>
    /// Scope of the resource manager context.
    /// </summary>
    public enum DatabaseScope : int {
        /// <summary>Database operations are performed within the domain of the user.</summary>
        User = NativeMethods.SCARD_SCOPE_USER,
        /// <summary>Database operations are performed within the domain of the system. The calling application must have appropriate access permissions for any database actions.</summary>
        System = NativeMethods.SCARD_SCOPE_SYSTEM
    }
    /// <summary>
    /// The acceptable protocols for the smartcard connection.
    /// </summary>
    [Flags]
    public enum SmartcardProtocols : int {
        /// <summary>No protocol negotiation will be performed by the drivers</summary>
        Raw = NativeMethods.SCARD_PROTOCOL_RAW,
        /// <summary>T=0 is an acceptable protocol.</summary>
        T0 = NativeMethods.SCARD_PROTOCOL_T0,
        /// <summary>T=1 is an acceptable protocol.</summary>
        T1 = NativeMethods.SCARD_PROTOCOL_T1,
        /// <summary>SmartcardShare.Direct has been specified, so that no protocol negotiation has occurred.</summary>
        Undefined = NativeMethods.SCARD_PROTOCOL_UNDEFINED
    }
    /// <summary>
    /// Flag that indicates whether other applications may form connections to the card.
    /// </summary>
    public enum SmartcardShare : int {
        /// <summary>This application is not willing to share the card with other applications.</summary>
        Exclusive = NativeMethods.SCARD_SHARE_EXCLUSIVE,
        /// <summary>This application is willing to share the card with other applications.</summary>
        Shared = NativeMethods.SCARD_SHARE_SHARED,
        /// <summary>This application is allocating the reader for its private use, and will be controlling it directly. No other applications are allowed access to it.</summary>
        Direct = NativeMethods.SCARD_SHARE_DIRECT
    }
    /// <summary>
    /// Action to take on the card in the connected reader on close.
    /// </summary>
    public enum SmartcardDisposition : int {
        /// <summary>Do not do anything special.</summary>
        Leave = NativeMethods.SCARD_LEAVE_CARD,
        /// <summary>Reset the card.</summary>
        Reset = NativeMethods.SCARD_RESET_CARD,
        /// <summary>Power down the card.</summary>
        Unpower = NativeMethods.SCARD_UNPOWER_CARD,
        /// <summary>Eject the card.</summary>
        Eject = NativeMethods.SCARD_EJECT_CARD
    }
    /// <summary>
    /// Current state of the smart card in the reader.
    /// </summary>
    public enum SmartcardState : int {
        /// <summary>There is no card in the reader.</summary>
        Absent = NativeMethods.SCARD_ABSENT,
        /// <summary>There is a card in the reader, but it has not been moved into position for use.</summary>
        Present = NativeMethods.SCARD_PRESENT,
        /// <summary>There is a card in the reader in position for use. The card is not powered.</summary>
        Swallowed = NativeMethods.SCARD_SWALLOWED,
        /// <summary>Power is being provided to the card, but the reader driver is unaware of the mode of the card.</summary>
        Powered = NativeMethods.SCARD_POWERED,
        /// <summary>The card has been reset and is awaiting PTS negotiation.</summary>
        Negotiable = NativeMethods.SCARD_NEGOTIABLE,
        /// <summary>The card has been reset and specific communication protocols have been established.</summary>
        Specific = NativeMethods.SCARD_SPECIFIC
    }
}
