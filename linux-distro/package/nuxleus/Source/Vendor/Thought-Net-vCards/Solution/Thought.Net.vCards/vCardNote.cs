
namespace Thought.Net.vCards
{
    /// <summary>
    ///     A note or comment in a vCard.
    /// </summary>
    public class vCardNote
    {

        private string language;
        private string text;


        /// <summary>
        ///     Initializes a new vCard note.
        /// </summary>
        public vCardNote()
        {
            this.language = string.Empty;
            this.text = string.Empty;
        }


        /// <summary>
        ///     Initializes a new vCard note with the specified text.
        /// </summary>
        /// <param name="text">
        ///     The text of the note or comment.
        /// </param>
        public vCardNote(string text)
        {
            this.language = string.Empty;
            this.text = text == null ? string.Empty : text;
        }


        /// <summary>
        ///     The language of the note.
        /// </summary>
        public string Language
        {
            get
            {
                return this.language;
            }
            set
            {

                if (value == null)
                {
                    this.language = string.Empty;
                }
                else
                {
                    this.language = value;
                }
            }
        }

        /// <summary>
        ///     The text of the note.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (value == null)
                {
                    this.text = string.Empty;
                }
                else
                {
                    this.text = value;
                }
            }
        }


        /// <summary>
        ///     Returns the text of the note.
        /// </summary>
        public override string ToString()
        {
            return this.text;
        }

    }
}
