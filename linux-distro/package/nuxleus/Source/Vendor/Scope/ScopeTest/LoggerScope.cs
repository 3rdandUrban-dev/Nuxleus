using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVMF.SOA.Common;

namespace ScopeTest
{
    public class LoggerScope : HandlerBase
    {
        public LoggerScope() : base() { }

        public string Message { get; set; }

        protected override void OnEnterScope(EventArgs e)
        {
            base.OnEnterScope(e);
            if (!string.IsNullOrEmpty(Message))
            {
                Console.WriteLine(">LOG --- Entering: {0} ---", Message);
            }
        }

        protected override void OnLeaveScope(EventArgs e)
        {
            base.OnLeaveScope(e);
            if (!string.IsNullOrEmpty(Message))
            {
                Console.WriteLine(">LOG --- Leaving: {0} ---", Message);
                Message = null;
            }
        }

        protected override void OnScopeException(ScopeExceptionEventArg e)
        {
            base.OnScopeException(e);
            if (!string.IsNullOrEmpty(Message))
            {
                Console.WriteLine(">LOG --- Exception caught: {0}, Type: {1}  ---", Message, e.Exception.GetType().Name);
            }
        }
    }
}
