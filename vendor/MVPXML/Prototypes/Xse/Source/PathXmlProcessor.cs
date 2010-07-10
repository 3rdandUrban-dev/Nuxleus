using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Mvp.Xml.Core
{
	public class PathXmlProcessor : PredicateActionXmlProcessor
	{
		string xmlNsNamespace;
		XmlNamespaceManager nsManager;
		Stack<NamespaceScope> elementScopes = new Stack<NamespaceScope>();
		IList<XmlMatch> matchList;
		//XmlMatch attributeMatch;

		int currentPosition = 0;
		int currentDepth = -1;
		Stack<int> matchIndexes = new Stack<int>();

		#region Ctors

		public PathXmlProcessor(string pathExpression, Action<XmlReader> action, XmlNameTable readerNameTable)
			: this(PathExpressionParser.Parse(pathExpression, false), action, readerNameTable)
		{
		}

		public PathXmlProcessor(string pathExpression, Action<XmlReader> action, XmlNamespaceManager nsManager)
			: this(PathExpressionParser.Parse(pathExpression, false), action, nsManager)
		{
		}

		public PathXmlProcessor(string pathExpression, bool matchEndElement, Action<XmlReader> action, XmlNameTable readerNameTable)
			: this(PathExpressionParser.Parse(pathExpression, matchEndElement), action, readerNameTable)
		{
		}

		public PathXmlProcessor(string pathExpression, bool matchEndElement, Action<XmlReader> action, XmlNamespaceManager nsManager)
			: this(PathExpressionParser.Parse(pathExpression, matchEndElement), action, nsManager)
		{
		}

		public PathXmlProcessor(IList<XmlMatch> matchList, Action<XmlReader> action, XmlNamespaceManager nsManager)
		{
			Initialize(matchList, action, nsManager);
		}

		public PathXmlProcessor(IList<XmlMatch> matchList, Action<XmlReader> action, XmlNameTable readerNameTable)
		{
			Guard.ArgumentNotNull(readerNameTable, "readerNameTable");

			Initialize(matchList, action, new XmlNamespaceManager(readerNameTable));
		}

		#endregion

		private void Initialize(IList<XmlMatch> matchList, Action<XmlReader> action, XmlNamespaceManager nsManager)
		{
			Guard.ArgumentNotNull(matchList, "matchList");
			Guard.ArgumentNotNull(action, "action");
			Guard.ArgumentNotNull(nsManager, "nsManager");

			if (matchList.Count == 0) throw new ArgumentException(Properties.Resources.MatchListEmpty);

			this.nsManager = nsManager;
			this.matchList = matchList;
			// Remove the attribute match from the list, so that 
			// all depth logic works against elements only.
			//if (matchList[matchList.Count - 1] is AttributeMatch)
			//{
			//    attributeMatch = matchList[matchList.Count - 1];
			//    matchList.Remove(attributeMatch);
			//}

			xmlNsNamespace = nsManager.NameTable.Add("http://www.w3.org/2000/xmlns/");

			InitializeProcessor(MatchPredicate, action);

			matchIndexes.Push(-1);
		}

		public override XmlReader Process(XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				BeginNamespaceScope(reader);
			}
			if (reader.NodeType == XmlNodeType.EndElement)
			{
				// Let base implementation perform matching logic first, 
				// before removing namespace context.
				XmlReader chained = base.Process(reader);
				
				EndNamespaceScope(reader);

				// Short-cirtuit.
				return chained;
			}

			// Let base implementation perform matching logic.
			return base.Process(reader);
		}

		private void BeginNamespaceScope(XmlReader reader)
		{
			// Perform XML namespace/prefix scoping management.
			bool pushScope = false;
			for (bool go = reader.MoveToFirstAttribute(); go; go = reader.MoveToNextAttribute())
			{
				if (reader.NamespaceURI == xmlNsNamespace)
				{
					// It's an xmlns:foo="bar" declaration.
					if (reader.Prefix.Length > 0)
					{
						nsManager.AddNamespace(reader.LocalName, reader.Value);
					}
					else
					{
						// It's an xmlns="foo" declaration.
						nsManager.AddNamespace(String.Empty, reader.Value);
					}

					pushScope = true;
				}
			}

			// If it had attributes, we surely moved through all of them searching for namespaces
			if (reader.HasAttributes)
			{
				reader.MoveToElement();
			}
			if (pushScope)
			{
				nsManager.PushScope();
				elementScopes.Push(new NamespaceScope(reader.Depth, reader.LocalName, reader.NamespaceURI));
			}
		}

		private void EndNamespaceScope(XmlReader reader)
		{
			if (elementScopes.Count > 0)
			{
				NamespaceScope currentScope = elementScopes.Peek();
				if (currentScope.Depth == reader.Depth &&
					currentScope.LocalName == reader.LocalName &&
					currentScope.NamespaceURI == reader.NamespaceURI)
				{
					// TODO: not necessary?
					// Remove all namespaces in scope.
					foreach (KeyValuePair<string, string> nsDeclaration in nsManager.GetNamespacesInScope(XmlNamespaceScope.Local))
					{
						nsManager.RemoveNamespace(nsDeclaration.Key, nsDeclaration.Value);
					}

					nsManager.PopScope();
					elementScopes.Pop();
				}
			}
		}

		/// <summary>
		/// Logic implementing the Predicate{XmlReader} required by the base class.
		/// </summary>
		private bool MatchPredicate(XmlReader reader)
		{
			// Should we be this restrictive? After all, 
			// someone could create an XmlMatch that matches 
			// even text in a document. Corner case. Wait for  
			// community feedback. This is a performance 
			// improvement to avoid checking too much.
			// DONE: XmlMatch matches things with name 
			// and optional prefix. Content matching is 
			// a very corner case.
			// CHANGED: If user wants perf., they should 
			// instruct the XmlReader to skip whitespace 
			// comments, PIs, etc. Normal Read matches 
			// should be performed.
			//if (reader.NodeType != XmlNodeType.Element &&
			//    reader.NodeType != XmlNodeType.EndElement &&
			//    reader.NodeType != XmlNodeType.Attribute)
			//{
			//    return false;
			//}

			int lastDepth = currentDepth;
			int lastPosition = currentPosition;
			SaveCurrentState(reader);

			//if (currentDepth < lastDepth) DecrementOrPop();

			bool shouldPushLevelIfNoMatch = !ResetMatchListIfFull(lastDepth);
			bool match = Evaluate(matchIndexes.Peek() + 1, reader);
			bool result = false;

			if (match)
			{
				matchIndexes.Push(matchIndexes.Pop() + 1);
				if (matchIndexes.Peek() == matchList.Count - 1)
				{
					//if (attributeMatch != null)
					//{
					//    for (bool go = reader.MoveToFirstAttribute(); go; go = reader.MoveToNextAttribute())
					//    {
					//        result = attributeMatch.Matches(reader, nsManager);
					//        if (result) break;
					//    }
					//}
					//else
					//{
						result = true;
					//}
				}
			}
			else
			{
				if (shouldPushLevelIfNoMatch && currentDepth >= lastDepth &&
					reader.NodeType == XmlNodeType.Element)
				{
					matchIndexes.Push(-1);
				}
			}

			if (reader.NodeType == XmlNodeType.EndElement)
			{
				UndoResetMatchListIfFull(lastDepth);
				DecrementOrPop();
			}

			return result;
		}

		private void UndoResetMatchListIfFull(int lastDepth)
		{
			if (matchIndexes.Count > 1 &&
				currentDepth < lastDepth &&
				matchIndexes.Peek() == -1)
			{
				matchIndexes.Pop();
				if (matchIndexes.Peek() != matchList.Count - 1)
				{
					matchIndexes.Push(-1);
				}
			}
		}

		private bool ResetMatchListIfFull(int lastDepth)
		{
			if (currentDepth > lastDepth && matchIndexes.Peek() == matchList.Count - 1)
			{
				matchIndexes.Push(-1);
				return true;
			}

			return false;
		}

		private void DecrementOrPop()
		{
			if (matchIndexes.Peek() == -1)
			{
				matchIndexes.Pop();
			}
			else
			{
				matchIndexes.Push(matchIndexes.Pop() - 1);
			}
		}

		private void SaveCurrentState(XmlReader reader)
		{
			currentDepth = reader.Depth;

			IXmlLineInfo info = reader as IXmlLineInfo;
			if (info == null)
			{
				currentPosition = 0;
			}
			else
			{
				currentPosition = (info.LineNumber ^ info.LinePosition);
			}
		}

		private bool Evaluate(int index, XmlReader reader)
		{
			if (index >= matchList.Count)
				return false;

			XmlMatch match = matchList[index];
			bool matches = match.Matches(reader, nsManager);

			return matches;
		}

		private static void ThrowInvalidPath(string path)
		{
			throw new ArgumentException(String.Format(
				CultureInfo.CurrentCulture,
				Properties.Resources.InvalidPath,
				path));
		}

		struct NamespaceScope
		{
			public NamespaceScope(int depth, string localName, string namespaceURI)
			{
				this.Depth = depth;
				this.LocalName = localName;
				this.NamespaceURI = namespaceURI;
			}

			public int Depth;
			public string LocalName;
			public string NamespaceURI;
		}
	}
}
