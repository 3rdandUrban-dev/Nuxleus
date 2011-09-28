using System;

namespace Nuxleus.Core
{
<<<<<<< HEAD
	public class SingletonProvider <T> where T:new()
	{
		SingletonProvider ()
		{
=======
	public class SingletonProvider <T> where T: class, new()
	{
		
		
		SingletonProvider ()
		{
			
>>>>>>> 1c9f7b30d9b16dbd6968aad395edf261f5973dad
		}

		public static T Instance {
			get { return SingletonCreator.instance; }
		}

		class SingletonCreator
		{
<<<<<<< HEAD
			static SingletonCreator ()
			{
			}

			internal static readonly T instance = new T ();
=======
			static Dictionary<string,string > contentTypes = new Dictionary<string, string> ();
			
			static SingletonCreator (params KeyValuePair<string,string> contentTypeParams)
			{
				

				foreach (KeyValuePair<string,string> contentType in contentTypeParams) {
					contentTypes [contentType.Key] = contentType.Value;
				
				}
			}
			
			internal static readonly T instance = new T (contentTypes);
>>>>>>> 1c9f7b30d9b16dbd6968aad395edf261f5973dad
		}
	}
	

}

