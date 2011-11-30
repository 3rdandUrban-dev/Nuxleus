using System.Collections.Generic;
using Nuxleus.Asynchronous;

namespace Nuxleus
{
    public static partial class ObjectExtensionMethods
    {
        public static IEnumerable<T> AttributesFromType<T>(this object o)
        {
            foreach (T type in o.GetType().GetCustomAttributes(typeof(T), false))
            {
                yield return type;
            }
        }
    }
}
