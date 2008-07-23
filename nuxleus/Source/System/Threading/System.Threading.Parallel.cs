/*
Copyright (c) 2003-2005, Intel Corporation
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this 
  list of conditions and the following disclaimer. 
* Redistributions in binary form must reproduce the above copyright notice, this 
  list of conditions and the following disclaimer in the documentation and/or 
  other materials provided with the distribution. 
* Neither the name of the Intel Corporation nor the names of its contributors may 
  be used to endorse or promote products derived from this software without 
  specific prior written permission. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
THE POSSIBILITY OF SUCH DAMAGE.
*/

/* 
This reference implementation is written for brevity and clarify, not absolute speed. 
It is hoped that experienced programmers can look at it and understand how to write 
something much faster and complicated.  

You  may want to experiment with the value of the constant ParallelEnvironment.DefaultNumberOfThreads.
See the XML comments on how to set it. 

The ParallelForLoop uses simple-minded "guided scheduling".  
It can be changed to "static scheduling" by "#define STATIC_SCHEDULING".

Arch D. Robison         Apr. 7, 2004.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Threading.Parallel {
    /// <summary>
    /// A delegate that processes iteration values.
    /// </summary>
    /// <remarks>
	/// This class will be replaced by the <see>System.Collections.Genereric.Action&lt;T&gt;</see>
    /// if that becomes part of the standard.
    /// </remarks>
	public delegate void Action<T>(T item);

	public sealed class ParallelEnvironment {
		private static int m_MaxThreads=RecommendedMaxThreads;

		/// <summary>
		/// The number of hardware threads available.
		/// </summary>
		/// <remarks>
		/// In a final implementation, this value would be obtained from the system somehow.
		/// If you are just trying the reference implementation, try setting this value to somewhere between
		/// n and 2n, where n is the number of physical threads.
		/// </remarks>
		/// <value></value>
		public static int RecommendedMaxThreads {
			get { return 4; }
		}

		/// <summary>
		/// The default maximum number of threads that a ParallelLoop will employ.
		/// </summary>
		/// <remarks>
		/// Set to 1 to force sequential execution of all parallel loop instances whose
		/// constructor do not specify the number of threads to use.
		/// </remarks>
		/// <value></value>
		public static int MaxThreads {
			get { return m_MaxThreads; }
			set {
				if( value<1 ) 
					throw new ArgumentException("MaxThreads must be positive");
				m_MaxThreads = value;
			}
		}
	}

    /// <summary>
    /// A parallel loop over iteration values of type T.
    /// </summary>
    /// <remarks>
	/// Abstract generic class <c></c>ParallelLoop&lt;T&gt; abstracts common behavior of the loop classes that iterate over values of type T.   
	/// Its derived classes differ in how the iteration space is defined.
    /// </remarks>
	public abstract class ParallelLoop<T> {
		/// <summary>
		/// Shorthand for <c>BeginRun(action)</c> followed by <c>EndRun()</c>.
		/// </summary>
		/// <param name="action">The delegate to be applied to each iteration value</param>
		public void Run(Action<T> action) {
			BeginRun(action);
			EndRun();
		}

        /// <summary>
        /// Possibly begin executing iterations, applying the action delegate to each iteration value.
        /// </summary>
        /// <remarks>
        /// Implementations, particularly on single-threaded hardware, are free to not start executing
        /// iterations until method <see cref="EndRun">EndRun</see> is called.
        /// <para>
        /// This method is not thread safe.  
		/// It should be called only once for a given instance of a <see cref="ParallelLoop&lt;T&gt;">ParallelLoop&lt;T&gt;</see>.
		/// </para>
		/// </remarks>
        /// <param name="action">delegate to apply to each iteration value</param>
		public abstract void BeginRun(Action<T> action);

        /// <summary>
        /// Waits until all iterations are finished (or cancelled).  
        /// If any of the iterations threw an exception, picks one of these exceptions and throws it.
        /// </summary>
        /// <remarks>
        /// This method is not thread safe.  
        /// It should be called  exactly once by the thread that called <see cref="BeginRun">BeginRun</see>. 
        /// </remarks>
		public void EndRun() {
			InternalEndRun();
		}

        /// <summary>
        /// Eventually cancel issuance of any further iterations. 
        /// </summary>
 		public abstract void Cancel();

        /// <summary>
        /// Make the current thread process some iterations of the loop.
        /// </summary>
        /// <remarks>
        /// Returns when thread runs out of work to do.
        /// </remarks>
		internal abstract void ProcessIterations(); 

        /// <summary>
        /// The callback to be applied to each iteration.
        /// </summary>
        /// <remarks>
        /// Always null until set by method <c>InternalBeginRun</c>, non-null afterwards. 
        ///</remarks>
		internal Action<T> ClientAction;

  	    /// <summary>
		/// Internal storage for property <see cref="MaxThreads">MaxThreads</see>.
		/// </summary>
		/// <remarks>
		/// The default value is implementation defined.
		/// </remarks>
		internal int m_NumberOfWorkers;

		/// <summary>
		/// Number of threads that are working on loop iterations.
		/// </summary>
		/// <remarks>
		/// The value includes the current thread once <see cref="EndRun">EndRun</see> has been called.
		/// </remarks>
		private int m_BusyCount;

		/// <summary>
		/// Event set by ThreadTask to communicate "all slave threads are done" back to the master.
		/// </summary>
		/// <remarks> 
		/// Always null if not using multiple threads.
		/// </remarks>
		private readonly AutoResetEvent m_Event;

		/// <summary>
		/// Always zero until loop starts executing.  
		/// Always one for loops that are executing sequentially.
		/// </summary>
		internal int m_ThreadLimit;

		internal ParallelLoop( int maxThreads ) {
			if (maxThreads<0)
				throw new ArgumentException("maxThreads must be non-negative");
			m_NumberOfWorkers = maxThreads==0 ? ParallelEnvironment.MaxThreads : maxThreads;
			if( m_NumberOfWorkers>1 )
				m_Event = new AutoResetEvent(false);
			LocalCallback = new WaitCallback( ThreadTask );
		}

        /// <summary>
        /// Called by derived classes to cancel.
        /// </summary>
		internal void InternalCancel() {
			m_ThreadLimit = 0;
		}

        /// <summary>
        /// Used to hold an exception thrown by an iteration.
        /// </summary>
        private Exception m_ThrownException;

		/// <summary>
		/// Task that each thread performs. 
		/// </summary> 
		/// <remarks>
		/// The clientCallback is applied to the given item, and then
		/// any other items are extracted as long as possible from GetOneIteration().
		/// </remarks>
		/// <param name="notUsed">dummy parameter for sake of being a <see cref="WaitCallback">WaitCallback</see></param>
		private void ThreadTask( object notUsed ) {
			try {
                try {
				    ProcessIterations();
                } catch( Exception e )  {
                    lock( LocalCallback ) 
                        m_ThrownException = e;
                     Cancel();
                }
			} finally {
				// Tell master that a thread finished.
				if (Threading.Interlocked.Decrement(ref m_BusyCount) == 0)  
					if( m_Event!=null )
						m_Event.Set();
			}
		}
    
        /// <summary>
		/// Callback that we feed to <see cref="System.Threading.ThreadPool">ThreadPool</see>.
        /// </summary>
		internal readonly WaitCallback LocalCallback; 

        /// <summary>
        /// Consider firing up one more task (or using self) as a worker.
        /// </summary>
        /// <param name="numberOfTasksForMaster">0 if another thread should be used, 1 if current thread (master) should be use.</param>
		internal void ConsiderRunningOneMoreThreadTask( int numberOfTasksForMaster ) {
			Debug.Assert( numberOfTasksForMaster==0 || numberOfTasksForMaster==1 );
			int k, oldBusyCount = m_BusyCount;
			do {
				k = oldBusyCount;
				int busyCountLimit = (m_ThreadLimit-1)+numberOfTasksForMaster;
				if (k>=busyCountLimit) {
					Debug.Assert( numberOfTasksForMaster==0 );
					return;
				}
				oldBusyCount = Interlocked.CompareExchange(ref m_BusyCount, k+1, k);
			} while (oldBusyCount != k);
			if (numberOfTasksForMaster != 0) {
				// Work on a task ourselves.
				ThreadTask(null);
				if( m_Event!=null )
					m_Event.WaitOne();
			} else {
				ThreadPool.QueueUserWorkItem(LocalCallback);
			}
		}

		/// <summary>
		/// Begin the loop iterations.
		/// </summary>
		/// <remarks>
		/// This method is not thread safe, because when called, 
		/// only the master thread is running.
		/// </remarks>
		/// <exception cref="ArgumentNullException">action is null</exception>
		/// <exception cref="InvalidOperationException">loop is already running</exception>
		/// <param name="action">delegate to apply to each item in the collection</param>
		/// <param name="clamp">upper bound on number of thread (including self) to use</param>
		internal void InternalBeginRun(Action<T> action, int clamp ) {
			if (action==null)
				throw new ArgumentNullException("action is null");

			ClientAction=action;
			if (m_ThreadLimit!=0)
				throw new InvalidOperationException("parallel loop already running");

			m_ThreadLimit=Math.Min(m_NumberOfWorkers, clamp);
			Debug.Assert( m_BusyCount==0, "there should not be any busy threads yet" );
			if (m_ThreadLimit==1) {
				// When not running in parallel, BeginRun must do the work.
				ConsiderRunningOneMoreThreadTask(/*numberOfTasksForMaster=*/1);
			} else {
				Thread.VolatileWrite(ref m_BusyCount, m_ThreadLimit-1);
				for (int k=1; k<m_ThreadLimit; ++k)
					ThreadPool.QueueUserWorkItem(LocalCallback);
			}
		}
		internal void InternalEndRun() {
			if( m_ThreadLimit>1 )
				ConsiderRunningOneMoreThreadTask(/*numberOfTasksForMaster=*/1);
			m_ThreadLimit = 0;
			if (m_ThrownException != null)
				throw m_ThrownException;
		}
	}
     
    /// <summary>
    /// A parallel while loop.
    /// </summary>
    /// <remarks>
	/// Generic class ParallelWhile provides a simple way to establish a pool of work to be distributed among multiple threads, 
	/// and wait for the work to complete before proceeding.  
    /// </remarks>
	public sealed class ParallelWhile<T>: ParallelLoop<T>  {
        /// <summary>
        /// Used for LIFO scheduling.
        /// </summary>
		private readonly Stack<T> m_Stack;
        /// <summary>
        /// Max size of LIFO allowed.
        /// </summary>
		private int m_CollectionLimit;
		/// <summary>
		/// See <see cref="ParallelLoop&lt;T&gt;.ProcessIterations">ProcessIterations</see>.
		/// </summary>
		internal sealed override void ProcessIterations() {
			for(;;) {
				T item;
				lock(m_Stack) {
					if( m_Stack.Count==0 ) return;
					item = m_Stack.Pop();
				}
				ClientAction(item);
			}
		}
        /// <summary>
        /// Add a work item.
        /// </summary>
        /// <remarks>
        /// This method can be called before or after method <see cref="BeginRun">BeginRun</see> is called.
        /// This method is always thread safe.
        /// </remarks>
        /// <param name="iterationValue">value of the iteration</param>
		public void Add(T iterationValue) {
			int collectionSize = 0;
			lock (m_Stack) {
				collectionSize = m_Stack.Count;
				if( collectionSize<m_CollectionLimit ) 
					m_Stack.Push(iterationValue);
			}
			if( ClientAction!=null ) {
				if( collectionSize<m_CollectionLimit )
                    // value was added to the collection
				    ConsiderRunningOneMoreThreadTask(0);
				else 
                    // value was not added to the collection. 
                    // It's up to us to deal with it.
					ClientAction(iterationValue);
			}
		}
		private void SetCollectionLimit() {
			if (m_NumberOfWorkers > 1)
				m_CollectionLimit=4*Math.Min(m_NumberOfWorkers, Int32.MaxValue/4); 
			else
				m_CollectionLimit = 0;
		}

        /// <summary>
        /// Construct a <see cref="ParallelWhile&lt;T&gt;">ParallelWhile</see> 
        /// with an initially empty collection of work items.
        /// </summary>
		/// <remarks>
		/// The loop does not start executing until at least method <see cref="BeginRun">BeginRun</see> is called,
		/// and possibly not until method <see cref="ParallelLoop&lt;T&gt;.EndRun">EndRun</see> is called.
		/// </remarks>
		/// <param name="numThreads">Maximum number of threads to employ.  
		/// If 0 then <see cref="ParallelEnvironment.MaxThreads">ParallelEnvironment.MaxThreads</see> is used as the maximum number of threads.
		/// </param>
		public ParallelWhile(int maxThreads) : base(maxThreads) {
			m_Stack = new Stack<T>();
			m_CollectionLimit = Int32.MaxValue;
		}
		/// <summary>
		/// Overload of constructor that uses the default number of threads.
		/// </summary>
		public ParallelWhile() : this(0) { }
        /// <summary>
        /// Begin processing work items.
        /// </summary>
        /// <param name="action">delegate that processes each work item</param>
		/// <param name="userCallback">an <see cref="System.AsyncCallback">AsyncCallback</see> to be called when the loop completes</param>
		/// <param name="stateObject">user-defined state object</param>
		public override void BeginRun(Action<T> action) {
			SetCollectionLimit();
			InternalBeginRun(action, m_Stack.Count);
		}

        /// <summary>
        /// Cancel any iterations that have not yet started.
        /// </summary>
        /// <remarks>
        /// Does not cancel any future iterations that might be added.
        /// </remarks>
		public override void Cancel() {
            InternalCancel();
			lock( m_Stack )
				m_Stack.Clear();
		}
	}

    /// <summary>
    /// A parallel loop over a collection.
    /// </summary>
	public sealed class ParallelForEach<T>: ParallelLoop<T> {
		private bool m_EndIsReached;
		private IEnumerator<T> m_Enumerator;
		internal sealed override void ProcessIterations() {
			for(;;) {
				T item;
				lock (m_Enumerator) {
					if( m_EndIsReached ) 
						return;
					if( !m_Enumerator.MoveNext() ) {
						m_EndIsReached=true;
						return;
					}
					item = m_Enumerator.Current;
				}
				ClientAction(item);
			}
		}
        /// <summary>
        /// Construct a parallel loop for iterating over a collection.
        /// </summary>
		/// <remarks>
		/// The loop does not start executing until at least method <see cref="BeginRun">BeginRun</see> is called,
		/// and possibly not until method <see cref="ParallelLoop&lt;T&gt;.EndRun">EndRun</see> is called.
		/// </remarks>
		/// <param name="collection">collection of values over which to iterate</param>
		public ParallelForEach(IEnumerable<T> collection, int maxThreads ) : base(maxThreads) {
			m_EndIsReached = false;
			m_Enumerator= collection.GetEnumerator();
		}

		/// <summary>
		/// Overload of constructor that uses the default number of threads.
		/// </summary>
		/// <param name="collection"></param>
		public ParallelForEach(IEnumerable<T> collection) : this(collection,0) { }

        /// <summary>
        /// Begin executing iterations.
        /// </summary>
		/// <param name="action">delegate that processes each work item</param>
		/// <param name="userCallback">an <see cref="System.AsyncCallback">AsyncCallback</see> to be called when the loop completes</param>
		/// <param name="stateObject">user-defined state object</param>
		public override void BeginRun(Action<T> action) {
			InternalBeginRun(action, Int32.MaxValue);
		}
        /// <summary>
        /// See <see cref="ParallelLoop&lt;T&gt;">ParallelLoop</see>
        /// </summary>
		public override void Cancel() {
			m_EndIsReached = true;
			InternalCancel();
		}
	}

    /// <summary>
    /// A parallel loop over consecutive integers, starting at 0.
    /// </summary>
	public sealed class ParallelFor: ParallelLoop<int> {
        /// <summary>
        /// One less than the next iteration value.
        /// </summary>
        /// <remarks>
        /// Its one less, and not the next iteration value itself, so that end-of-loop detection 
        /// works even if the last value is Int32.MaxValue.
        /// </remarks>
		private int myCurrent = -1;
        /// <summary>
        /// Total number of iterations to execute. 
        /// </summary>
        /// <remarks>
        /// Set by constructor and remains unchanged, unless method Cancel is called, in which
        /// case it is reset to zero.
        /// </remarks>
		private int myLimit;
        internal sealed override void ProcessIterations() {
#if STATIC_SCHEDULING
            int c = myLimit/MaxThreads;
#else
            // Simple-minded "guided scheduling" a la OpenMP.
            int m = m_NumberOfWorkers==1 ? 1 : 4*m_NumberOfWorkers;
#endif
            for (;;) {
                // Grab a chunk of iterations
                int k, l, n;
                // Lock on LocalCallback simply because it is handy, and avoids
                // locking on a user-visible object.
                lock (LocalCallback) {
                    k=myCurrent;
                    // Grab private copy of myLimit for calculations, because method Cancel might asynchronously change it.
                    l=myLimit;
                    if (l<=k) return;
#if STATIC_SCHEDULING
                    n=Math.Min(l-k,c);
#else
                    n=Math.Max((l-k)/m, 1);
#endif
                    myCurrent=k+n;
                }
                for (int j=0;j<n;++j)
                    ClientAction(k+j);
            }
        }
        /// <summary>
        /// Construct a parallel loop that will iterate over the integers 0..count-1.
        /// </summary>
        /// <remarks>
		/// The loop does not start executing until at least method <see cref="BeginRun">BeginRun</see> is called,
		/// and possibly not until method <see cref="ParallelLoop&lt;T&gt;.EndRun">EndRun</see> is called.
        /// </remarks>
        /// <param name="count">number of loop iterations</param>
		public ParallelFor( int count, int maxThreads ) : base(maxThreads) {
			if( count<0 )
				throw new ArgumentException("count must be non-negative");
			myLimit = count;
			myCurrent = 0;
		}
		/// <summary>
		/// Overload of constructor that uses <see cref="ParallelEnvironment.MaxThreads">ParallelEnvironment.MaxThreads</see> threads.
		/// </summary>
		/// <param name="count"></param>
		public ParallelFor(int count) : this(count,0) { }

		/// <summary>
		/// See <see cref="ParallelLoop&lt;T&gt;.BeginRun">ParallelLoop.BeginRun</see>
		/// </summary>
		public override void BeginRun(Action<int> action) {
			InternalBeginRun(action, myLimit);
		}
        /// <summary>
        /// See <see cref="ParallelLoop&lt;T&gt;.Cancel">ParallelLoop.Cancel</see>
        /// </summary>
		public override void Cancel() {
			// Need volatile write here to ensure that value it read by other threads.
			Thread.VolatileWrite(ref myLimit,0);
			InternalCancel();
		}
	}
}

////==================== END OF FILE ====================


