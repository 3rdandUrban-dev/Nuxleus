using System;
using System.Runtime.InteropServices;
using Manos.IO;

namespace Libev
{
	abstract class Watcher : IBaseWatcher
	{
		protected IntPtr watcher_ptr;
		private bool disposed;
		protected GCHandle gc_handle;

		internal Watcher (Loop loop)
		{
			Loop = loop;
			gc_handle = GCHandle.Alloc (this);
		}

		public Loop Loop { 
			get;
			private set;
		}

		public bool IsRunning {
			get;
			private set;
		}

		~Watcher ()
		{
			Dispose ();
		}

		public virtual void Dispose ()
		{
			if (disposed) {
				return;
			}
			Stop ();
			DestroyWatcher ();
			watcher_ptr = IntPtr.Zero;
			gc_handle.Free ();
			GC.SuppressFinalize (this);
			disposed = true;
		}

		public virtual void Start ()
		{
			if (IsRunning)
				return;
			IsRunning = true;
			StartImpl ();
		}

		public virtual void Stop ()
		{
			if (!IsRunning)
				return;
			IsRunning = false;
			StopImpl ();
		}

		protected abstract void StartImpl ();

		protected abstract void StopImpl ();

		protected abstract void DestroyWatcher ();
	}
}
