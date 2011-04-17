// Taken directly from http://www.codeproject.com/KB/cs/PoorMansInjector.aspx and modified where and as necessary
using System;

namespace Nuxleus.Core
{
    public struct Scope
    {
        public delegate Chain Chain(Chain code);

        public delegate void Block();

        public Scope(params Chain[] codes)
        {
            this.code = null;
            if (codes != null)
            {
                foreach (Chain code in codes)
                {
                    AddCode(code);
                }
            }
        }

        public Scope(params Scope[] others)
        {
            this.code = null;
            if (others != null)
            {
                foreach (Scope other in others)
                {
                    AddCode(other.code);
                }
            }
        }

        Chain code;

        private static Chain BlockToChain(Block code)
        {
            return c =>
            {
                code();
                return null;
            };
        }

        private void AddCode(Chain otherCode)
        {
            if (otherCode != null)
            {
                Chain thisCode = this.code;
                if (thisCode == null)
                {
                    this.code = otherCode;
                }
                else
                {
                    this.code = (c) => otherCode(thisCode(c));
                }
            }
        }

        public Block Begin
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (code != null)
                {
                    code(BlockToChain(value))(null);
                }
            }
        }

        public static Scope operator +(Scope scope1, Scope scope2)
        {
            Scope result = new Scope(scope1);
            result.AddCode(scope2.code);
            return result;
        }

        public static Scope operator +(Scope scope, Chain code)
        {
            Scope result = new Scope(scope);
            result.AddCode(code);
            return result;
        }

        public static implicit operator Scope(Chain code)
        {
            return new Scope(code);
        }

        public static implicit operator Chain(Scope scope)
        {
            return scope.code;
        }
    }
}
