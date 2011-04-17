
/* =======================================================================
 * vCard Library for .NET
 * Copyright (c) 2007 David Pinch; http://www.thoughtproject.com.
 * See LICENSE.TXT for licensing information.
 * ======================================================================= */

using System;
using System.Collections.ObjectModel;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     A collection of <see cref="vCardEmailAddress"/> objects.
    /// </summary>
    /// <seealso cref="vCardEmailAddress"/>
    /// <seealso cref="vCardEmailType"/>
    public class vCardEmailAddressCollection : Collection<vCardEmailAddress>
    {

        public vCardEmailAddress GetFirstChoice(vCardEmailAddressType emailType)
        {

            vCardEmailAddress firstNonPreferred = null;

            foreach (vCardEmailAddress email in this)
            {

                if ((email.EmailType & emailType) == emailType)
                {

                    if (firstNonPreferred == null)
                        firstNonPreferred = email;

                    if (email.IsPreferred)
                        return email;
                }

            }

            return firstNonPreferred;

        }

    }
}
