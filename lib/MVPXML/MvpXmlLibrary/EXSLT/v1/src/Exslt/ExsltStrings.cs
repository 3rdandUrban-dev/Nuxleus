#region Using
using System;
using System.Xml.XPath; 
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
#endregion

namespace Mvp.Xml.Exslt 
{
	/// <summary>
	/// Implements the functions in the http://exslt.org/strings namespace 
	/// </summary>
	public class ExsltStrings {
        private readonly static char[] hexdigit = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};


        #region tokenize()

		/// <summary>
		/// Implements the following function 
		///		node-set tokenize(string, string)
		/// </summary>
		/// <param name="str"></param>
		/// <param name="delimiters"></param>		
		/// <param name="flags"></param>
		/// <returns>This function breaks the input string into a sequence of strings, 
		/// treating any character in the list of delimiters as a separator. 
		/// The separators themselves are not returned. 
		/// The tokens are returned as a set of 'token' elements.</returns>
		public XPathNodeIterator tokenize(string str, string delimiters)
        {					

			XmlDocument doc = new XmlDocument(); 
			doc.LoadXml("<tokens/>"); 
			
            if (delimiters == String.Empty) 
            {
                foreach (char c in str) 
                {
                    XmlElement elem = doc.CreateElement("token"); 
                    elem.InnerText = c.ToString(); 
                    doc.DocumentElement.AppendChild(elem); 
                }
            } 
            else 
            { 
                foreach(string token in str.Split(delimiters.ToCharArray()))
                {
			
                    XmlElement elem = doc.CreateElement("token"); 
                    elem.InnerText  = token; 
                    doc.DocumentElement.AppendChild(elem); 
                }
            }

			return doc.CreateNavigator().Select("//token"); 
		}


		/// <summary>
		/// Implements the following function 
		///		node-set tokenize(string)
		/// </summary>
		/// <param name="str"></param>
		/// <param name="delimiters"></param>		
		/// <param name="flags"></param>
		/// <returns>This function breaks the input string into a sequence of strings, 
		/// using the whitespace characters as a delimiter. 
		/// The separators themselves are not returned. 
		/// The tokens are returned as a set of 'token' elements.</returns>
		public XPathNodeIterator tokenize(string str){					

			Regex regex = new Regex("\\s+"); 

			XmlDocument doc = new XmlDocument(); 
			doc.LoadXml("<tokens/>"); 			
			 
			foreach(string token in regex.Split(str)){
			
				XmlElement elem = doc.CreateElement("token"); 
				elem.InnerText  = token; 
				doc.DocumentElement.AppendChild(elem); 
			}

			return doc.CreateNavigator().Select("//token"); 
		}

        #endregion

        #region replace()
/// <summary>
/// Implements the following function 
///		string replace(string, string, string) 
/// </summary>
/// <param name="str"></param>
/// <param name="oldValue"></param>
/// <param name="newValue"></param>
/// <returns></returns>
/// <remarks>This function has completely diffeerent semantics from the EXSLT function. 
/// The description of the EXSLT function is confusing and furthermore no one has implemented
/// the described semantics which implies that others find the method problematic. Instead
/// this function is straightforward, it replaces all occurrences of oldValue with 
/// newValue</remarks>
		public string replace(string str, string oldValue, string newValue)
        {

			return str.Replace(oldValue, newValue); 
		}

        #endregion

        #region padding()
/// <summary>
/// Implements the following function 
///		string padding(number)
/// </summary>
/// <param name="number"></param>
/// <returns></returns>
		public string padding(int number) {
			string s = String.Empty; 

			if(number < 0){
				return s;
			}else{
				return s.PadLeft(number); 
			}
		}

		/// <summary>
		/// Implements the following function 
		///		string padding(number, string)
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
    public string padding(int number, string s)
    {								 

      if(number < 0 || s == string.Empty)
      {
        return String.Empty;
      }       
      else 
      {
        StringBuilder sb = new StringBuilder(s); 
				
        while(sb.Length < number)
        { 				
          sb.Append(s); 
        }

        if(sb.Length > number)
        {
          return sb.Remove(number, sb.Length - number).ToString(); 	
        }
        else
        {
          return sb.ToString(); 
        }
      }
    }
        #endregion

        #region split()

		/// <summary>
		/// Implements the following function 
		///		node-set split(string)
		/// </summary>
		/// <param name="str"></param>
		/// <remarks>This function breaks the input string into a sequence of strings, 
		/// using the space character as a delimiter. 
		/// The space character itself is never returned not even when there are 
		/// adjacent space characters. 
		/// </remarks>
		/// <returns>The tokens are returned as a set of 'token' elements</returns>
		public XPathNodeIterator split(string str)
        {
		
			XmlDocument doc = new XmlDocument(); 
			doc.LoadXml("<tokens/>"); 
			

			foreach(string match in str.Split(new char[]{' '})){
			
				if(!match.Equals(String.Empty)){
					XmlElement elem = doc.CreateElement("token"); 
					elem.InnerText  = match; 
					doc.DocumentElement.AppendChild(elem); 
				}
			}

			return doc.CreateNavigator().Select("//token"); 

		}

