
using System;
using System.Collections.ObjectModel;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     A collection of <see cref="vCardNote"/> objects.
    /// </summary>
    public class vCardNoteCollection : Collection<vCardNote>
    {

        /// <summary>
        ///     Adds a new note to the collection.
        /// </summary>
        /// <param name="text">
        ///     The text of the note.
        /// </param>
        /// <returns>
        ///     The <see cref="vCardNote"/> object representing the note.
        /// </returns>
        public vCardNote Add(string text)
        {

            vCardNote note = new vCardNote(text);
            Add(note);
            return note;

        }

    }

}
