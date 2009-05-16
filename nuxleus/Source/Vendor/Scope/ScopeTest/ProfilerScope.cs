using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVMF.SOA.Common;

namespace ScopeTest
{
    public class ProfilerScope : HandlerBase
    {
        DateTime start;

        public TimeSpan EllapsedTime
        {
            get;
            private set;
        }

        protected override void OnEnterScope(EventArgs e)
        {
            base.OnEnterScope(e);
            start = DateTime.Now;
            EllapsedTime = TimeSpan.Zero;
        }

        protected override void OnLeaveScope(EventArgs e)
        {
            EllapsedTime = DateTime.Now - start;
            base.OnLeaveScope(e);
        }
    }
}
