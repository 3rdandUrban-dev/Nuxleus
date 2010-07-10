using System;
using System.Collections.Generic;
using System.Text;

namespace XmlMvp.XPathmania.Internal
{
    internal class ErrorInfoLine
    {
        #region Constructor
        internal ErrorInfoLine(string desc, int seq, ErrorType type)
        {
            this.description = desc;
            this.sequence = seq.ToString();
            this.type = type;
            switch (this.type)
            {
                case ErrorType.Serious:
                    this.image = Internal.Resources.Serious;
                    break;
                case ErrorType.Warning:
                    this.image = Internal.Resources.Warning;
                    break;
                case ErrorType.Information:
                    this.image = Internal.Resources.Information;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Properties
        private string description;
        public string Description
        {
            get { return this.description; }
        }

        private string sequence;
        public string Sequence
        {
            get { return this.sequence; }
        }

        private System.Drawing.Bitmap image;
        public System.Drawing.Bitmap Image
        {
            get { return this.image; }
        }

        private ErrorType type;
        internal ErrorType Type
        {
            get { return this.type; }
        }

        #endregion

        internal  enum ErrorType
        {
            Serious,
            Warning,
            Information
        }
    }
}
