#region usage

using System.Xml.XPath;

#endregion

namespace Mvp.Xml.Common.XPath
{
	/// <summary>
	/// Enables a class to return an XPathNavigator from the current context or position.
	/// </summary>
	public interface IHasXPathNavigator
	{
        /// <summary>
        /// Returns the XPathNavigator for the current context or position.
        /// </summary>        
		XPathNavigator GetNavigator();
	}
}
