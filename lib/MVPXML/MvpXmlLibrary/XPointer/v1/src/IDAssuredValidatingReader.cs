#region using

using System.Xml;

#endregion

namespace Mvp.Xml.XPointer 
{

    /// <summary>
    /// Auxillary XmlReader that always reports dummy DOCTYPE. This is done
    /// to turn on support for id() function in XML Schema defined XML documents.
    /// See http://www.tkachenko.com/blog/archives/000060.html.
    /// </summary>
    internal class IdAssuredValidatingReader : XmlValidatingReader 
    {
        #region private members

        private bool _exposeDummyDoctype;
        private bool _isInProlog = true;    

        #endregion

        #region constructors
    
        /// <summary>
        /// Constructs <c>IdAssuredValidatingReader</c> on top of another reader.
        /// </summary>
        /// <param name="r"></param>   
        public IdAssuredValidatingReader(XmlReader r) : base (r) {}

        #endregion

        #region XmlValidatingReader overrides
    
        /// <summary>See <see cref="XmlValidatingReader.NodeType"/>.</summary>
        public override XmlNodeType NodeType 
        {
            get 
            { 
                return _exposeDummyDoctype ?
                    XmlNodeType.DocumentType :
                    base.NodeType; 
            }            
        }
    
        /// <summary>See <see cref="XmlValidatingReader.MoveToNextAttribute"/>.</summary>
        public override bool MoveToNextAttribute() 
        {
            return _exposeDummyDoctype?
                false :
                base.MoveToNextAttribute();
        }
    
        /// <summary>See <see cref="XmlValidatingReader.Read"/>.</summary>
        public override bool Read() 
        {
            if (_isInProlog) 
            {
                if (!_exposeDummyDoctype) 
                {
                    //We are looking for the very first element
                    bool baseRead = base.Read();
                    if (base.NodeType == XmlNodeType.Element) 
                    {
                        _exposeDummyDoctype = true;  
                        return true;
                    } 
                    else if (base.NodeType == XmlNodeType.DocumentType) 
                    {
                        //Document has own DOCTYPE, switch back to normal flow
                        _exposeDummyDoctype = false;
                        _isInProlog = false;
                        return true;
                    }
                    else 
                    {
                        return baseRead;
                    }
                } 
                else 
                {
                    //Done, switch back to normal flow
                    _exposeDummyDoctype = false;
                    _isInProlog = false;
                    return true;
                }
            } 
            else
                return base.Read();
        }

        #endregion
    }
}