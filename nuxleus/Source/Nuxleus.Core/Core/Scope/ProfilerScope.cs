// Taken directly from http://www.codeproject.com/KB/cs/PoorMansInjector.aspx and modified where and as necessary
using System;

namespace Nuxleus.Core
{

    public class ProfilerScope : HandlerBase {

        DateTime start;

        public TimeSpan EllapsedTime {
            get;
            private set;
        }

        protected override void OnEnterScope(EventArgs e) {
            base.OnEnterScope(e);
            start = DateTime.Now;
            EllapsedTime = TimeSpan.Zero;
        }

        protected override void OnLeaveScope(EventArgs e) {
            EllapsedTime = DateTime.Now - start;
            base.OnLeaveScope(e);
        }
    }
}
