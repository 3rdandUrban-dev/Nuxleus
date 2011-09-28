using System.Xml;

/// <remarks />
public class BaseReader : XmlReader 
{
	#region XmlReader overrides

	#region Properties
    
	/// <summary>
	/// See <see cref="XmlReader.AttributeCount"/>
	/// </summary>
	public override int AttributeCount 
	{
		get 
		{
			return 0;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.BaseURI"/>
	/// </summary>
	public override string BaseURI 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.Depth"/>
	/// </summary>
	public override int Depth 
	{
		get 
		{
			return 0;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.EOF"/>
	/// </summary>
	public override bool EOF 
	{
		get 
		{
			return false;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.HasValue"/>
	/// </summary>
	public override bool HasValue 
	{
		get 
		{
			return false;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.IsDefault"/>
	/// </summary>
	public override bool IsDefault 
	{
		get 
		{
			return false;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.IsEmptyElement"/>
	/// </summary>
	public override bool IsEmptyElement 
	{
		get 
		{
			return false;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.Item"/>
	/// </summary>
	public override string this[string name, string namespaceURI] 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.Item"/>
	/// </summary>
	public override string this[string name] 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.Item"/>
	/// </summary>
	public override string this[int i] 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.LocalName"/>
	/// </summary>
	public override string LocalName 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.Name"/>
	/// </summary>
	public override string Name 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.NamespaceURI"/>
	/// </summary>
	public override string NamespaceURI 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.NameTable"/>
	/// </summary>
	public override XmlNameTable NameTable 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.NodeType"/>
	/// </summary>
	public override XmlNodeType NodeType 
	{
		get 
		{
			return 0;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.Prefix"/>
	/// </summary>
	public override string Prefix 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.QuoteChar"/>
	/// </summary>
	public override char QuoteChar 
	{
		get 
		{
			return '0';
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.ReadState"/>
	/// </summary>
	public override ReadState ReadState 
	{
		get 
		{
			return 0;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.Value"/>
	/// </summary>
	public override string Value 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.XmlLang"/>
	/// </summary>
	public override string XmlLang 
	{
		get 
		{
			return null;
		}
	}
    
	/// <summary>
	/// See <see cref="XmlReader.XmlSpace"/>
	/// </summary>
	public override XmlSpace XmlSpace 
	{
		get 
		{
			return 0;
		}
	}
    
	#endregion Properties

	#region Methods

	/// <summary>
	/// See <see cref="XmlReader.Close"/>
	/// </summary>
	public override void Close() 
	{
	}
    
	/// <summary>
	/// See <see cref="XmlReader.GetAttribute"/>
	/// </summary>
	public override string GetAttribute(string name, string namespaceURI) 
	{
		return null;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.GetAttribute"/>
	/// </summary>
	public override string GetAttribute(string name) 
	{
		return null;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.GetAttribute"/>
	/// </summary>
	public override string GetAttribute(int i) 
	{
		return null;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.LookupNamespace"/>
	/// </summary>
	public override string LookupNamespace(string prefix) 
	{
		return null;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.MoveToAttribute"/>
	/// </summary>
	public override bool MoveToAttribute(string name, string ns) 
	{
		return false;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.MoveToAttribute"/>
	/// </summary>
	public override bool MoveToAttribute(string name) 
	{
		return false;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.MoveToAttribute"/>
	/// </summary>
	public override void MoveToAttribute(int i) 
	{
	}
    
	/// <summary>
	/// See <see cref="XmlReader.MoveToElement"/>
	/// </summary>
	public override bool MoveToElement() 
	{
		return false;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.MoveToFirstAttribute"/>
	/// </summary>
	public override bool MoveToFirstAttribute() 
	{
		return false;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.MoveToNextAttribute"/>
	/// </summary>
	public override bool MoveToNextAttribute() 
	{
		return false;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.Read"/>
	/// </summary>
	public override bool Read() 
	{
		return false;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.ReadAttributeValue"/>
	/// </summary>
	public override bool ReadAttributeValue() 
	{
		return false;
	}
    
	/// <summary>
	/// See <see cref="XmlReader.ResolveEntity"/>
	/// </summary>
	public override void ResolveEntity() 
	{
	}

	#endregion Methods

	#endregion XmlReader overrides
}
