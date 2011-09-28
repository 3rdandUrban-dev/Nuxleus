using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVMF.SOA.Common;

namespace ScopeTest
{
    public class ExceptionHandlerScope : HandlerBase
    {
        protected override void Call()
        {
            try
            {
                base.Call();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("!!! ArgumentNullException handled. Message: '{0}', ParamName: '{1}'.", ex.Message, ex.ParamName);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("!!! ArgumentException handled. Message: '{0}', ParamName: '{1}'.", ex.Message, ex.ParamName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("!!! Exception handled. Message: '{0}'.", ex.Message);
            }
        }
    }
}
