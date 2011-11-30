using System.Collections.Generic;
using Nuxleus.Asynchronous;
using Nuxleus.Core;

namespace Nuxleus
{
    public static partial class AsyncExtensionMethods
    {
        public static IEnumerable<IAsync> AsIAsync(this ITask operation)
        {
            return operation.InvokeAsync();
        }
    }
}
