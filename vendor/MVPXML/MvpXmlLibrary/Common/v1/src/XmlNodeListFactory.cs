#region using

using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

#endregion using

namespace Mvp.Xml.Common
{
	/// <summary>
	/// Constructs <see cref="XmlNodeList"/> instances from 
	/// <see cref="XPathNodeIterator"/> objects.
	/// </summary>
	/// <remarks>See http://weblogs.asp.net/cazzu/archive/2004/04/14/113479.aspx. 
	/// <para>Author: Daniel Cazzulino, kzu.net@gmail.com</para>
	/// </remarks>
	public sealed class XmlNodeListFactory
	{
		private XmlNodeListFactory() {}

		#region Public members

		/// <summary>
		/// Creates an instance of a <see cref="XmlNodeList"/> that allows 
		/// enumerating <see cref="XmlNode"/> elements in the iterator.
		/// </summary>
		/// <param name="iterator">The result of a previous node selection 
		/// through an <see cref="XPathNavigator"/> query.</param>
		/// <returns>An initialized list ready to be enumerated.</returns>
		/// <remarks>The underlying XML store used to issue the query must be 
		/// an object inheriting <see cref="XmlNode"/>, such as 
		/// <see cref="XmlDocument"/>.</remarks>
		public static XmlNodeList CreateNodeList(XPathNodeIterator iterator)
		{
			return new XmlNodeListIterator(iterator);
		}

		#endregion Public members

		#region XmlNodeListIterator

		private class XmlNodeListIterator: XmlNodeList
		{
			XPathNodeIterator _iterator;
			ArrayList _nodes = new ArrayList();

			public XmlNodeListIterator(XPathNodeIterator iterator)
			{
				_iterator = iterator.Clone();
			}

			public override IEnumerator GetEnumerator()
			{
				return new XmlNodeListEnumerator(this);
			}
			
			public override XmlNode Item(int index)
			{
				return this[index];
			}

			public override int Count 
			{ 
				get 
				{ 
					if (!_done) ReadToEnd();
					return _nodes.Count;
				} 
			}
			
			public override XmlNode this[int index]
			{ 
				get 
				{ 
					if (index >= _nodes.Count)
						ReadTo(index);
					// Compatible behavior with .NET
					if (index >= _nodes.Count || index < 0)
						return null;
					return (XmlNode) _nodes[index];
				} 
			}

			/// <summary>
			/// Reads the entire iterator.
			/// </summary>
			private void ReadToEnd()
			{
				while (_iterator.MoveNext())
				{
					IHasXmlNode node = _iterator.Current as IHasXmlNode;
					// Check IHasXmlNode interface.
					if (node == null)
						throw new ArgumentException(SR.XmlNodeListFactory_IHasXmlNodeMissing);
					_nodes.Add(node.GetNode());
				}
				_done = true;
			}

			/// <summary>
			/// Reads up to the specified index, or until the 
			/// iterator is consumed.
			/// </summary>
			private void ReadTo(int to)
			{
				while (_nodes.Count <= to)
				{
					if (_iterator.MoveNext())
					{
						IHasXmlNode node = _iterator.Current as IHasXmlNode;
						// Check IHasXmlNode interface.
						if (node == null)
							throw new ArgumentException(SR.XmlNodeListFactory_IHasXmlNodeMissing);
						_nodes.Add(node.GetNode());
					}
					else
					{
						_done = true;
						return;
					}
				}
			}

			/// <summary>
			/// Flags that the iterator has been consumed.
			/// </summary>
			private bool Done
			{
				get { return _done; }
			} bool _done;

            /// <summary>
            /// Current count of nodes in the iterator (read so far).
            /// </summary>
            private int CurrentPosition
            {
                get { return _nodes.Count; }
            }

			#region XmlNodeListEnumerator

			private class XmlNodeListEnumerator: IEnumerator
			{
				XmlNodeListIterator _iterator;
				int _position = -1;

				public XmlNodeListEnumerator(XmlNodeListIterator iterator)
				{
					_iterator = iterator;
				}

				#region IEnumerator Members

				void System.Collections.IEnumerator.Reset()
				{
					_position = -1;
				}


				bool System.Collections.IEnumerator.MoveNext()
				{
					_position++;
                    _iterator.ReadTo(_position);

					// If we reached the end and our index is still 
					// bigger, there're no more items.
                    if (_iterator.Done && _position >= _iterator.CurrentPosition)
						return false;

					return true;
				}

				object System.Collections.IEnumerator.Current
				{
					get
					{
						return _iterator[_position];
					}
				}

				#endregion
			}

			#endregion XmlNodeListEnumerator
		}

		#endregion XmlNodeListIterator
	}
}
