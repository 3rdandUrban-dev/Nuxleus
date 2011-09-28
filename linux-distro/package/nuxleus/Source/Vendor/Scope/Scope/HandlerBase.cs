using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VVMF.SOA.Common {
    public sealed class ScopeExceptionEventArg : EventArgs {
        public Exception Exception { get; internal set; }

        public bool Handled { get; set; }
    }

    public abstract class HandlerBase {
        public HandlerBase() {
            scope = new Scope(ScopeMethod);
            Enabled = true;
        }

        Scope scope;

        public Scope Scope {
            get { return scope; }
        }

        public static implicit operator Scope(HandlerBase handler) {
            if (handler == null)
                return new Scope();
            return handler.scope;
        }

        bool inFlag;

        public bool Enabled { get; set; }

        public event EventHandler EnterScope;

        public event EventHandler LeaveScope;

        public event EventHandler<ScopeExceptionEventArg> ScopeException;

        Scope.Chain ScopeMethod(Scope.Chain code) {
            if (inFlag || !Enabled)
                return (p) => { code(null); return null; };

            return (p) => {
                inFlag = true;
                try {
                    OnEnterScope(EventArgs.Empty);
                    this.code = code;
                    Call();
                } finally {
                    OnLeaveScope(EventArgs.Empty);
                    inFlag = false;
                }
                return null;
            };
        }

        Scope.Chain code;

        protected virtual void Call() {
            if (code != null) {
                try {
                    code(null);
                } catch (Exception ex) {
                    ScopeExceptionEventArg e = new ScopeExceptionEventArg { Exception = ex };
                    OnScopeException(e);
                    if (!e.Handled)
                        throw ex;
                } finally {
                    code = null;
                }
            }
        }

        protected virtual void OnEnterScope(EventArgs e) {
            EventHandler handler = EnterScope;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnLeaveScope(EventArgs e) {
            EventHandler handler = LeaveScope;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnScopeException(ScopeExceptionEventArg e) {
            EventHandler<ScopeExceptionEventArg> handler = ScopeException;
            if (handler != null)
                handler(this, e);
        }
    }
}
