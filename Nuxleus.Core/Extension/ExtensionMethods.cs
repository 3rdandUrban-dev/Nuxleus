using System.Collections.Generic;
using Nuxleus.Asynchronous;

namespace Nuxleus.Core
{
    public static class ExtensionMethods
    {
        public static IEnumerable<IAsync> AsIAsync(this ITask operation)
        {
            return operation.InvokeAsync();
        }
        public static IEnumerable<T> AttributesFromType<T>(this object o)
        {
            foreach (T type in o.GetType().GetCustomAttributes(typeof(T), false))
            {
                yield return type;
            }
        }
    }
}
