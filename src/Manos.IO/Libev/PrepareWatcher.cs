using System;
using System.Runtime.InteropServices;
using Manos;
using Manos.IO;

namespace Libev
{
	class PrepareWatcher : Watcher, IPrepareWatcher
	{
		private Action<PrepareWatcher, EventTypes> callback;
		private static UnmanagedWatcherCallback unmanaged_callback;

		static PrepareWatcher ()
		{
			unmanaged_callback = StaticCallback;
		}

		public PrepareWatcher (Loop loop, Action<PrepareWatcher, EventTypes> callback) : base (loop)
		{
			this.callback = callback;
			
			watcher_ptr = manos_prepare_watcher_create (unmanaged_callback, GCHandle.ToIntPtr (gc_handle));
		}

		protected override void DestroyWatcher ()
		{
			manos_prepare_watcher_destroy (watcher_ptr);
		}

		private static void StaticCallback (IntPtr data, EventTypes revents)
		{
			try {
				var handle = GCHandle.FromIntPtr (data);
				var watcher = (PrepareWatcher) handle.Target;
				watcher.callback (watcher, revents);
			} catch (Exception e) {
				Console.Error.WriteLine ("Error handling prepare event: {0}", e.Message);
				Console.Error.WriteLine (e.StackTrace);
			}
		}

		protected override void StartImpl ()
		{
			ev_prepare_start (Loop.Handle, watcher_ptr);
		}

		protected override void StopImpl ()
		{
			ev_prepare_stop (Loop.Handle, watcher_ptr);
		}

		[DllImport ("libev", CallingConvention = CallingConvention.Cdecl)]
		private static extern void ev_prepare_start (IntPtr loop, IntPtr watcher);

		[DllImport ("libev", CallingConvention = CallingConvention.Cdecl)]
		private static extern void ev_prepare_stop (IntPtr loop, IntPtr watcher);

		[DllImport ("libmanos", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr manos_prepare_watcher_create (UnmanagedWatcherCallback callback, IntPtr data);

		[DllImport ("libmanos", CallingConvention = CallingConvention.Cdecl)]
		private static extern void manos_prepare_watcher_destroy (IntPtr watcher);
	}
}

