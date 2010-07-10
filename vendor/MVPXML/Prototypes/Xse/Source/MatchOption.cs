using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Xml.Core
{
	public enum MatchMode
	{
		RootElement,
		RootEndElement,
		Element,
		EndElement,
		Default = Element,
	}
}
