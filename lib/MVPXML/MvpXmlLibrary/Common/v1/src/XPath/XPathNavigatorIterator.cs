#region using

using System;
using System.Collections;
using System.Xml.XPath;

#endregion using 

namespace Mvp.Xml.Common.XPath
{
	/// <summary>
	/// An <see cref="XPathNodeIterator"/> that allows 
	/// arbitrary addition of the <see cref="XPathNavigator"/> 
	/// nodes that belong to the set.
	/// </summary>
	public class XPathNavigatorIterator : XPathNodeIterator
	{
		#region Fields & Ctors

		private ArrayList _navigators = new ArrayList();
		private int _position = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathNavigatorIterator"/>.
		/// </summary>
		public XPathNavigatorIterator()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathNavigatorIterator"/>, 
		/// using the received navigator as the initial item in the set. 
		/// </summary>
		public XPathNavigatorIterator(XPathNavigator navigator)
		{
			_navigators.Add(navigator);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathNavigatorIterator"/>, 
		/// using the received navigators as the initial set. 
		/// </summary>
		public XPathNavigatorIterator(XPathNavigator[] navigators)
		{
			_navigators.AddRange(navigators);
		}

		/// <summary>
		/// For cloning purposes.
		/// </summary>
		private XPathNavigatorIterator(ArrayList navigators)
		{
			_navigators = (ArrayList) navigators.Clone();
		} 

		#endregion Fields & Ctors

		#region Public Methods

		/// <summary>
		/// Adds a <see cref="XPathNavigator"/> to the set.
		/// </summary>
		/// <param name="navigator">The navigator to add. It's cloned automatically.</param>
		public void Add(XPathNavigator navigator)
		{
			if (_position != -1) 
				throw new InvalidOperationException(
					SR.XPathNavigatorIterator_CantAddAfterMove);

			_navigators.Add(navigator.Clone());
		}

		/// <summary>
		/// Adds a <see cref="XPathNodeIterator"/> containing a set of navigators to add.
		/// </summary>
		/// <param name="iterator">The set of navigators to add. Each one is cloned automatically.</param>
		public void Add(XPathNodeIterator iterator)
		{
			if (_position != -1) 
				throw new InvalidOperationException(
					SR.XPathNavigatorIterator_CantAddAfterMove);

			while (iterator.MoveNext())
			{
				_navigators.Add(iterator.Current.Clone());
			}
		}

		/// <summary>
		/// Resets the iterator.
		/// </summary>
		public void Reset()
		{
			_position = -1;
		}

		#endregion Public Methods

		#region XPathNodeIterator Overrides

		/// <summary>
		/// See <see cref="XPathNodeIterator.Clone"/>.
		/// </summary>
		public override XPathNodeIterator Clone()
		{
			return new XPathNavigatorIterator(_navigators);
		}

		/// <summary>
		/// See <see cref="XPathNodeIterator.Count"/>.
		/// </summary>
		public override int Count
		{
			get { return _navigators.Count; }
		}

		/// <summary>
		/// See <see cref="XPathNodeIterator.Current"/>.
		/// </summary>
		public override XPathNavigator Current
		{
			get { return _position == -1 ? null : (XPathNavigator) _navigators[_position]; }
		}

		/// <summary>
		/// See <see cref="XPathNodeIterator.CurrentPosition"/>.
		/// </summary>
		public override int CurrentPosition
		{
			get { return _position + 1; }
		}

		/// <summary>
		/// See <see cref="XPathNodeIterator.MoveNext"/>.
		/// </summary>
		public override bool MoveNext()
		{
			if (_navigators.Count == 0) return false;

            _position++;
			if (_position < _navigators.Count) return true;

			return false;
		}

		#endregion XPathNodeIterator Overrides
	}
}