		/// <summary>
		/// Implements the following function 
		///		node-set split(string, string)
		/// </summary>
		/// <param name="str"></param>
		/// <param name="delimiter"></param>
		/// <remarks>This function breaks the input string into a sequence of strings, 
		/// using the space character as a delimiter. 
		/// The space character itself is never returned not even when there are 
		/// adjacent space characters. 
		/// </remarks>
		/// <returns>The tokens are returned as a set of 'token' elements</returns>
		public XPathNodeIterator split(string str, string delimiter){
		
			XmlDocument doc = new XmlDocument(); 
			doc.LoadXml("<tokens/>"); 
			

			if(delimiter.Equals(String.Empty)){
				foreach(char match in str){
			
					XmlElement elem = doc.CreateElement("token"); 
					elem.InnerText  = match.ToString(); 
					doc.DocumentElement.AppendChild(elem); 
				}
			}else{
				//since there is no String.Split(string) method we use the Regex class 
				//and escape special characters. 
				//. $ ^ { [ ( | ) * + ? \
				delimiter = delimiter.Replace("\\","\\\\").Replace("$", "\\$").Replace("^", "\\^");
				delimiter = delimiter.Replace("{", "\\{").Replace("[", "\\[").Replace("(", "\\("); 
				delimiter = delimiter.Replace("*","\\*").Replace(")", "\\)").Replace("|", "\\|");
				delimiter = delimiter.Replace("+", @"\+").Replace("?", "\\?").Replace(".", "\\."); 
				 				
				Regex regex = new Regex(delimiter); 


				foreach(string match in regex.Split(str)){
			
					if((!match.Equals(String.Empty)) && (!match.Equals(delimiter))){
						XmlElement elem = doc.CreateElement("token"); 
						elem.InnerText  = match; 
						doc.DocumentElement.AppendChild(elem); 
					}
				}
			}

			return doc.CreateNavigator().Select("//token"); 
		}

        #endregion

        #region concat()
	/// <summary>
	/// Implements the following function 
	///		string concat(node-set)
	/// </summary>
	/// <param name="nodeset"></param>
	/// <returns></returns>
		public string concat(XPathNodeIterator nodeset)
        {

			StringBuilder sb = new StringBuilder(); 
		
			while(nodeset.MoveNext()){
				sb.Append(nodeset.Current.Value); 
			}

			return sb.ToString(); 
		}
        #endregion
		
        #region align()
		/// <summary>
		/// Implements the following function
		///     string str:align(string, string, string)
		/// </summary>
		/// <param name="str">String to align</param>
		/// <param name="padding">String, within which to align</param>
		/// <param name="alignment">left/right/center</param>
		/// <returns>Aligned string.</returns>
		public string align(string str, string padding, string alignment) 
        {
		    if (str.Length > padding.Length)
		        return str.Substring(0, padding.Length);
		    else if (str.Length == padding.Length)
		        return str;
		    else {
		        switch (alignment) {
		            case "right":
		                return padding.Substring(0, padding.Length - str.Length) + str;
		            case "center":		                
		                int space = (padding.Length - str.Length)/2;
                        return padding.Substring(0, space) + str + 
                                padding.Substring(str.Length + space);		                    		                
		            default:
		                //Align to left by default
		                return str + padding.Substring(str.Length);
		        }
		    }		        
		}		       
        
        /// <summary>
        /// Implements the following function
        ///     string str:align(string, string)
        /// </summary>
        /// <param name="str">String to align</param>
        /// <param name="padding">String, within which to align</param>
        /// <returns>Aligned to left string.</returns>
        public string align(string str, string padding) {        
            return align(str, padding, "left");
        }		        

        #endregion

        #region encode-uri()
        /// <summary>
        /// This wrapper method will be renamed during custom build
        /// to provide conformant EXSLT function name.
        /// </summary>    
        public string encodeUri_RENAME_ME(string str, bool encodeReserved)
        {
            return encodeUri(str, encodeReserved);
        }

        /// <summary>
        /// This wrapper method will be renamed during custom build
        /// to provide conformant EXSLT function name.
        /// </summary>    
        public string encodeUri_RENAME_ME(string str, bool encodeReserved, string encoding)
        {
            return encodeUri(str, encodeReserved, encoding);
        }

        /// <summary>
        /// Implements the following function
        ///      string str:encode-uri(string, string)
        /// </summary>
        /// <param name="str">String to encode</param>
        /// <param name="encodeReserved">If true, will encode even the [RFC 2396] 
        /// and [RFC 2732] "reserved characters".</param>
        /// <returns>The encoded string</returns>
        public string encodeUri(string str, bool encodeReserved)
        {
            return encodeUriImpl(str, encodeReserved, Encoding.UTF8);
        }

        /// <summary>
        /// Implements the following function
        ///      string str:encode-uri(string, string, string)
        /// </summary>
        /// <param name="str">String to encode</param>
        /// <param name="encodeReserved">If true, will encode even the 
        /// [RFC 2396] and [RFC 2732] "reserved characters"</param>
        /// <param name="encoding">A character encoding to use</param>
        /// <returns>The encoded string</returns>
        public string encodeUri(string str, bool encodeReserved, string encoding)
        {            
            Encoding enc = null;
            try 
            {
                enc = Encoding.GetEncoding(encoding);
            } 
            catch 
            {
                //Not supported encoding, return empty string
                return String.Empty;
            }
            return encodeUriImpl(str, encodeReserved, enc);
        }
        
