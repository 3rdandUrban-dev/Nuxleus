// At present time, all that follows is a direct copy/paste of
// the code presented by Brian McNamara as part of the entry 
// @ http://lorgonblog.spaces.live.com/Blog/cns!701679AD17B6D310!1146.entry
// Just getting my feet wet with the style he's presented here.
using System;

namespace Nuxleus.Core.Transaction
{
    public class Transactional
    {
        /// Run 'stepWithEffect' followed by continuation 'k', but if the continuation later fails with
        /// an exception, undo the original effect by running 'compensatingEffect'.
        /// Note: if 'compensatingEffect' throws, it masks the original exception.
        public static B Do<A, B>(Func<A> stepWithEffect, Action<A> compensatingEffect, Func<A, B> k) {
            var stepCompleted = false;
            var allOk = false;
            A a = default(A);
            try {
                a = stepWithEffect();
                stepCompleted = true;
                var r = k(a);
                allOk = true;
                return r;
            }
            finally {
                if (!allOk && stepCompleted) {
                    compensatingEffect(a);
                }
            }
        }
        // if A == void, this is the overload
        public static B Do<B>(Action stepWithEffect, Action compensatingEffect, Func<B> k) {
            return Do(
                () => {
                    stepWithEffect();
                    return 0;
                },
                (int dummy) => {
                    compensatingEffect();
                },
                (int dummy) => {
                    return k();
                });
        }
        // if A & B are both void, this is the overload
        public static void Do(Action stepWithEffect, Action compensatingEffect, Action k) {
            Do(
                () => {
                    stepWithEffect();
                    return 0;
                },
                (int dummy) => {
                    compensatingEffect();
                },
                (int dummy) => {
                    k();
                    return 0;
                });
        }
    }
}
