using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Mvp.Xml.Core
{
	public class PredicateActionXmlProcessor : XmlProcessor
	{
		bool initialized = false;
		Predicate<XmlReader> predicate;
		Action<XmlReader> action;

		public PredicateActionXmlProcessor(Predicate<XmlReader> predicate, Action<XmlReader> action)
		{
			InitializeProcessor(predicate, action);
		}

		protected PredicateActionXmlProcessor()
		{
		}

		protected void InitializeProcessor(Predicate<XmlReader> predicate, Action<XmlReader> action)
		{
			Guard.ArgumentNotNull(predicate, "predicate");
			Guard.ArgumentNotNull(action, "action");

			this.predicate = predicate;
			this.action = action;
			initialized = true;
		}

		protected Predicate<XmlReader> Predicate
		{
			get { return predicate; }
		}

		protected Action<XmlReader> Action
		{
			get { return action; }
		}

		public override XmlReader Process(XmlReader reader)
		{
			if (!initialized)
				throw new InvalidOperationException(Properties.Resources.ProcessorNotInitialized);

			if (predicate(reader))
			{
				action(reader);
			}

			return reader;
		}
	}
}