        /// <summary>
        /// Implements the following function
        ///      string str:encode-uri(string, string, string)
        /// </summary>
        /// <param name="str">String to encode</param>
        /// <param name="encodeReserved">If true, will encode even the 
        /// [RFC 2396] and [RFC 2732] "reserved characters"</param>
        /// <param name="encoding">A character encoding to use</param>
        /// <returns>The encoded string</returns>
        private string encodeUriImpl(string str, bool encodeReserved, Encoding enc)
        {
            if (str == string.Empty)
                return str;            
            StringBuilder res = new StringBuilder(str.Length);
            char[] chars = str.ToCharArray();
            if (encodeReserved) 
            {
                for (int i=0; i<chars.Length; i++) 
                {
                    char c = chars[i];
                    if ((c>='a' && c<='z') || (c>='A' && c<='Z') || (c>='0' && c<='9'))
                        res.Append(c);
                    else 
                    {
                        switch (c) 
                        {
                            case '-':case '_':case '.':case '!':case '~':case '*':
                            case '\'':case '(':case ')':
                                res.Append(c);
                                break;
                            case '%':
                                if (i<chars.Length-2 && IsHexDigit(chars[i+1]) && IsHexDigit(chars[i+2])) 
                                    res.Append(c);
                                else
                                    EncodeChar(res, enc, chars, i);
                                break;
                            default:
                                EncodeChar(res, enc, chars, i);
                                break;
                        }
                    }
                }
            } 
            else 
            {
                for (int i=0; i<chars.Length; i++) 
                {
                    char c = chars[i];
                    if ((c>='a' && c<='z') || (c>='A' && c<='Z') || (c>='0' && c<='9'))
                        res.Append(c);
                    else 
                    {
                        switch (c) 
                        {
                            case '-':case '_':case '.':case '!':case '~':case '*':
                            case '\'':case '(':case ')':case ';':case '/':case '?':                                                                                    
                            case ':':case '@':case '&':case '=':case '+':case '$':
                            case ',':case '[':case ']':
                                res.Append(c);
                                break;
                            case '%':
                                if (i<chars.Length-2 && IsHexDigit(chars[i+1]) && IsHexDigit(chars[i+2])) 
                                    res.Append(c);
                                else
                                    EncodeChar(res, enc, chars, i);
                                break;
                            default:
                                EncodeChar(res, enc, chars, i);
                                break;
                        }
                    }
                }

            }            
            return res.ToString();
        }

        private void EncodeChar(StringBuilder res, Encoding enc, char[] str, int index) 
        {
            foreach (byte b in enc.GetBytes(str, index, 1))
                res.AppendFormat("%{0}{1}", hexdigit[b>>4], hexdigit[b&15]);
        }
        
        private bool IsHexDigit(char c) 
        {
            return (c>='0' && c<='9') || (c>='a' && c<='f') || (c>='A' && c<='F');
        }
        #endregion

        #region decode-uri()
        /// <summary>
        /// This wrapper method will be renamed during custom build
        /// to provide conformant EXSLT function name.
        /// </summary>    
        public string decodeUri_RENAME_ME(string str)
        {
            return decodeUri(str);
        }

        /// <summary>
        /// This wrapper method will be renamed during custom build
        /// to provide conformant EXSLT function name.
        /// </summary>    
        public string decodeUri_RENAME_ME(string str, string encoding)
        {
            return decodeUri(str, encoding);
        }

        /// <summary>
        /// Implements the following function
        ///      string str:decode-uri(string)
        /// </summary>
        /// <param name="str">String to decode</param>        
        /// <returns>The decoded string</returns>
        public string decodeUri(string str)
        {
            return decodeUriImpl(str, Encoding.UTF8);
        }

        /// <summary>
        /// Implements the following function
        ///      string str:decode-uri(string, string)
        /// </summary>
        /// <param name="str">String to decode</param>        
        /// <param name="encoding">A character encoding to use</param>
        /// <returns>The decoded string</returns>
        public string decodeUri(string str, string encoding)
        { 
            if (encoding == String.Empty)
                return String.Empty;
            Encoding enc = null;
            try 
            {
                enc = Encoding.GetEncoding(encoding);
            } 
            catch 
            {
                //Not supported encoding, return empty string
                return String.Empty;            
            }            
            return decodeUriImpl(str, enc);
        }

        /// <summary>
        /// Implementation for 
        ///   string str:decode-uri(string, string)
        /// </summary>
        /// <param name="str">String to decode</param>        
        /// <param name="encoding">A character encoding to use</param>
        /// <returns>The decoded string</returns>
        private string decodeUriImpl(string str, Encoding enc)
        {
            if (str == string.Empty)
                return str;            
            return HttpUtility.UrlDecode(str, enc);            
        }
        #endregion
	}
}