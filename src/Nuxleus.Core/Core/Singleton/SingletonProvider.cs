using System;

namespace Nuxleus.Core
{
	public class SingletonProvider <T> where T: class, new()
	{
		
		
		SingletonProvider ()
		{
			
		}

		public static T Instance {
			get { return SingletonCreator.instance; }
		}

		class SingletonCreator
		{
			static Dictionary<string,string > contentTypes = new Dictionary<string, string> ();
			
			static SingletonCreator (params KeyValuePair<string,string> contentTypeParams)
			{
				

				foreach (KeyValuePair<string,string> contentType in contentTypeParams) {
					contentTypes [contentType.Key] = contentType.Value;
				
				}
			}
			
			internal static readonly T instance = new T (contentTypes);
		}
	}
	

}

