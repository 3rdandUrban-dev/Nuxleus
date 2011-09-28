
using System;
using System.Collections.Generic;
using System.Text;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     Extended options for the <see cref="vCardStandardWriter"/> class.
    /// </summary>
    public enum vCardStandardWriterOptions
    {

        None = 0,

        /// <summary>
        ///     Indicates whether or not commas should be escaped in values.
        /// </summary>
        /// <remarks>
        ///     The vCard specification requires that commas be escaped
        ///     in values (e.g. a "," is translated to "\,").  However, Microsoft
        ///     Outlook(tm) does not properly decode these escaped commas.  This
        ///     option instruct the writer to ignored (not translate) embedded
        ///     commas for better compatibility with Outlook.
        /// </remarks>
        IgnoreCommas = 1

    }
}
